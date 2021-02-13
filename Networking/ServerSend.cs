using System;
using System.Collections.Generic;
using System.Text;

namespace Server {
	static class ServerSend {
		public static void SendTCPData (int to, Packet packet) {
			packet.WriteLength();
			Server.clients[to].tcp.SendTcpPacket(packet);
		}
		public static void SendTCPDataToAll (Packet packet) {
			packet.WriteLength();
			for (int i = 1; i <= Server.MaxClients; i++)
				Server.clients[i].tcp.SendTcpPacket(packet);
		}
		public static void SendTCPDataToAll (int exempt, Packet packet) {
			packet.WriteLength();
			for (int i = 1; i <= Server.MaxClients; i++)
				if (i != exempt)
					Server.clients[i].tcp.SendTcpPacket(packet);
		}

		public static void Welcome (int to) {
			Packet packet = new Packet((int) ServerPackets.welcome);
			packet.Write(to);
			packet.Write(Program.TPS);
			packet.Write("Welcome to the server.");
			packet.Write(HashCode.Combine(Server.tcpListener.Server.LocalEndPoint.ToString(), Server.Port/*, Server.MaxClients*/));
			SendTCPData(to, packet);
			packet.Dispose();
		}
	}
}