using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace Mithril
{
	public sealed class Connection
	{
		private int HandshakeNumber { get; set; }
		public bool Connected { get; private set; }

		private IMithrilBase Receiver { get; set; }
		public EndPoint EndPoint { get; private set; }
		public int Id { get; private set; }
		private Socket Socket { get; set; }

		private Dictionary<byte, ReliablePacket> Packets { get; } = new Dictionary<byte, ReliablePacket>();
		private byte nextPacketId = 0;

		private int ExpectedId { get; set; }
		private Dictionary<int, Mithril.Buffer> Unexpected { get; } = new Dictionary<int, Mithril.Buffer>();
		private Timer expectingTimeout = new Timer(2_000); // ms
		private Timer pingTimer = new Timer(10_000);

		public Connection(IMithrilBase receiver, int id, EndPoint endPoint, Socket socket)
		{
			Receiver = receiver;
			Id = id;
			EndPoint = endPoint;
			Socket = socket;

			expectingTimeout.AutoReset = false;
			expectingTimeout.Elapsed += ExpectingTimeout;
			pingTimer.Elapsed += PingTimerElapsed;
		}

		public void InitializeHandshake()
		{
			Random random = new Random();
			HandshakeNumber = random.Next(int.MaxValue / 2);
			Mithril.Buffer packet = new Mithril.Buffer(5);
			packet.WriteByte(Common.ICHI);
			packet.WriteInt(HandshakeNumber);
			Send(packet);
		}

		private void HandshakeComplete()
		{
			pingTimer.Start();
			Connected = true;
			Receiver.FireConnectEvent(Id);
		}

		public void Disconnect()
		{
			Mithril.Buffer packet = new Mithril.Buffer();
			packet.WriteByte(Common.DISCONNECT);
			packet.WriteByte(Common.R_DISCONNECT);
			Send(packet);
		}

		private void Send(Mithril.Buffer buffer)
		{
			Socket.SendTo(buffer.Data, buffer.Count, SocketFlags.None, EndPoint);
		}

		public void SendUnreliable(Mithril.Buffer buffer, bool wholeBuffer)
		{
			Mithril.Buffer packet = new Mithril.Buffer();
			packet.WriteByte(Common.UNRELIABLE);
			packet.Write(buffer, wholeBuffer);
			Send(packet);
		}

		public void SendReliable(Mithril.Buffer buffer, bool wholeBuffer)
		{
			CalcNextPacketId();

			Mithril.Buffer data = new Mithril.Buffer();
			data.WriteByte(Common.RELIABLE);
			data.WriteByte(nextPacketId);
			data.Write(buffer, wholeBuffer);

			ReliablePacket packet = new ReliablePacket(this, nextPacketId, data);
			Packets.Add(packet.Id, packet);

			Send(data);
		}

		private void CalcNextPacketId()
		{
			nextPacketId = (byte)((nextPacketId + 1) % byte.MaxValue);
		}

		private void CancelSend(byte id)
		{
			Packets.Remove(id);
		}

		public void OnReceive(Mithril.Buffer buffer)
		{
			switch (buffer.ReadByte())
			{
				// we initialized handshake
				case Common.NI:
					if (buffer.ReadInt() == HandshakeNumber + 10)
					{
						HandshakeNumber = buffer.ReadInt();

						Mithril.Buffer packet = new Mithril.Buffer();
						packet.WriteByte(Common.SAN);
						packet.WriteInt(HandshakeNumber + 10);
						HandshakeNumber = new Random().Next(int.MaxValue / 2);
						packet.WriteInt(HandshakeNumber);
						Send(packet);
					}
					else
					{
						Disconnect();
					}
					break;
				case Common.YON:
					if (buffer.ReadInt() == HandshakeNumber + 10)
					{
						HandshakeComplete();
					}
					else
					{
						Disconnect();
					}
					break;
				// remote initialize handshake
				case Common.ICHI:
					{
						HandshakeNumber = buffer.ReadInt();

						Mithril.Buffer packet = new Mithril.Buffer();
						packet.WriteByte(Common.NI);
						packet.WriteInt(HandshakeNumber + 10);
						HandshakeNumber = new Random().Next(int.MaxValue / 2);
						packet.WriteInt(HandshakeNumber);
						Send(packet);
						break;
					}
				case Common.SAN:
					if (buffer.ReadInt() == HandshakeNumber + 10)
					{
						HandshakeNumber = buffer.ReadInt();

						Mithril.Buffer packet = new Mithril.Buffer();
						packet.WriteByte(Common.YON);
						packet.WriteInt(HandshakeNumber + 10);
						Send(packet);

						HandshakeComplete();
					}
					else
					{
						Disconnect();
					}
					break;
				case Common.PING:
					Mithril.Buffer pingAck = new Mithril.Buffer(1);
					pingAck.WriteByte(Common.PING_ACK);
					Send(pingAck);
					break;
				case Common.PING_ACK:
					break;
				case Common.UNRELIABLE:
					Receiver.FireReceiveEvent(Id, buffer);
					break;
				case Common.RELIABLE:
					byte actualId = buffer.ReadByte();

					Mithril.Buffer ack = new Mithril.Buffer();
					ack.WriteByte(Common.ACK);
					ack.WriteByte(actualId);
					Send(ack);

					if (ExpectedId == actualId)
					{
						Receiver.FireReceiveEvent(Id, buffer);
						ExpectedId = (ExpectedId + 1) % int.MaxValue;
						CheckForDelivery();
					}
					else
					{
						Unexpected.Add(actualId, buffer);

						if (!expectingTimeout.Enabled)
						{
							expectingTimeout.Start();
						}
					}
					break;
				case Common.ACK:
					byte id = buffer.ReadByte();
					Packets[id]?.Timer.Stop();
					CancelSend(id);
					break;
				case Common.DISCONNECT:
					Receiver.FireDisconnectEvent(Id, buffer.ReadByte());
					break;
			}

			pingTimer.Stop();
			pingTimer.Start();
		}

		private void ExpectingTimeout(object sender, ElapsedEventArgs e)
		{
			ExpectedId = (ExpectedId + 1) % int.MaxValue;
			CheckForDelivery();
		}

		private void CheckForDelivery()
		{
			while (Unexpected.ContainsKey(ExpectedId))
			{
				Receiver.FireReceiveEvent(Id, Unexpected[ExpectedId]);
				Unexpected.Remove(ExpectedId);
				ExpectedId = (ExpectedId + 1) % int.MaxValue;
			}

			if (Unexpected.Count > 0)
			{
				expectingTimeout.Stop();
				expectingTimeout.Start();
			}
		}

		private void PingTimerElapsed(object sender, ElapsedEventArgs e)
		{
			Mithril.Buffer packet = new Mithril.Buffer();
			packet.WriteByte(Common.PING);
			Send(packet);
		}

		public void Cleanup()
		{
			expectingTimeout.Stop();
			pingTimer.Stop();
		}

		class ReliablePacket
		{
			private Connection Connection { get; set; }
			public byte Id { get; private set; }
			public Mithril.Buffer Buffer { get; private set; }

			/// <summary>
			/// Retry timer
			/// </summary>
			public Timer Timer { get; private set; }
			private int Tries { get; set; }

			public ReliablePacket(Connection connection, byte id, Mithril.Buffer buffer)
			{
				Connection = connection;
				Id = id;
				Buffer = buffer;
				Timer = new Timer(1000);
				Timer.Elapsed += OnRetry;
				Timer.Start();
			}

			private void OnRetry(object sender, ElapsedEventArgs e)
			{
				Connection.Send(Buffer);
				Tries++;

				if (Tries >= 10)
				{
					Connection.CancelSend(Id);
					Timer.Stop();
				}
			}
		}
	}
}
