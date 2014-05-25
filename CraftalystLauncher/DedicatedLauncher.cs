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
		public DedicatedLauncher (string instanceName, InstanceDescription defaultInstanceDescription):
			this()
		{
			var instance = Craft.GetInstance (instanceName);

			if (instance == null)
				instance = Craft.CreateInstance(instanceName, defaultInstanceDescription);
		}

		public DedicatedLauncher ()
		{
			Craft = new Context ();
			Craft.AppName = "Craftalyst Launcher";
			Craft.AppVersion = "0.6.0";
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

			Directory.CreateDirectory(Path.GetDirectoryName(credsFileName));
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

		private InstanceDescription DownloadDescription(string uri)
		{
			using (StreamReader sr = new StreamReader(Downloader.Open(uri))) {
				return InstanceDescription.Parse(sr.ReadToEnd());
			}
		}

		public void RunInstance()
		{	

			//Login ();

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

			RunUx();
		}

		public void RunUx()
		{
			Gtk.Timeout.Add (10, delegate () {
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
					return false;
				}

				return true;
			});

			Gtk.Application.Run ();
		}

		public void Run()
		{
			Thread.CurrentThread.Name = "UX";

			try {
				while (true) {
					RunNew();
				}
			} catch (CancelException) { }
		}

		public void RunNew ()
		{
			var instances = Craft.GetInstances();

			if (instances.Count == 0) {
				var dialog = new ChooseServerDialog();

				InstanceDescription instanceDescription = null;

				while (true) {
					var dialogResult = dialog.Run();
					if (dialogResult == (int)Gtk.ResponseType.Cancel)
						throw new CancelException();

					var serverHost = dialog.Server;
					string urlHttp80 = string.Format("http://{0}/craftalyst/instance.json", serverHost);
					string urlHttp8080 = string.Format("http://{0}:8080/craftalyst/instance.json", serverHost);

					try {
						instanceDescription = DownloadDescription(urlHttp80);
					} catch (Exception) {
						Logger.Debug("Failed to retrieve URL '{0}', this might not be an error.", urlHttp80);
					}

					if (instanceDescription == null) {
						try {
							instanceDescription = DownloadDescription(urlHttp8080);
						} catch (Exception) {
							Logger.Debug("Failed to retrieve URL '{0}'", urlHttp8080);
						}
					}

					if (instanceDescription == null) {
						MessageBox("Failed to connect to Craftalyst server {0}", serverHost);
						continue;
					}

					string localId = string.Format("{0}_{1}", serverHost, instanceDescription.Id);
					try {
						Instance = Craft.CreateInstance(localId, instanceDescription);
					} catch (Exception e) {
						MessageBox("Failed to create local instance {0}: "+e.Message);
						Logger.Debug(e);
					}
					
					if (Instance == null) {
						MessageBox("Failed to create Craftalyst instance!");

						return;
					}

					break;
				}

				dialog.Destroy();
			} else {
				var dialog = new InstanceSelectionDialog(Craft);

				while (true) {
					var result = dialog.Run();
					
					if (result == (int)Gtk.ResponseType.Cancel)
						throw new CancelException();

					Instance = dialog.SelectedInstance;

					if (Instance == null) {
						MessageBox("Please select an instance to launch.");
						continue;
					}

					break;
				}

				dialog.Destroy();
			}

			RunInstance();
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

			var crafter = new DedicatedLauncher();
			crafter.Run();
		}
	}
}
