using System;
using System.Threading;
using System.Threading.Tasks;

// https://youtu.be/QajkUJeypy4?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5&t=576

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
			Thread mainThread = new Thread(new ThreadStart(MainThread));
			mainThread.Start();
			//Time();

			new Client();
			Client.Instance.ConectToServer();

			Thread textThread = new Thread(new ThreadStart(TextThread));
			textThread.Start();
		}
		private static void TextThread () {
			while (isRunning) {
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.Write($">{Name}=-> ");
				Console.ForegroundColor = ConsoleColor.DarkBlue;
				ClientSend.Message(Console.ReadLine());
				Console.ResetColor();
			}


		}
		public static async Task Time () {
			await Task.Delay(1);
			while (isRunning)
				ClientSend.time = DateTime.Now.Ticks;
		}
		private static void MainThread () {
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
						Thread.Sleep(time - DateTime.Now.AddMilliseconds(-3));
				}
			}
		}
	}
}