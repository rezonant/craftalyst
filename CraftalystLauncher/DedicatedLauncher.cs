using System;
using Gtk;
using Craftalyst;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using System.IO;
using System.Linq;

namespace CraftalystLauncher
{
	public class DedicatedLauncher
	{
		public DedicatedLauncher (string instanceName, InstanceDescription defaultInstanceDescription)
		{
			Craft = new Context ();
			Craft.AppName = "Craftalyst Launcher";
			Craft.AppVersion = "0.1.0a9";
			
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
			SavedCredentials saved = null;
			string credsFileName = Path.Combine(Instance.GameFolder, "credentials.json");

			if (File.Exists(credsFileName)) {
				using (var sr = new StreamReader(credsFileName))
					saved = SavedCredentials.Parse(sr.ReadToEnd());
			}

			if (saved != null) {
				login.Username = saved.AutoLoginUser;

				var savedCreds = saved.Credentials.Where(x => x.UserName == saved.AutoLoginUser).FirstOrDefault();
				if (savedCreds != null) {
					login.Password = savedCreds.Password;
					login.RememberCredentials = true;
				}
			}

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
				} else if (response == (int)Gtk.ResponseType.Cancel) {
					Gtk.Application.Quit();
					Environment.Exit(0);
				}
			}

			saved = new SavedCredentials() {
				AutoLoginUser = login.Username,
				Credentials = new List<SavedCredential>()
			};

			if (login.RememberCredentials) {
				saved.Credentials = new List<SavedCredential>() {
					new SavedCredential() {
						UserName = login.Username,
						Password = login.Password
					}
				};
			}

			using (StreamWriter sw = new StreamWriter(credsFileName))
				sw.Write(saved.ToJson());

			login.Hide ();
			login.Destroy();
		}

		static DedicatedLauncher singleton = null;

		public static DedicatedLauncher Singleton {
			get {
				return singleton;
			}
		}

		private EventWaitHandle uxJobWait = null;

		public void EnqueueUxJob (System.Action a)
		{

			if (uxJobWait == null)
				uxJobWait = new EventWaitHandle(false, EventResetMode.AutoReset);

			lock (this.jobQueue)
				this.jobQueue.Enqueue(a);

			while (true) {
				int count = 0;

				lock (jobQueue)
					count = jobQueue.Count;

				if (count > 100)
					Thread.Sleep(25);
				else
					break;
			}

			//uxJobWait.Set();
		}

		private Queue<System.Action> jobQueue = new Queue<System.Action>();

		public void Run ()
		{
			Thread.CurrentThread.Name = "UX";

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

			Console.WriteLine("Registering GTK event sweeper");

			delly = delegate () {
				try {
					//Gdk.Threads.Enter();
					lock (jobQueue) {
						System.Action job = null;
						int i = 0, max = 100;
						while (i < max && jobQueue.Count > 0 && (job = jobQueue.Dequeue()) != null) {
							job();
							++i;
						}
					}
					//Gdk.Threads.Leave();

				} catch (Exception exc) {
					Console.WriteLine (exc);
				}

				Gtk.Timeout.Add (10, delly);
				return false;
			};

			Gtk.Timeout.Add (10, delly);

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
