using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace Client {
	class Client {
		public static Client Instance = null;
		public readonly static int dataBufferSize = 4096;
		//public int ServerHash;
		public string ip = "127.0.0.1";
		public int port = 2925;
		public int id;
		public TCP tcp;
		public UDP udp;
		private delegate void PacketHandler(Packet p);
		private static Dictionary<int, PacketHandler> Handlers;

		public Client () {

			if (Instance == null)
				Instance = this;
			else if (Instance != this) {
				//this = null;
				//delete(this);
				throw new InvalidOperationException("Could not Create a new client, because an instance already Exists");
				//return;
			}
			//this.id = id;
			tcp = new TCP();
			udp = new UDP();
		}
		public void ConectToServer () {
			tcp.Connect();
			Init();
		}

		public class TCP {
			public TcpClient socket;
			private Packet recievedData;
			private NetworkStream stream;
			private byte[] buffer;
			public ulong PID { get; private set; } = 0;

			public TCP () {
				
			}
			public void Connect (/*TcpClient _socket*/) {
				socket =new TcpClient() { 
					ReceiveBufferSize = dataBufferSize,
					SendBufferSize = dataBufferSize,
				};

				socket.BeginConnect(Instance.ip, Instance.port, ConectCallback, socket);
				Console.WriteLine($"Conecting to: {Instance.ip} : {Instance.port}");
				buffer = new byte[dataBufferSize];

			}
			private void ConectCallback (IAsyncResult res) {
				socket.EndConnect(res);

				if (!socket.Connected) {
					Console.WriteLine($"Could not conect to: {Instance.ip} : {Instance.port}");
					return;
				}
				Console.WriteLine($"Conected to: {Instance.ip} : {Instance.port}");

				recievedData = new Packet();

				stream = socket.GetStream();
				stream.BeginRead(buffer, 0, dataBufferSize, new AsyncCallback(RecieveCallback), null);
			}
			private void RecieveCallback (IAsyncResult res) {
				try {
					int byteLenght = stream.EndRead(res);
					if (byteLenght <= 0)
						return;
					byte[] data = new byte[byteLenght];
					Array.Copy(data, buffer, byteLenght);

					recievedData.Reset(handleData(data));

					stream.BeginRead(buffer, 0, dataBufferSize, new AsyncCallback(RecieveCallback), null);
				} catch (Exception e) {
					Console.WriteLine($"Error recieving TCP data {e}");
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
							Handlers[PID](P);
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
			public void SendData (Packet packet) {
				try {
					if (socket != null)
						stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
				} catch (Exception e) {
					Console.WriteLine($"Error sending data to server via TCP: {e}");
					//throw;
				}
			}
		}

		public class UDP {
			public UdpClient socket;
			public IPEndPoint endPoint;
			public UDP () {
				endPoint = new IPEndPoint(IPAddress.Parse(Instance.ip), Instance.port);
			}
			public void Conect(int localP) {
				socket = new UdpClient(localP);
				socket.Connect(endPoint);
				socket.BeginReceive(RecieveCallback, null);
			}
			public void SendData (Packet packet) {
				try {
					packet.InsertInt(Instance.id);
					if (socket != null) {
						socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
					}
				} catch (Exception e) {
					Console.WriteLine($"Error sending data via UDP: {e}");
					//throw;
				}
			}
			private void RecieveCallback (IAsyncResult ar) {
				try {
					byte[] data = socket.EndReceive(ar, ref endPoint);
					if (data.Length < 4) {
						// TODO: nothing;
						return;
					}
					HandleData(data);
				} catch (Exception e) {

					// throw;
				}
			}
			private void HandleData (byte[] data) {
				using (Packet packet = new Packet(data)) {
					int pLenght = packet.Length();
					data = packet.ToArray();
				}
				ThreadManager.ExecuteOnMainThread(() => {
					using (Packet packet = new Packet(data)) {
						int PID = packet.ReadInt();
						Handlers[PID](packet);
					}
				});
			}
		}
		private void Init () {
			Handlers = new Dictionary<int, PacketHandler>();
			Handlers.Add((int) ServerPackets.welcome, ClientHandle.WelcomeR);
			Console.WriteLine("Initialized Packets");
		}
	}
}