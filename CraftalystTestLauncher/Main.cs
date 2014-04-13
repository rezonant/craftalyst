using System;
using Craftalyst;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CraftalystTestLauncher {
	class MainClass {
		public static void Main (string[] args)
		{
			var craftalyst = new Context ();

			craftalyst.AppName = "Craftalyst 1337 Launcher";
			craftalyst.AppVersion = "0.0.1a6";
			craftalyst.AppId = "CraftalystLauncher";

			string username = "";
			string password = "";
			MinecraftSession session = null;

			while (true) {
				Console.WriteLine ();
				Console.WriteLine ("Craftalyst {0}", Context.Version);
				Console.WriteLine ("Warning: The server play.tirrin.com will control what software components are downloaded and from where.");
				Console.WriteLine ("         Only use Craftalyst with servers you trust.");
				Console.WriteLine ();
				Console.WriteLine ("(C) 2014 rezonaut");
				Console.Write ("Minecraft Login : ");
				username = Console.ReadLine ();
				Console.Write ("       Password : ");

				ConsoleKeyInfo input;
				StringBuilder buffer = new StringBuilder();
				while (true) {
					input = Console.ReadKey(true);
					if (input.Key == ConsoleKey.Enter) {
						Console.WriteLine();
						break;
					}
					buffer.Append (input.KeyChar);
				}
				password = buffer.ToString();
				buffer = null;

				var auth = new MinecraftAuthentication();
				bool loggedIn = false;

				try {
					session = auth.Login(username, password);

					if (session != null)
						loggedIn = true;

				} catch (Exception e) {
					Console.WriteLine();
					Console.WriteLine("Invalid Login: "+e.Message);
					Console.WriteLine(e);

					Thread.Sleep(100000);

					loggedIn = false;
				}

				Console.Clear();

				if (loggedIn)
					break;
			}
			
			Console.Clear();

			// LET'S GET GOING!
			Console.WriteLine ();
			Console.WriteLine ("Let's do this!");
			Console.WriteLine ();

			Instance instance = craftalyst.GetOrCreateInstance("CraftalystTest", new InstanceDescription() {
				Version = "1.6.4-forge",
				Name = "Tirrin",
				Description = "Client instance for Tirrin server",
				SyncUrl = "http://play.tirrin.com:8080/craftalyst",
				SyncConfigs = new List<string>(),
				Mods = new List<ModDescription>(),
				ConfigVersion = 0
			});
				
			var mc = instance.CreateMinecraft();
			mc.Start(session);
		}
	}
}
