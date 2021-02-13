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
	}
}