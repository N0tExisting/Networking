using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Server {
	class Client {
		public static int dataBufferSize = 4096;

		public TCP tcp;
		public int id;
		public string name;
		public bool used;

		public Client (int id) {
			this.id = id;
			tcp = new TCP(id);
		}

		public class TCP {
			public TcpClient socket;
			private readonly int id;
			private Packet recievedData;
			private NetworkStream stream;
			private byte[] buffer;
			public TCP (int _id) {
				id = _id;
			}
			public void Connect (TcpClient _socket) {
				socket = _socket;
				socket.ReceiveBufferSize = dataBufferSize;
				socket.SendBufferSize = dataBufferSize;

				stream = socket.GetStream();

				recievedData = new Packet();
				buffer = new byte[dataBufferSize];

				stream.BeginRead(buffer, 0, dataBufferSize, new AsyncCallback(RecieveCallback), null);

				ServerSend.Welcome(id);
			}
			public void SendTcpPacket (Packet packet) {
				try {
					if (socket != null) {
						stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
					}
				} catch (Exception e) {
					Console.WriteLine($"Error sending data to player {id} via TCP: {e}");
				}
			}
			private void RecieveCallback(IAsyncResult res) {
				try {
					int byteLenght = stream.EndRead(res);
					if (byteLenght <= 0)
						return;
					byte[] data = new byte[byteLenght];
					Array.Copy(data, buffer, byteLenght);

					recievedData.Reset(handleData(data));

					stream.BeginRead(buffer, 0, dataBufferSize, new AsyncCallback(RecieveCallback), null);
				} catch (Exception e) {
					Console.WriteLine($"Error recieving TCP data:\n{e}");
					if (e.ToString().Contains("Unable to read data from the transport connection: An existing connection was forcibly closed by the remote host")) {
						this.socket = null;
					}
					//throw;
				}
			}
			private bool handleData (byte[] data) {
				int pLenght = 0;
				recievedData.SetBytes(data);
				if (recievedData.UnreadLength() >= 4) {
					pLenght = recievedData.ReadInt();
					if (pLenght <= 0)
						return true;
				}
				while (pLenght > 0 && pLenght <= recievedData.UnreadLength()) {
					byte[] pBytes = recievedData.ReadBytes(pLenght);
					ThreadManager.ExecuteOnMainThread(() => {
						using (Packet P = new Packet(pBytes)) {
							int PID = P.ReadInt();
							Server.Managers[PID](id, P);
						}
					});
					pLenght = 0;
					if (recievedData.UnreadLength() >= 4) {
						pLenght = recievedData.ReadInt();
						if (pLenght <= 0)
							return true;
					}
				}
				if (pLenght <= 1)
					return true;
				else
					return false;
			}
		}
	}
}