using System;
using Craftalyst;
using System.IO;
using System.Threading;

namespace CraftalystLauncher
{
	public partial class InstanceSelectionDialog : Gtk.Dialog
	{
		public InstanceSelectionDialog (Context craftalyst)
		{
			this.Build ();

			Context = craftalyst;

			instanceList.AddColumn("Name",   1);
			instanceList.AddColumn("Server", 2);
			instanceList.AddColumn("ID",     3);

			RefreshInstances();
		}

		public void RefreshInstances()
		{
			var store = new Gtk.ListStore(typeof(Instance), typeof(string), typeof(string), typeof(string));

			foreach (var instance in Context.GetInstances()) {
				string host = "Unknown";
				try {
					host = new Uri(instance.Description.SyncUrl).Host;
				} catch (Exception) { }

				store.AppendValues(instance, instance.Description.Name, host, 
				                   System.IO.Path.GetFileName(instance.GameFolder));
			}

			instanceList.Model = store;
		}

		public Craftalyst.Instance SelectedInstance {
			get {
				Gtk.TreeIter iter;
				Gtk.TreeModel model;

				if (instanceList.Selection.GetSelected(out model, out iter))
					return model.GetValue(iter, 0) as Instance;

				return null;
			}
		}

		public Context Context { get; set; }		

		protected void OnConnectBtnEntered (object sender, EventArgs e)
		{
		}		

		private InstanceDescription DownloadDescription(string uri)
		{
			using (StreamReader sr = new StreamReader(Downloader.Open(uri))) {
				return InstanceDescription.Parse(sr.ReadToEnd());
			}
		}

		protected void OnConnectBtnActivated (object sender, EventArgs ev)
		{
			var dialog = new ChooseServerDialog();
			int response = dialog.Run();

			if (response == (int)Gtk.ResponseType.Cancel) {
				dialog.Destroy();
				return;
			}
	
			var serverHost = dialog.Server;
			string urlHttp80 = string.Format("http://{0}/craftalyst/instance.json", serverHost);
			string urlHttp8080 = string.Format("http://{0}:8080/craftalyst/instance.json", serverHost);
			InstanceDescription instanceDescription = null;
			
			dialog.Destroy();

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
				DedicatedLauncher.MessageBox("Failed to connect to Craftalyst server {0}", serverHost);
				return;
			}

			string localId = string.Format("{0}_{1}", serverHost, instanceDescription.Id);
			Instance instance = null;

			try {
				instance = Context.CreateInstance(localId, instanceDescription);
			} catch (Exception e) {
				DedicatedLauncher.MessageBox("Failed to create local instance {0}: "+e.Message);
				Logger.Debug(e);
			}
			
			if (instance == null) {
				DedicatedLauncher.MessageBox("Failed to create Craftalyst instance!");
				return;
			}

			var installDialog = new InstallationDialog();
			installDialog.Parent = this;
			installDialog.Present();
			var listener = new InstallationDialogStatusListener(installDialog);

			// Run install dialog as a separate thread,
			// and wait until it is done while processing UI events for it

			var thread = new Thread(delegate() {
				instance.Install(listener);
				var mc = instance.CreateMinecraft();
				mc.Setup(listener);
			});
			thread.IsBackground = true;
			thread.Name = "GameSync";
			thread.Start();
			GLib.Timeout.Add(100, delegate() {
				if (!thread.IsAlive) {
					Gtk.Application.Quit();
					return false;
				}

				return true;
			});

			DedicatedLauncher.Singleton.RunUx();
			installDialog.Destroy();

			RefreshInstances();
		}		

		protected void OnConnectBtnClicked (object sender, EventArgs e)
		{
			OnConnectBtnActivated(sender, e);
		}		

		protected void OnDeleteBtnActivated (object sender, EventArgs e)
		{
			var instance = SelectedInstance;

			if (instance == null) {
				DedicatedLauncher.MessageBox("Please select the instance you wish to delete.");
				return;
			}

			var dialog = new Gtk.MessageDialog(
				this, Gtk.DialogFlags.Modal, Gtk.MessageType.Question, Gtk.ButtonsType.YesNo, 
				"Are you sure you want to delete the instance '{0}' from {1}? This cannot be undone.", 
				instance.Name, instance.Host);

			try {
				int result = dialog.Run();

				if (result == (int)Gtk.ResponseType.No)
					return;

				instance.Delete();
			} finally {
				dialog.Destroy();
			}

			RefreshInstances();

		}		

		protected void OnDeleteBtnClicked (object sender, EventArgs e)
		{
			OnDeleteBtnActivated(sender, e);
		}







	}
}

