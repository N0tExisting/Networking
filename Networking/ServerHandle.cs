using System;
using System.Collections.Generic;
using System.Text;

namespace Server {
	class ServerHandle {
		public static void WelcomeRecieved (int from, Packet packet) {
			int CID = packet.ReadInt();
			bool worked = packet.ReadBool();
			int hash;
			if (!worked) {
				hash = packet.ReadInt();
				Console.WriteLine($"Client {from}'s Hash({hash}) did not match our Hash");
			}
			string Name = packet.ReadString();
			Server.clients[from].name = Name;
			Server.clients[from].used = true;
			if (CID != from) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Client '{Name}' ({Server.clients[from].tcp.socket.Client.RemoteEndPoint})" +
					$" (ID : {from}) thinks he has the id of {CID}!");
				Console.ResetColor();
				Console.Beep(50, 100);
				return;
			}
			Console.WriteLine($"Client {from}({Server.clients[from].tcp.socket.Client.RemoteEndPoint})" +
				$"conected succesfuly and is named: \'{Name}\'");
		}
		public static void Message (int from, Packet packet) {
			string msg = packet.ReadString();
			if (string.IsNullOrWhiteSpace(msg)) return;
			msg = msg.Trim();
			Console.Write(from);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write($">{Server.clients[from].name}=-> ");
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine(msg);
			ServerSend.Text(from, msg);
		}
	}
}