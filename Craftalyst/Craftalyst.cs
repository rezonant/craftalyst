using System;
using System.IO;

namespace Craftalyst {
	public class Context {
		public Context()
		{
			AppName = "Craftalyst";
			AppId = "Craftalyst";
			AppVersion = "0.0.0.0.0.1";
		}

		/// <summary>
		/// This is the Craftalyst version number.
		/// </summary>
		public const string Version = "0.0.1a6";

		/// <summary>
		/// The name of the application, without any spaces or special characters.
		/// This identifier is used for your application's data folder, or whenever else a 
		/// clean identifier is needed instead of a formatted, multi-word name like AppName would be.
		/// For instance, if your launcher is called "Mega-X-Launcher (Extreme Edition)", then you would:
		/// * AppId = "MegaXLauncherExtremeEdition" 
		/// * AppName = "Mega-X-Launcher (Extreme Edition)"
		/// 
		/// </summary>
		/// <value>
		/// The app identifier.
		/// </value>
		public virtual string AppId { get; set; }

		/// <summary>
		/// The display name of the application wrapping Craftalyst, for use in 
		/// error messages or generated crash reports, etc.
		/// For instance, if your launcher is called "Mega-X-Launcher (Extreme Edition)", then you would:
		/// * AppName = "Mega-X-Launcher (Extreme Edition)"
		/// * AppId = "MegaXLauncherExtremeEdition" 
		/// </summary>
		/// <value>
		/// The display name of the application
		/// </value>
		public virtual string AppName { get; set; }

		/// <summary>
		/// Gets or sets the application version number. 
		/// </summary>
		/// <value>
		/// The app version.
		/// </value>
		public virtual string AppVersion { get; set; }

		/// <summary>
		/// This name is used when identifying the launcher in crash reports, etc.
		/// By default it includes the AppName and the AppVersion. Can be changed by
		/// overriding the Craftalyst class.
		/// </summary>
		/// <value>
		/// The full name of the launcher which is currently in use
		/// </value>
		public virtual string FullName { 
			get {
				return string.Format ("{0} {1}", AppName, AppVersion);
			}
		}

		/// <summary>
		/// Gets the place Craftalyst considers the user's own application data folder. The default
		/// implementation is good for almost all cases, on Windows it's "C:\Users\liam\Application Data"
		/// and on UNIX it's "~/.config". 
		/// </summary>
		/// <value>
		/// The user app data folder.
		/// </value>
		public virtual string UserAppDataFolder { 
			get {
				return Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData);
			}
		}

		/// <summary>
		/// Gets the folder Craftalyst will use for all of it's cache data, etc. This defaults to the
		/// folder named by AppId beneath the folder named by UserAppDataFolder. For instance on Windows
		/// where AppId has not been changed by your application, that might
		/// be "C:\Users\liam\Application Data\Craftalyst"
		/// </summary>
		/// <value>
		/// The app data folder.
		/// </value>
		public virtual string AppDataFolder {
			get {
				return Path.Combine(UserAppDataFolder, AppId);
			}
		}

		public virtual string AssetsFolder { 
			get {
				return Path.Combine(AppDataFolder, "assets");
			}
		}

		public virtual string VersionsFolder {
			get {
				return Path.Combine(AppDataFolder, "versions");
			}
		}

		public virtual string LibraryFolder {
			get {
				return Path.Combine(AppDataFolder, "libraries");
			}
		}

		public virtual string InstancesFolder {
			get {
				return Path.Combine(AppDataFolder, "instances");
			}
		}

		public virtual string ModsStoreFolder {
			get {
				return Path.Combine(AppDataFolder, "mods");
			}
		}

		public Instance GetOrCreateInstance (string name, InstanceDescription description)
		{
			var instance = GetInstance(name);

			if (instance == null) 
				instance = CreateInstance(name, description);
			else
				instance.Sync();

			return instance;
		}

		public Instance GetInstance (string name)
		{
			string descFile = Path.Combine(InstancesFolder, name, "craftalyst-instance.json");

			if (!File.Exists(descFile))
				return null;

			var config = InstanceDescription.FromFile(descFile);
			var instance = new Instance(this, Path.Combine(InstancesFolder, name), config);

			return instance;
		}

		public Instance CreateInstance(string name, InstanceDescription description)
		{
			var instance = new Instance(this, Path.Combine (InstancesFolder, name), description);
			instance.Description = description;
			instance.IsNewInstance = true;

			return instance;
		}
	}
}

