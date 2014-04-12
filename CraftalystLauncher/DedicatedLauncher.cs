using System;
using Gtk;
using Craftalyst;
using System.Collections.Generic;
using System.Threading;
using System.Timers;

namespace CraftalystLauncher
{
	public class DedicatedLauncher
	{
		public DedicatedLauncher (string instanceName, InstanceDescription defaultInstanceDescription)
		{
			Craft = new Context ();
			Craft.AppName = "Craftalyst Launcher";
			Craft.AppVersion = "0.0.1a1";
			
			var instance = Craft.GetInstance (instanceName);

			if (instance == null)
				instance = Craft.CreateInstance(instanceName, defaultInstanceDescription);

			Instance = instance;

			singleton = this;
		}

		public Context Craft { get; set; }
		public Instance Instance { get; set; }

		public static void MessageBox (string msg, params object[] args)
		{
			var dialog = new MessageDialog(null, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, msg, args);

			dialog.Run();
			dialog.Destroy();
		}

		public MinecraftSession Session { get; set; }

		public void Login()
		{
			
			LoginDialog login = new LoginDialog ();

			while (true) {
				int response = login.Run ();

				if (response == (int)Gtk.ResponseType.Ok) {

					var auth = new MinecraftAuthentication ();
					string message = "Invalid login!";

					try {
						Session = auth.Login (login.Username, login.Password);
					} catch (Exception e) {
						Session = null;
						message = e.Message;
					}

					if (Session == null || Session.AccessToken == null) {
						MessageBox (message);
						continue;
					}

					break;
				}
			}

			login.Hide ();
			login.Destroy();
		}

		static DedicatedLauncher singleton = null;

		public static DedicatedLauncher Singleton {
			get {
				return singleton;
			}
		}

		public void EnqueueUxJob (System.Action a)
		{
			lock (this.jobQueue)
				this.jobQueue.Enqueue(a);
		}

		private Queue<System.Action> jobQueue = new Queue<System.Action>();

		public void Run ()
		{
			Thread.CurrentThread.Name = "MAIN";

			Login ();
			
			var installDialog = new InstallationDialog ();
			installDialog.Show ();
			installDialog.ShowAll();

			ControlThread = new Thread(delegate() {
				if (Instance.IsNewInstance) {
					Install(installDialog);
				}

				EnqueueUxJob(delegate() {
					installDialog.Destroy();
					Launch();
				});
			});

			ControlThread.Start();

			Function delly;

			delly = delegate () {
				try {
					Gdk.Threads.Enter();
					lock (jobQueue) {
						System.Action job = null;
						while (jobQueue.Count > 0 && (job = jobQueue.Dequeue()) != null)
							job();
					}
					Gdk.Threads.Leave();

				} catch (Exception exc) {
					Console.WriteLine (exc);
				}

				Gtk.Timeout.Add (100, delly);
				return false;
			};

			Gtk.Timeout.Add (100, delly);

			Application.Run ();
		}

		public Thread ControlThread { get; set; }

		public void Install (InstallationDialog installDialog)
		{
			this.Instance.Install(new InstallationDialogStatusListener(installDialog));
		}

		public void Sync (InstallationDialog installDialog)
		{
			this.Instance.Sync (new InstallationDialogStatusListener(installDialog));
		}

		public void Launch()
		{
			MainWindow win = new MainWindow (Instance, Session);
			win.Show ();
		}

		public static void Main (string[] args)
		{
			Application.Init ();

			var crafter = new DedicatedLauncher("Tirrin", new InstanceDescription() {
				Version = "1.6.4-forge",
				Name = "Tirrin",
				Description = "Client instance for Tirrin server",
				SyncUrl = "http://play.tirrin.com:8080/craftalyst",
				SyncConfigs = new List<string>(),
				Mods = new List<ModDescription>(),
				ConfigVersion = 0
			});
			crafter.Run();
		}
	}
}
