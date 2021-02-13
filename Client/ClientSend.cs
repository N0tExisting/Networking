using System;
using System.Collections.Generic;
using System.Text;

namespace Client {
	class ClientSend {
		public static void SendTcpData (Packet packet) {
			packet.WriteLength();
			Client.Instance.tcp.SendData(packet);
		}
		public static void SendUdpData (Packet packet) {
			packet.WriteLength();
			Client.Instance.udp.SendData(packet);
		}
		public static void WelcomeRecS (bool valid, int? hash = null) {
			using (Packet packet = new Packet((int) ClientPackets.welcomeReceived)) {
				packet.Write(Client.Instance.id);
				packet.Write(valid);
				if (!valid)
					packet.Write((int)hash);
				packet.Write(Program.Name);
				SendTcpData(packet);
			}
		}
		public static void Message (string mesage) {
			using (Packet packet = new Packet((int)ClientPackets.Message)){
				//packet.Write(Client.Instance.id);
				packet.Write(mesage);
				SendUdpData(packet);
			}
		}
		public static long time = DateTime.Now.Ticks;
		public static readonly Packet pong = new Packet(2); // udpPong
		public static void PingPongS (long sTime) {
			Packet p = pong;
			p.Write(sTime);
			p.Write(time);
			SendUdpData(p);
			p.Dispose();
		}
	}
}