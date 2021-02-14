using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Server {
	static class Server {
		public static int MaxClients { get; private set; }
		public static int Port { get; private set; }
		public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
		public static TcpListener tcpListener{ get; private set; }
		//public static UdpClient udpListener{ get; private set; }
		public delegate void PacketManager(int from, Packet p);
		public static Dictionary<int, PacketManager> Managers = new Dictionary<int, PacketManager>();
		public static void Start (int maxClients, int port) {
			MaxClients = maxClients;
			Port = port;

			Console.WriteLine("Starting server...");

			InitializeServerData();
			//udpListener = new UdpClient(port);
			tcpListener = new TcpListener(IPAddress.Any, Port);
			tcpListener.Start();
			tcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpCallback), null);
			//udpListener.BeginReceive(UdpRecieveCallback, null);

			Console.WriteLine($"Server started on port: {Port}.");
		}
		private static void TcpCallback (IAsyncResult ar) {
			TcpClient client = tcpListener.EndAcceptTcpClient(ar);
			tcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpCallback), null);
			Console.WriteLine($"Incoming conection from {client.Client.RemoteEndPoint}...");
			for (int i = 1; i <= MaxClients; i++) {
				if (clients[i].tcp.socket == null) {
					clients[i].tcp.Connect(client);
					Console.WriteLine($"Conected {client.Client.RemoteEndPoint} as client {i}");
					return;
				}
			}
			Console.WriteLine($"{client.Client.RemoteEndPoint} cailed to conect: Server full!");
		}
		//private static void UdpRecieveCallback (IAsyncResult ar) {
		//	try {
		//		IPEndPoint ClientEndPoint = new IPEndPoint(IPAddress.Any, 0);
		//		byte[] data = udpListener.EndReceive(ar, ref ClientEndPoint);
		//		udpListener.BeginReceive(UdpRecieveCallback, null);
		//
		//		if (data.Length < 4) return;
		//		using (Packet packet = new Packet(data)) {
		//			int CID = packet.ReadInt();
		//			if (CID == 0) return;
		//			if (clients[CID].udp.endPoint == null) {
		//				clients[CID].udp.Connect(ClientEndPoint);
		//				return;
		//			}
		//			if (clients[CID].udp.endPoint.ToString() == ClientEndPoint.ToString()) {
		//				clients[CID].udp.HandleData(packet);
		//			}
		//		}
		//	} catch (Exception e) {
		//		Console.WriteLine($"Error recieving UDP data:\n{e}");
		//	}
		//}
		//public static void SendUdpData(IPEndPoint endPoint, Packet packet) {
		//	try {
		//		if (endPoint != null) {
		//			udpListener.BeginSend(packet.ToArray(), packet.Length(), null, null);
		//		}
		//	} catch (Exception e) {
		//		Console.WriteLine($"Error sending UDP data:\n{e}");
		//	}
		//}
		private static void InitializeServerData () {
			for (int i = 1; i <= MaxClients; i++) {
				clients.Add(i, new Client(i));
			}
			Console.WriteLine("Prepared Clients");

			Managers.Add((int)ClientPackets.welcomeReceived, ServerHandle.WelcomeRecieved);
			Managers.Add((int)ClientPackets.Message, ServerHandle.Message);
			Console.WriteLine("Initalized packets.");
		}
	}
}