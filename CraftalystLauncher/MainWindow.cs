using System;
using Gtk;
using Craftalyst;
using System.Collections.Generic;
using System.Linq;
using CraftalystLauncher;
using System.Threading;
using System.Diagnostics;
using System.IO;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow (Instance instance, MinecraftSession session): base (Gtk.WindowType.Toplevel)
	{
		Instance = instance;
		Session = session;
		Build ();

		titleLabel.Text = instance.Description.Name;

		if (session != null)
			usernameLabel.Markup = string.Format ("Logged in as <b>{0}</b>", Session.Username);
		else {
			var saved = GetSavedCredentials();
			if (saved == null) {
				usernameLabel.Markup = string.Format ("Click Play to Login");
			} else {
				usernameLabel.Markup = string.Format ("Welcome back <b>{0}</b>!", saved.AutoLoginUser);
			}
		}

		var syncSummary = instance.CheckSync();

		List<string> items = new List<string>();
		
		items.AddRange(syncSummary.UpgradedMods.Select(
			mod => string.Format ("Upgraded mod: {0} {1}", mod.NewVersion.Name, mod.NewVersion.Version)
		));
		items.AddRange(syncSummary.AddedMods.Select(
			mod => string.Format ("New mod: {0} {1}", mod.Name, mod.Version)
		));
		items.AddRange(syncSummary.RemovedMods.Select(
			mod => string.Format ("Removed mod: {0}", mod.Name)
		));

		if (syncSummary.NewConfiguration)
			items.Add("New configuration to download.");

		InstanceName = instance.Description.Name;
		Description = instance.Description.Description;

		var newDesc = Instance.FetchServerInstanceDescription();
		serverNews.Buffer.Text = newDesc.MessageOfTheDay;
		
		//playButton.ImagePosition = PositionType.Left;

		if (items.Count > 0) {
			UpdateSummary = string.Join("\n", items.Select (x => " * "+x));
			playButton.Image = Image.NewFromIconName(Stock.GoUp, IconSize.Button);
			playButton.Label = "Update to Play!";
		} else {
			UpdateSummary = "None, ready to play!";
			playButton.Hide();
			playButton.Unrealize();
			playButton.Label = "Play!";
			playButton.Image = Image.NewFromIconName(Stock.Apply, IconSize.Button);
			playButton.Show();
		}
	}

	public MinecraftSession Session { get; set; }
	public Instance Instance { get; set; }
	public Process GameProcess { get; set; }

	string description = "This is a Craftalyst instance.";
	string name = "Craftalyst";

	public string Description { 
		get {
			return description;
		} set {
			description = value;
			UpdateOverview();
		}
	}

	public string InstanceName {
		get {
			return name;
		} set {
			name = value;
			titleLabel.Text = value;
			UpdateOverview();
		}
	}

	string updateSummary = "Checking for updates...";

	public string UpdateSummary {
		get {
			return updateSummary;
		}
		set {
			updateSummary = value;
			UpdateOverview();
		}
	}

	bool gameIsActive = false;

	public bool GameIsActive {
		get {
			return gameIsActive;
		} set {
			if (value == gameIsActive)
				return;

			if (value) {
				playButton.Label = "Kill Minecraft";
			} else {
				playButton.Label = "Play";
			}

			gameIsActive = value;
		}
	}

	private void UpdateOverview ()
	{
		overviewAndStatus.Buffer.Text = string.Format (
			"You are playing on {0}.\n" +
			"\n"+
			"{1}\n"+
			"\n" +
			"Changes required to play:\n{2}",

			InstanceName,
			Description,
			UpdateSummary
		);
	}

	public void Log (string str)
	{
		//var iter = minecraftLog.Buffer.EndIter;
		//minecraftLog.Buffer.Insert(ref iter, str);
		
		//minecraftLog.Buffer.Text += str + "\n";
		var iter = minecraftLog.Buffer.EndIter;
		minecraftLog.Buffer.Insert(ref iter, string.Format("{0}\n", str));

		minecraftLog.ScrollToIter(minecraftLog.Buffer.EndIter, 0, false, 0, 0);
	}

	public void GameEnded (int exitCode)
	{
		this.Log("");
		this.Log(string.Format("Minecraft has exited ({0})", exitCode));

		this.GameIsActive = false;
		this.minecraftStatus.Text = string.Format("Minecraft has stopped ({0})", exitCode);
	}
	
	protected void OnShown (object sender, EventArgs a)
	{
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		a.RetVal = true;
		Destroy();
		Application.Quit ();
	}

	protected void OnPlayButtonActivated (object sender, EventArgs e)
	{
		if (GameIsActive)
			KillMinecraft();
		else
			Play();
	}	

	void KillMinecraft ()
	{
		GameProcess.Kill();
	}

	public void Play()
	{
		try {
			if (Session == null)
				Login();
		} catch (CancelException) {
			return;
		}

		Launch();
	}

	string SavedCredentialsFile
	{
		get {
			return System.IO.Path.Combine (Instance.GameFolder, "credentials.json");
		}
	}

	public SavedCredentials GetSavedCredentials ()
	{
		string credsFileName = SavedCredentialsFile;

		if (!File.Exists (credsFileName))
			return null;

		using (var sr = new StreamReader (credsFileName))
			return SavedCredentials.Parse (sr.ReadToEnd ());
	}

	public void Login()
	{
		LoginDialog login = new LoginDialog ();
		var saved = GetSavedCredentials();

		try {
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
						DedicatedLauncher.MessageBox (message);
						continue;
					}

					break;
				} else if (response == (int)Gtk.ResponseType.Cancel) {
					throw new CancelException();
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

			Directory.CreateDirectory(System.IO.Path.GetDirectoryName(SavedCredentialsFile));
			using (StreamWriter sw = new StreamWriter(SavedCredentialsFile))
				sw.Write(saved.ToJson());

			login.Hide ();
			login.Destroy();
			
			usernameLabel.Markup = string.Format ("Playing as <b>{0}</b>", Session.Username);
		} finally {
			login.Destroy();
		}
	}

	public void Launch()
	{
		playButton.Sensitive = false;
		var monitor = new MainWindowGameMonitor(this);

		var installDialog = new InstallationDialog();
		installDialog.Parent = this;
		installDialog.Show();
		installDialog.GdkWindow.Raise();

		// Run syncing in installation dialog as a separate thread,
		// and wait until it is done while processing UI events for it

		var thread = new Thread(delegate() {
			var listener = new InstallationDialogStatusListener(installDialog);
			Instance.Sync(listener);
			var mc = Instance.CreateMinecraft();
			mc.Setup(listener);
		});
		thread.IsBackground = true;
		thread.Name = "GameSync";
		thread.Start();
		GLib.Timeout.Add(100, delegate() {
			if (!thread.IsAlive) {
				Application.Quit();
				return false;
			}

			return true;
		});
		Gtk.Application.Run();

		// Run the game

		installDialog.Log ("");
		installDialog.Log ("Getting ready to start Minecraft!");
		installDialog.Status = "All updates completed!";
		installDialog.Title = "All updates completed!";
		installDialog.Destroy ();
		
		monitor.OutputLine(GameMessageType.Output, "");
		monitor.OutputLine(GameMessageType.Output, "Starting Minecraft...");

		playButton.Sensitive = true;
		GameIsActive = true;
		thread = new Thread(delegate() {
			var mc = Instance.CreateMinecraft();
			GameProcess = mc.Start(Session, monitor);

			DedicatedLauncher.Singleton.EnqueueUxJob(delegate() {
				this.minecraftStatus.Text = string.Format("Minecraft is running (Process #{0})", GameProcess.Id);
			});

			Console.WriteLine("Game launched from seperate thread!");
		});
		thread.IsBackground = true;
		thread.Name = "GameLauncher";
		thread.Start();

		Console.WriteLine("Control returned.");
	}

	protected void OnTsButtonActivated (object sender, EventArgs e)
	{
		DedicatedLauncher.MessageBox("WTF");
	}	

	protected void OnMapButtonActivated (object sender, EventArgs e)
	{
		Process.Start("http://tirrin.com/map");
	}	

	protected void OnWikiButtonActivated (object sender, EventArgs e)
	{
		Process.Start("http://tirrin.com/wiki");
	}	

	protected void OnForumsButtonActivated (object sender, EventArgs e)
	{
		Process.Start("http://tirrin.com/forums");
	}	

	protected void OnWebsiteButtonActivated (object sender, EventArgs e)
	{
		Process.Start("http://tirrin.com/");
	}	

	protected void OnDonateButtonActivated (object sender, EventArgs e)
	{
		Process.Start("http://tirrin.com/donate");
	}	

	protected void OnReportServerDownButtonActivated (object sender, EventArgs e)
	{
		Process.Start("http://tirrin.com/reportServerDown");
	}








}
