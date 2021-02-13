using System;
using System.Collections.Generic;
using System.Text;

namespace Server {
	static class ServerSend {
		public static long time = DateTime.Now.Ticks;
		public static void SendTcpData (int to, Packet packet) {
			packet.WriteLength();
			Server.clients[to].tcp.SendTcpPacket(packet);
		}
		public static void SendTcpDataToAll (Packet packet) {
			packet.WriteLength();
			for (int i = 1; i <= Server.MaxClients; i++)
				Server.clients[i].tcp.SendTcpPacket(packet);
		}
		public static void SendTcpDataToAll (int exempt, Packet packet) {
			packet.WriteLength();
			for (int i = 1; i <= Server.MaxClients; i++)
				if (i != exempt)
					Server.clients[i].tcp.SendTcpPacket(packet);
		}
		public static void SendUdpData (int to, Packet packet) {
			packet.WriteLength();
			Server.clients[to].udp.SendData(packet);
		}
		public static void SendUdpDataToAll (Packet packet) {
			packet.WriteLength();
			for (int i = 1; i <= Server.MaxClients; i++)
				Server.clients[i].udp.SendData(packet);
		}
		public static void SendUdpDataToAll (int exempt, Packet packet) {
			packet.WriteLength();
			for (int i = 1; i <= Server.MaxClients; i++)
				if (i != exempt)
					Server.clients[i].udp.SendData(packet);
		}
		public static void Welcome (int to) {
			Packet packet = new Packet((int) ServerPackets.welcome);
			packet.Write(to);
			packet.Write(Program.TPS);
			packet.Write("Welcome to the server.");
			packet.Write(HashCode.Combine(Server.tcpListener.Server.LocalEndPoint.ToString(), Server.Port/*, Server.MaxClients*/));
			SendTcpData(to, packet);
			packet.Dispose();
		}
		public static void Text (int from, string msg) {
			Packet packet = new Packet((int)ServerPackets.Message);
			packet.Write(Server.clients[from].name);
			packet.Write(msg);
			SendUdpDataToAll(from, packet);
			packet.Dispose();
		}
	}
}