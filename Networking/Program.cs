using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server {
	class Program {
		public static bool isRunning { get; private set; } = false;
		public static int TPS { get; private set; } = 30;
		public static double DeltaT => 1000.0 / TPS;
		static void Main(string[] args) {
			Console.Title = "Server";
			Console.WriteLine("Hello World!");
			isRunning = true;

			Thread mainThead = new Thread(new ThreadStart(MainThead));
			mainThead.Start();
			//Time();

			Server.Start(50, 2925);
		}
		//public static async Task Time () {
		//	await Task.Delay(1);
		//	while (true)
		//		ServerSend.time = DateTime.Now.Ticks;
		//}
		private static void MainThead () {
			Console.WriteLine($"Main thead started. Running at {TPS} ticks per second");
			DateTime next = DateTime.Now;
			while (isRunning) {
				while (next < DateTime.Now) {
					ThreadManager.UpdateMain();
					next = next.AddMilliseconds(DeltaT);
					if (next > DateTime.Now)
						Thread.Sleep(next - DateTime.Now.AddMilliseconds(3));
				}
			}
		}
	}
}