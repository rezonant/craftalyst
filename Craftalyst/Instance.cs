using System;
using Craftalyst;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Linq;

namespace Craftalyst
{
	/// <summary>
	/// Represents an installed Minecraft instance. 
	/// </summary>
	public class Instance {
		internal Instance (Context context, string gameFolder, InstanceDescription description)
		{
			Context = context;
			GameFolder = gameFolder;
			AssetsFolder = context.AssetsFolder;
			VersionsFolder = context.VersionsFolder;
			LibraryFolder = context.LibraryFolder;
			Description = description;
			MinecraftVersion = description.Version;
			ModsStoreFolder = context.ModsStoreFolder;

			string descriptionFile = Path.Combine (gameFolder, "craftalyst-instance.json");
			if (File.Exists (descriptionFile)) {
				using (var sr = new StreamReader(descriptionFile))
					Description = InstanceDescription.Parse (sr.ReadToEnd ());
			} else {
				Description = new InstanceDescription() {
					Name = "Minecraft",
					Description = "A game about blocks",
					SyncUrl = "",
					Version = MinecraftVersion,
					Mods = new List<ModDescription>()
				};
			}
		}

		public string Name { get; set; }
		public string MinecraftVersion { get; set; }
		public Context Context { get; set; }
		public string GameFolder { get; set; }
		public string AssetsFolder { get; set; }
		public string VersionsFolder { get; set; }
		public string LibraryFolder { get; set; }
		public string ModsStoreFolder { get; set; }
		public bool IsNewInstance { get; set; }

		public string Host {
			get {
				try {
					var uri = new Uri(Description.SyncUrl);

					return uri.Host;
				} catch (Exception) { }

				return null;
			}
		}

		public InstanceDescription Description { get; set; }

		private Minecraft GameObject { get; set; }

		public Minecraft CreateMinecraft ()
		{
			if (GameObject != null)
				return GameObject;

			var mc = new Minecraft(GameFolder, AssetsFolder, LibraryFolder, VersionsFolder);
			mc.SelectedVersion = MinecraftVersion;
			mc.UnofficialVersionsUrl = string.Format("{0}/versions", this.Description.SyncUrl);
			if (Description.StartRam != null)
				mc.StartRam = this.Description.StartRam;
			if (Description.MaxRam != null)
				mc.MaxRam = this.Description.MaxRam;
			return GameObject = mc;
		}

		public void SaveDescription ()
		{
			if (Description == null)
				throw new InvalidOperationException("Cannot save null description object");

			using (var sw = new StreamWriter(Path.Combine(GameFolder, "craftalyst-instance.json")))
				sw.Write (Description.ToJson());
		}
		
		public void Sync ()
		{
			Sync (new ConsoleStatusListener());
		}

		/// <summary>
		/// Checks what Craftalyst would do if Sync() were called.
		/// </summary>
		/// <returns>
		/// The sync.
		/// </returns>
		public SyncSummary CheckSync ()
		{
			InstanceDescription fromServer;
			InstanceDescription oldDescription = Description;

			try {
				fromServer = FetchServerInstanceDescription ();
			} catch (FailedDownloadException) {
				return null;
			}

			SyncSummary summary = new SyncSummary();

			summary.RemovedMods = oldDescription.Mods
				.Where (x => !fromServer.Mods.Any (newMod => newMod.ArtifactId == x.ArtifactId))
				.Where (x => !fromServer.Mods.Any (newMod => newMod.Id == x.Id))
				.ToList();

			summary.AddedMods = fromServer.Mods
				.Where (x => !oldDescription.Mods.Any (oldMod => oldMod.ArtifactId == x.ArtifactId))
				.Where (x => !oldDescription.Mods.Any (oldMod => oldMod.Id == x.Id))
				.ToList();

			summary.UpgradedMods = fromServer.Mods
				.Where (x => !oldDescription.Mods.Any (oldMod => oldMod.ArtifactId == x.ArtifactId))
				.Where (x => oldDescription.Mods.Any (oldMod => oldMod.Id == x.Id))
				.Select(x => new ModUpgradeSummary() {
					NewVersion = x,
					OldVersion = oldDescription.Mods.Where (oldMod => oldMod.Id == x.Id).SingleOrDefault()
					}).ToList();

			summary.NewConfiguration = oldDescription.ConfigVersion < fromServer.ConfigVersion;

			return summary;
		}

		public InstanceDescription FetchServerInstanceDescription ()
		{
			WebClient client = new WebClient ();
			InstanceDescription fromServer = null;
			string syncUrl = string.Format ("{0}/instance.json", Description.SyncUrl);

			try {
				using (Stream stream = client.OpenRead(syncUrl))
				using (StreamReader sr = new StreamReader(stream))
					fromServer = InstanceDescription.Parse (sr.ReadToEnd ());
				return fromServer;
			} catch (Exception e) {
				throw new FailedDownloadException(syncUrl, e);
			}
		}

		public void Sync (IStatusListener listener)
		{
			listener.SetTitle("Synchronizing with server");
			listener.SetStatus("Please wait...");
			listener.SetProgress(0);
			listener.Log("Retrieving server-side instance description...");

			InstanceDescription fromServer = null;
			try {
				fromServer = FetchServerInstanceDescription();
			} catch (FailedDownloadException e) {
				listener.Log ("An error occurred fetching server-side instance description: {0}", e.Message);
				listener.Log ("Exception: {0}", e);
				return;
			}

			if (fromServer == null) {
				listener.Log ("Sync failed: Could not retrieve server-side instance description");
				return;
			}
			
			if (Description.StartRam == null)
				Description.StartRam = fromServer.StartRam;

			if (Description.MaxRam == null)
				Description.StartRam = fromServer.MaxRam;
			
			listener.SetStatus("Received server-side instance description...");
			listener.Log("Successfully retrieved server-side instance description");
			listener.Log("Checking for changes to instance configuration...");

			var oldDescription = Description;
			Description = fromServer;

			// Query for mods which have been removed
			
			listener.Log ("Checking for removed mods...");
			listener.SetStatus("Checking for removed mods...");
			var removedMods = oldDescription.Mods
					.Where (x => !Description.Mods.Any (newMod => newMod.ArtifactId == x.ArtifactId));

			
			// Query for mods which have been added

			listener.Log ("Checking for newly installed mods...");
			listener.SetStatus("Checking for newly installed mods...");

			var addedMods = Description.Mods.Where (x => !oldDescription.Mods.Any (oldMod => oldMod.ArtifactId == x.ArtifactId));

			int count = 0;
			int total = removedMods.Count () + addedMods.Count ();

			foreach (var removedMod in removedMods) {
				listener.SetStatus(string.Format ("Removing {0} version {1}", removedMod.Name, removedMod.Version));
				listener.Log ("Server removed mod {0}: removing...", removedMod.ArtifactId);
				new Mod (removedMod).Remove (this.GameFolder);
				
				listener.SetProgress((double)count / (double)total);
				++count;
			}

			foreach (var addedMod in addedMods) {
				listener.SetStatus(string.Format ("Installing {0} version {1}", addedMod.Name, addedMod.Version));
				listener.Log ("Server added mod {0}: installing...", addedMod.ArtifactId);
				new Mod (addedMod).Install (this.ModsStoreFolder, this.GameFolder, listener);

				listener.SetProgress((double)count / (double)total);
				++count;
			}

			// Download new configuration files...

			listener.Log ("Checking configuration version..");
			listener.Log ("Local: {0}       Remote: {1}", oldDescription.ConfigVersion, Description.ConfigVersion);

			if (oldDescription.ConfigVersion < Description.ConfigVersion) {
				listener.Log (" * Need to update configuration from server...");
				UpdateConfigFiles (listener); // A change is needed...
			} else {
				listener.Log(" - Up to date");
			}

			listener.SetStatus("Writing updated instance description...");
			using (var sw = new StreamWriter(Path.Combine (GameFolder, "craftalyst-instance.json")))
				sw.Write(Description.ToJson());
			
			SaveDescription();

			listener.SetProgress(1);
		}

		public void Delete ()
		{
			Directory.Delete(GameFolder, true);
		}

		public void InstallIfNecessary()
		{
			if (IsNewInstance)
				Install ();
		}

		public void Install ()
		{
			Install (new ConsoleStatusListener());
		}

		public void Install (IStatusListener listener)
		{
			Directory.CreateDirectory(GameFolder);
			Directory.CreateDirectory(AssetsFolder);
			Directory.CreateDirectory(VersionsFolder);
			Directory.CreateDirectory(LibraryFolder);
			
			listener.Log("");
			listener.Log("");
			listener.Log("Installing Minecraft...");

			var mc = CreateMinecraft();
			mc.Install(listener);

			// Install mods...
			
			listener.Log("");
			listener.Log("");
			listener.Log("Installing mods...");
			listener.SetTitle("Installing mods...");

			int count = 0;
			int total = Description.Mods.Count ();

			foreach (var addedMod in Description.Mods) {
				listener.Log("Installing mod {0}", addedMod.ArtifactId);
				listener.SetStatus(string.Format ("Installing mod {0} {1}...", addedMod.Name, addedMod.Version));
				new Mod(addedMod).Install(this.ModsStoreFolder, this.GameFolder, listener);
				listener.SetProgress((double)count / (double)total);
			}

			listener.Log("Saving instance description...");
			listener.SetTitle("Saving instance...");
			listener.SetStatus("Just a bit longer!");

			SaveDescription();

			// Install configuration files...
			UpdateConfigFiles(listener);

			listener.Log("Performing synchronization with server...");
			// Update to latest from server
			Sync (listener);

			listener.SetTitle("All is done!");
			listener.SetProgress(1);
		}		

		void UpdateConfigFiles (IStatusListener listener)
		{
			listener.SetTitle("Syncing client configuration from server...");
			listener.Log("Updating configuration files from the server...");
			listener.SetStatus("Please wait...");
			listener.SetProgress(0);

			List<string> configFiles = new List<string>();

			foreach (var list in Description.Mods.Select (x => x.ConfigFiles).Where (x => x != null))
				configFiles.AddRange(list);

			configFiles.AddRange(Description.SyncConfigs);

			int total = configFiles.Count();
			int count = 0;

			foreach (string configFile in configFiles) {
				string localFile = Path.Combine(GameFolder, "config", configFile);
				if (File.Exists(localFile))
					File.Delete(localFile);

				listener.SetStatus(string.Format ("Pulling {0}", configFile));
				Downloader.Single(string.Format("{0}/config/{1}", Description.SyncUrl, configFile), localFile);

				listener.SetProgress((double)count / (double)total);
				++count;
			}
			listener.SetProgress(1);
		}


	}
}
