using System;
using Gtk;
using Craftalyst;
using System.Collections.Generic;
using System.Linq;
using CraftalystLauncher;
using System.Threading;
using System.Diagnostics;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow (Instance instance, MinecraftSession session): base (Gtk.WindowType.Toplevel)
	{
		Instance = instance;
		Session = session;
		Build ();

		titleLabel.Text = instance.Name;
		usernameLabel.Markup = string.Format ("Playing as <b>{0}</b>", session.Username);

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

		if (items.Count > 0)
			UpdateSummary = string.Join("\n", items.Select (x => " * "+x));
		else
			UpdateSummary = "None, ready to play!";

		InstanceName = instance.Description.Name;
		Description = instance.Description.Description;

		var newDesc = Instance.FetchServerInstanceDescription();
		serverNews.Buffer.Text = newDesc.MessageOfTheDay;
	}
	
	public MinecraftSession Session { get; set; }
	public Instance Instance { get; set; }

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
		
		minecraftLog.Buffer.Text += str + "\n";
		minecraftLog.ScrollToIter(minecraftLog.Buffer.EndIter, 0, false, 0, 0);
	}

	public void GameEnded ()
	{
		// TODO
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnPlayButtonActivated (object sender, EventArgs e)
	{
		var installDialog = new InstallationDialog();
		installDialog.ShowAll();
		Instance.Sync(new InstallationDialogStatusListener(installDialog));

		installDialog.Log ("");
		installDialog.Log ("Getting ready to start Minecraft!");
		installDialog.Status = "All updates completed!";
		installDialog.Title = "All updates completed!";
		installDialog.Destroy ();

		var thread = new Thread(delegate() {
			var mc = Instance.CreateMinecraft();
			mc.Start(Session, new MainWindowGameMonitor(this));
			Console.WriteLine("Game launched from seperate thread!");
		});

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
