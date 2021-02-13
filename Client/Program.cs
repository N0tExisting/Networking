using System;
using System.Threading;

// https://youtu.be/uh8XaC0Y5MA?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5&t=454

namespace Client {
	class Program {
		public static bool isRunning = false;
		public static string Name { get; private set; } = "Not Existing";
		public static int TPS = 5;
		public static double DeltaT => 1000.0 / TPS;
		static void Main(string[] args) {
			Console.Title = "Client";
			Console.WriteLine("Hello World!");

			isRunning = true;
			Thread mainThead = new Thread(new ThreadStart(MainThead));
			mainThead.Start();

			new Client();
			Client.Instance.ConectToServer();
		}
		private static void MainThead () {
			Console.WriteLine($"Main thead started. Running at {TPS} ticks per second");
			var time = DateTime.Now.AddMilliseconds(DeltaT);
			int oldTPS = TPS;
			while (isRunning) {
				while (time < DateTime.Now) {
					time = time.AddMilliseconds(DeltaT);
					ThreadManager.UpdateMain();
					if (oldTPS != TPS) {
						oldTPS = TPS;
						Console.WriteLine($"Main thead now running at {TPS} ticks per second");
					}
					if (time > DateTime.Now)
						Thread.Sleep(time - DateTime.Now.AddMilliseconds(3));
				}
			}
		}
	}
}