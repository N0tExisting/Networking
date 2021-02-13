using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Client {
	class ClientHandle {
		public static void WelcomeR (Packet packet) {
			Client.Instance.id = packet.ReadInt();
			int sTps = packet.ReadInt();
			string msg = packet.ReadString();
			int ServerHash = packet.ReadInt();
			int thisHash = HashCode.Combine(Client.Instance.tcp.socket.Client.RemoteEndPoint.ToString(), Client.Instance.port);
			Console.WriteLine($"Server TPS is: {sTps}");
			Client.Instance.udp.Conect(((IPEndPoint) Client.Instance.tcp.socket.Client.LocalEndPoint).Port);

			if (ServerHash != thisHash) {
				Console.WriteLine($"Servers Hash ({ServerHash}) was not equal to our Hash ({thisHash})");
				Console.WriteLine($"Message from the Server : {msg}");
				ClientSend.WelcomeRecS(false, thisHash);

			} else {
				Console.WriteLine($"Message from the Server {ServerHash}: {msg}");
				ClientSend.WelcomeRecS(true);
			}
		}
	}
}
