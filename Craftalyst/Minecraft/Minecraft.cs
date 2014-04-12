using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Xml.Linq;
using System.Security.AccessControl;
using System.Threading;

namespace Craftalyst
{
    /// <summary>
    /// A convenience facade for the various Minecraft APIs included within Craftalyst.
    /// </summary>
    public class Minecraft
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="Craftalyst.Minecraft"/> class.
		/// </summary>
		/// <param name='craftalyst'>
		/// The Craftalyst context. 
		/// </param>
		/// <param name='gameLocation'>
		/// The game folder to operate within. It will be created if it does not exist.
		/// </param>
		public Minecraft(string gameLocation, string assetLocation, string libraryLocation, string versionsLocation)
		{
			GameLocation = gameLocation;
			AssetsLocation = assetLocation;
			LibraryLocation = libraryLocation;
			VersionsLocation = versionsLocation;
			
			if (!Directory.Exists(GameLocation))
				Directory.CreateDirectory(GameLocation);

			if (!Directory.Exists(AssetsLocation))
				Directory.CreateDirectory(AssetsLocation);

			Save = "";
			OnlineMode = true;
			AutoSelect = true;
			JavaFile = "java";
			StartRam = "256";
			MaxRam = "2048";
		}

		public const string MinecraftLibraryUrl = "https://libraries.minecraft.net";
		public const string MinecraftVersionsUrl = "http://s3.amazonaws.com/Minecraft.Download/versions";
		public const string MinecraftAssetUrl = "http://resources.download.minecraft.net";
		public const string MinecraftManifestUrl = "http://s3.amazonaws.com/Minecraft.Download/indexes";

        internal static Dictionary<string, string[]> versionList = new Dictionary<string, string[]>
        {
            //{"AL_LatestID", new string[] { version.latest.release, version.latest.snapshot }},
            //{"1.6.4",       new string[] { "time", "releaseTime", "Type" }}
        };

        internal static Dictionary<string, string[]> versionData = new Dictionary<string, string[]>
        {
            //{"id"                  , new string[] { "1.6.4" }},
            //{"time"                , new string[] { "2013-09-19T10:52:37-05:00" }},
            //{"releaseTime"         , new string[] { "2013-09-19T10:52:37-05:00" }},
            //{"Type"                , new string[] { "release" }},
            //{"minecraftArguments"  , new string[] { "--username ${auth_player_name} --session ${auth_session} --version ${version_name} --gameDir ${game_directory} --assetsDir ${game_assets} --uuid ${auth_uuid} --accessToken ${auth_access_token}" }},
            //{"mainClass"           , new string[] { "net.minecraft.client.main.Main" }},
            //{"libraries"           , new string[] { "net\sf\jopt-simple\jopt-simple\4.5\jopt-simple-4.5.jar" "etc" "etc" }},
            //{"natives"             , new string[] { "net\sf\jopt-simple\jopt-simple\4.5\jopt-simple-4.5.jar" "etc" "etc" }},
        };

		/// <summary>
		/// The folder where Minecraft will be installed into / launched from.
		/// </summary>
		/// <value>
		/// A folder path
		/// </value>
		public string GameLocation { get; set; }

		/// <summary>
		/// The folder where assets will be saved into. This is NEW launcher (1.6+) style,
		/// with objects/indexes folders beneath. You do not have to populate the directory 
		/// at all, Craftalyst will handle retrieving any needed resources from Minecraft
		/// servers.
		/// </summary>
		/// <value>
		/// A folder path
		/// </value>
		public string AssetsLocation { get; set; }

		/// <summary>
		/// The folder where library resources will be saved. This is typical Maven repository
		/// heirarchy.
		/// </summary>
		/// <value>
		/// A folder path
		/// </value>
		public string LibraryLocation { get; set; }

		/// <summary>
		/// The folder in which the game JARs, each of a particular version of Minecraft, will be stored
		/// and accessed from.
		/// </summary>
		/// <value>
		/// A folder path.
		/// </value>
		public string VersionsLocation { get; set; }

		/// <summary>
		/// When Minecraft.net reports that it does not have a particular version manifest available for 
		/// the given version string, where do we look for that version?
		/// 
		/// While Mojang possibly might not appreciate the ping and subsequent 404 every time someone installs
		/// a Forge-modded installation of Minecraft when it comes to bandwidth, I have to assume they appreciate 
		/// the knowledge that a forge-loaded version of Minecraft is, in fact, being installed. What they see
		/// when you install a version of 1.6.4 that is modded with Forge while using Craftalyst is:
		/// GET /versions/1.6.4-forge/1.6.4-forge.json HTTP/1.1
		/// To which their server responds:
		/// HTTP/1.1 404 Not Found
		/// 
		/// Mojang can search for this and correlate numbers on how many Forge installs are happening via Craftalyst,
		/// which I think is a nice touch on my end for them, since those might be valuable business metrics, at least
		/// I hope :-)
		/// 
		/// When this happens, Craftalyst will assume that the version being requested is an UNOFFICIAL version.
		/// At this point, the same version manifest request is repeated, but this time heading to the URL specified
		/// in UnofficialVersionsUrl. By default, this will be craftalyst.org's servers, which will faithfully report
		/// any Forge-modded Minecraft version for which Forge has released a version.json. The reported version manifest
		/// is modified only in 2 ways, both related to the "minecraftforge" library entry. First, the library name is 
		/// changed from "minecraftforge" to "forge" to match what's on files.minecraftforge.net, and second the 
		/// version number reported in Forge's version.json is prefixed with the Minecraft version in question. Finally,
		/// as a totally Forge-specific hack that does not happen for normal Minecraft libraries, Craftalyst will append
		/// "-universal" to the end of the URL filename when Forge is detected as the library being examined. For instance,
		/// Forge 9.11.1.965 running on Minecraft 1.6.4 becomes:
		/// net.minecraftforge:forge:1.6.4-9.11.1.965
		/// And since Forge already gives "http://files.minecraftforge.net/maven/" as the base Maven URL in its Minecraft 
		/// library entry, the URL thus becomes:
		/// http://files.minecraftforge.net/maven/net/minecraftforge/forge/1.6.4-9.11.1.965/forge-1.6.4-9.11.1.965-universal.jar
		/// 
		/// Other than these additions, and the fact that we request org.scala-lang packages from the official Apache-hosted 
		/// Maven repository, all other resource fetching is identical to the standard Minecraft launcher.
		/// 
		/// </summary>
		/// <value>
		/// The unofficial versions URL.
		/// </value>
		public string UnofficialVersionsUrl { get; set; }

		public string Save { get; set; }
		public string StartRam { get; set; }
        public string MaxRam { get; set; }
        public bool DisplayCMD { get; set; }
        public string CPUPriority  { get; set; }
        public bool OnlineMode  { get; set; }
        public string OfflineName { get; set; }
        public string SelectedVersion { get; set; }
        public bool AutoSelect { get; set; }
		public bool UseNightly { get; set; }
        public bool Force64Bit { get; set; }
		public string JavaFile { get; set; }
        public string BuildArguments { get; set; }
		public bool UseLatest { get; set; }
		public MinecraftVersionParameters VersionParameters { get; private set; }

		public event StatusHandler OnStatusChanged;

		protected void ChangeStatus(string text)
		{
			OnStatusChanged(text);
		}

		public string Username { get; set; }

		public MinecraftAccountType AccountType { 
			get {
				if (Username == null)
					return MinecraftAccountType.Legacy;

				return Username.Contains("@")? MinecraftAccountType.Mojang : MinecraftAccountType.Legacy;
			}
		}

        /// <summary>
        /// Run this method to download, proccess, and open minecraft.
        /// </summary>
        /// <param name="username">Input a username here. Example: "username"</param>
        /// <param name="password">Input a password here. Example: "pass1234"</param>
        /// <returns></returns>
        public void Start (string username = "", string password = "")
		{
			Username = username;
			var session = Authenticate (username, password);
			if (session == null)
				throw new Exception("Failed to authenticate");
			Start (session);
        }
		
        public void Start (MinecraftSession authSession)
		{
			Start(authSession, new ConsoleGameProcessMonitor());
		}

		private ThreadStart GenerateGameMessagePumper(Stream input, GameMessageType type, Queue<GameMessage> queue)
		{
			return delegate(object state) {
				using (StreamReader sr = new StreamReader(input)) {
					string line = null;
					while ((line = sr.ReadLine()) != null) {
						lock (queue)
							queue.Enqueue(new GameMessage() {
								Type = type,
								Line = line
							});
					}
				}
			};
		}

        /// <summary>
        /// Starts Minecraft. This method will call Setup() to ensure that all needed assets, libraries, and 
		/// the needed game version are downloaded into the various cache directories, and installed into 
		/// the folder listed at GameFolder. If you are using a verison of Minecraft prior to 1.7.4, we will
		/// verify the contents of assets/legacy/virtual are exactly the same as those listed in the 
		/// Minecraft asset manifest associated to the version being launched. Any mismatches will be replaced
		/// from the asset cache.
        /// </summary>
        /// <param name="authSession">
		/// The Minecraft authentication session to use while launching the game. To obtain one of these,
		/// construct a MinecraftAuthentication instance and call the Login() method, giving a valid Minecraft
		/// username and password.
		/// </param>
        /// <param name="monitor">
		/// Receives events related to the Minecraft game process. Events will be received on a foreign thread,
		/// You as the caller MUST ensure that you perform any operations within the correct thread. Many user 
		/// interface frameworks are single-threaded by default, and thus require you to pass a message to the 
		/// UI thread from the game monitor thread which calls your IGameMonitor instance. Please do not report
		/// problems to us or your UI framework because you aren't doing threading correctly. We cannot stress
		/// enough to learn the basics of the C# lock() statement and also System.Collections.Generic.Queue<T>. 
		/// </param>
        /// <returns></returns>
        public void Start (MinecraftSession authSession, IGameProcessMonitor monitor)
		{
			Username = authSession.OriginalUsername;
            Setup();
            GetNatives();

			// Run Minecraft, with a pair of threads for pumping messages out of StandardError/Output and into
			// a queue, which we pick up with a third "mediator" thread, which handles calling the IGameMonitor passed 
			// by the caller.

            var proc = RunCommand(authSession);
			Queue<GameMessage> messages = new Queue<GameMessage>();
			var stdErrThread = new Thread(GenerateGameMessagePumper(proc.StandardError, GameMessageType.Error, messages));
			var stdOutThread = new Thread(GenerateGameMessagePumper(proc.StandardOutput, GameMessageType.Output, messages));

			var monitorThread = new Thread(delegate (object state) {
				while (true) {
					if (proc.HasExited && stdErrThread.IsAlive && stdOutThread.IsAlive) {
						Console.WriteLine("Minecraft appears to have exited ({0}) and all thread activity is completed.", proc.ExitCode);
						break;
					}

					lock (messages) {
						if (messages.Count == 0)
							continue;

						// Call the monitor to report this output line. 
						// Note that this occurs in OUR thread. You, as the receiver of this
						// event, need to make sure you perform operations therein in the proper
						// thread. Learn to use Queues and always lock em.
						
						var msg = messages.Dequeue();
						monitor.OutputLine(msg.Type, msg.Line);
					}

					Thread.Sleep(0);
				}

				// Game has ended
				monitor.GameEnded(proc.ExitCode);
			});

			Console.WriteLine("Starting monitor threads...");
			stdErrThread.Start();
			stdOutThread.Start();
			monitorThread.Start();

			Console.WriteLine("Game has started, returning control to UI...");
        }

        /// <summary>
        /// Login User, Use the user credentials to login and retrieve the session ID.
        /// </summary>
        /// <param name="username">Input username here. Example: "username"</param>
        /// <param name="password">Input password here. Example: "pass1234"</param>
        /// <returns>Status of exceptions or success</returns>
        private MinecraftSession Authenticate (string inputUsername = "", string inputPassword = "")
		{
			if (!OnlineMode) {
				return new MinecraftSession () {
					Username = OfflineName
				};
			}

			var session = new MinecraftAuthentication ().Login (inputUsername, inputPassword);

			if (session != null)
				session.OriginalUsername = inputUsername;

			return session;
        }

        /// <summary>
        /// Save the login data.
        /// </summary>
        /// <param name="username">Input the users username</param>
        /// <param name="password">Input the users password</param>
        /// <param name="propperUsername">Input the users propper username</param>
        /// <param name="saveLogin">Input if the user prompted to save Login.</param>
        /// <param name="autoLogin">Input if the user prompted to have this user automatically Login.</param>
        /// <returns>Status of exceptions or success</returns>
        private static string setSaveData(string username, string password, string propperUsername, bool saveLogin = false, bool autoLogin = false)
        {
            string status = "Successful";

            //atomFileData.saveConfFile(atomFileData.configFile, atomFileData.config);
        
            return status;
        }

        /// <summary>
        /// Look for & Load JSON parameters 
        /// </summary>
        /// <param name="nightlyBuilds">Whether or not to use the latest builds for the libraries.</param>
        /// <returns>Status of exceptions or success</returns>
        private string GetVersionParam(bool nightlyBuilds = false) {
            string subString = "";
            string status = "Successful";
            string fileName = GameLocation + @"\versions\" + SelectedVersion + @"\" + SelectedVersion + ".json";

            if ((DateTime.Now - File.GetLastWriteTime(fileName)).TotalHours > 1) {
                if (OnlineMode) {
                    try {
                        Downloader.Single("http://s3.amazonaws.com/Minecraft.Download/versions/" + SelectedVersion + "/" + SelectedVersion + ".json", fileName);
                    } catch (Exception ex) {
                        subString = ex.Message;
                    }
                } else {
                    subString = "Offline Mode, File Missing. You need to login and download first, before offline mode can be used.";
                }
            }

            if (File.Exists(fileName)) {
                versionData = MinecraftVersionList.GetVersionData(fileName, nightlyBuilds);
            } else {
                status = subString + " / Version data file missing.";
            }

            return status;
        }

		public MinecraftVersionParameters GetVersionParameters (string id)
		{
			string url = string.Format ("{0}/{1}/{1}.json", Minecraft.MinecraftVersionsUrl, id);
			string filePath = Path.Combine (this.VersionsLocation, id, string.Format ("{0}.json", id));

			Directory.CreateDirectory (Path.GetDirectoryName (filePath));

			if (!File.Exists (filePath)) {
				try {
					Downloader.Single (url, filePath);
				} catch (WebException) {
					Downloader.Single (string.Format ("{0}/{1}/{1}.json", this.UnofficialVersionsUrl, id), filePath);
				}
			}

			using (StreamReader sr = new StreamReader(filePath))
				return MinecraftVersionParameters.Parse(sr.ReadToEnd ());
		}

		public MinecraftVersionList GetVersionList()
		{
			throw new NotImplementedException();
		}

		
		private void InstallAssets () 
		{
			InstallAssets(new ConsoleStatusListener());
		}

		private void InstallAssets (IStatusListener listener)
		{
			listener.SetTitle("Installing Minecraft assets...");
			listener.SetProgress(0);

			listener.Log ("Installing assets...");

			string indexName = VersionParameters.Assets;

			if (string.IsNullOrEmpty (indexName))
				indexName = "legacy";
			string indexesDir = Path.Combine(AssetsLocation, "indexes");

			Directory.CreateDirectory(indexesDir);
			string manifestFile = string.Format ("{0}/{1}.json", indexesDir, indexName);

			if (!File.Exists (manifestFile)) {
				listener.Log ("Retrieving asset manifest file {0}/{1}.json...", Minecraft.MinecraftManifestUrl, indexName);
				Downloader.Single (string.Format ("{0}/{1}.json", Minecraft.MinecraftManifestUrl, indexName), 
					manifestFile);
			} else {
				listener.Log ("Using cached manifest file assets.{0}.json...", indexName);
			}

			MinecraftAssetManifest manifest = null;
			using (StreamReader sr = new StreamReader(manifestFile))
				manifest = MinecraftAssetManifest.Parse (sr.ReadToEnd ());
			
			listener.Log ("Found {0} assets in manifest...", manifest.Objects.Count);

			var toInstall = manifest.Objects.Where (x => !x.IsInstalled(AssetsLocation));
			listener.Log ("Need to get {0} assets from Minecraft.net...", toInstall.Count ());

			int progress = 0;
			int total = toInstall.Sum (x => x.Size);
			int count = 0;
			int totalCount = toInstall.ToList().Count;

			foreach (var obj in toInstall) {
				obj.Install (manifest, AssetsLocation, listener);
				progress += obj.Size;
				count += 1;

				listener.SetProgress((double)progress / (double)total);
				listener.SetStatus(
					string.Format (
						"Installed {0} / {1} assets. Downloaded {2} / {3} MB", 
				        count, totalCount, 
						Math.Round(progress / 1024.0 / 1024.0 * 100.0) / 100.0, 
						Math.Round (total / 1024.0 / 1024.0 * 100.0) / 100.0));
			}

			listener.Log("Successfully installed all assets!");

			listener.SetProgress(1);
		}
		
		private void InstallLibraries ()
		{
			InstallLibraries(new ConsoleStatusListener());
		}

		private void InstallLibraries(IStatusListener listener)
		{
			listener.Log ("Installing libraries...");
			listener.SetProgress(0);

			var libraries = VersionParameters.Libraries.Where (x => x.AppliesHere).Where (x => x.RequiredForClient).ToList ();

			// Install missing libraries

			int progress = 0;
			int totalCount = libraries.Count ();

			foreach (var lib in libraries) {
				var libUrl = lib.Url;
				var libJar = lib.GetPath(this);
				var libDir = Path.GetDirectoryName(libJar);
				Directory.CreateDirectory(libDir);

				// Download the library if it does not already exist in the libraries directory
				if (!File.Exists (libJar))
					Downloader.Single(libUrl, libJar, listener);

				// Ugh, apply the extract rules. I hope I do this right.
				// If Extract is present, that means we extract it. Otherwise, the jar just sits there... right??

				if (lib.Extract != null) {
					ZipUtility.Extract(libJar, Path.Combine (GameLocation, "bin", "natives"), "-META-INF");
				} else {
					Directory.CreateDirectory(Path.Combine (GameLocation, "bin"));
					File.Copy (libJar, Path.Combine (GameLocation, "bin", lib.JarName));
				}
				
				++progress;

				listener.SetStatus(string.Format ("Installed {0} / {1} libraries", progress, totalCount));
				listener.SetProgress((double)progress / totalCount);
			}

			listener.SetProgress(1);
		}

		private void InstallGame ()
		{
			Logger.Debug ("Installing game...");

			// Download the minecraft.jar for this version of minecraft. Stored in the VersionsLocation directory
			// so that all instances can share and cache the jars.

			string gameVersion = SelectedVersion;

			if (VersionParameters.GameVersion != null)
				gameVersion = VersionParameters.GameVersion;

			string jarUrl = string.Format ("{0}/{1}/{2}.jar", Minecraft.MinecraftVersionsUrl, gameVersion, gameVersion);
			string jarStore = Path.Combine (VersionsLocation, gameVersion, gameVersion + ".jar");

			if (!File.Exists (jarStore))
				Downloader.Single (jarUrl, jarStore);

			// Install this version as the minecraft.jar under the bin directory of our game instance.

			string jarFile = Path.Combine (GameLocation, "bin", "minecraft.jar");
			if (!File.Exists (jarFile))
				File.Copy (jarStore, jarFile);
		}

        /// <summary>
        /// Look for & Download required Files
        /// </summary>
        /// <returns>Status of exceptions or success</returns>
        public void Setup ()
		{
			Logger.Debug ("Retrieving version information from Minecraft.net...");
			VersionParameters = this.GetVersionParameters(SelectedVersion);
			Logger.Debug ("Verifying assets...");
			InstallAssets();
        }

		public void Install(IStatusListener listener)
		{
			listener.SetTitle("Installing Minecraft...");
			listener.SetProgress(0);

			Directory.CreateDirectory(this.GameLocation);
			Directory.CreateDirectory(Path.Combine (this.GameLocation, "bin"));

			listener.Log ("Retrieving version information from Minecraft.net...");
			VersionParameters = this.GetVersionParameters(SelectedVersion);

			listener.Log ("Craftalyst Minecraft setup...");

			listener.Log ("Installing assets...");
			InstallAssets(listener);

			listener.Log ("Installing libraries...");
			InstallLibraries(listener);
			listener.Log ("Installing game...");
			InstallGame();
		}

        /// <summary>
        /// Unzip Natives
        /// </summary>
        /// <returns>Status of exceptions or success</returns>
        private void GetNatives()
        {
			/*
            foreach (string entry in versionData["natives"])
                otherDotNetZip.Extract(GameLocation + @"\libraries\" + entry, 
					                       GameLocation + @"\versions\" + versionData["id"][0] + @"\" + versionData["id"][0] + "-natives-AL74", "META-INF");
			*/
        }

		public string JarFile {
			get {
				return Path.Combine (GameLocation, "bin", "minecraft.jar");
			}
		}

        /// <summary>
        /// Create and construct Minecraft Command
        /// </summary>
        /// <returns>Status of exceptions or success</returns>
        private string FormatArguments(MinecraftSession session)
        {
			string mcNatives = string.Format ("-Djava.library.path=\"{0}\"", Path.Combine (GameLocation, "bin", "natives"));

			// Add -cp arguments for all non-extracted jar libs that we need

			List<string> mcLibraryEntries = new List<string>();

			// Include libraries in java library path

			var jarLibs = VersionParameters.Libraries.Where (x => x.AppliesHere).Where (x => x.Extract == null);
            foreach (var lib in jarLibs)
				mcLibraryEntries.Add (string.Format ("{0}", Path.Combine (GameLocation, "bin", lib.JarName)));


			mcLibraryEntries.Add (JarFile);

			string mcLibraries = "-cp \""+string.Join(Path.PathSeparator.ToString(), mcLibraryEntries.ToArray())+"\"";

			// Determine the path for the Minecraft jar

			string mcClass = VersionParameters.MainClass;
			string mcArgs = VersionParameters.Arguments;

            mcArgs = mcArgs.Replace("${auth_player_name}", session.Username);
			string accountType = "legacy";

			if (AccountType == MinecraftAccountType.Mojang)
				accountType = "mojang";

			string authSession = "OFFLINE_MODE";
			string authUUID = "OFFLINE_MODE";
			string authAccessToken = "OFFLINE_MODE";
            if (OnlineMode) {
				authSession = "token:" + session.AccessToken + ":" + session.ProfileId;
				authUUID = session.ProfileId;
				authAccessToken = session.AccessToken;
            }
			
            mcArgs = mcArgs.Replace("${auth_session}", authSession);
            mcArgs = mcArgs.Replace("${auth_uuid}", authUUID);
            mcArgs = mcArgs.Replace("${auth_access_token}", authAccessToken);

            mcArgs = mcArgs.Replace("${version_name}", SelectedVersion);
			mcArgs = mcArgs.Replace("${game_directory}", string.Format ("\"{0}\"", GameLocation));
			mcArgs = mcArgs.Replace("${game_assets}", string.Format ("\"{0}\"", AssetsLocation));
			mcArgs = mcArgs.Replace("${assets_root}", string.Format ("\"{0}\"", AssetsLocation));
			mcArgs = mcArgs.Replace("${assets_index_name}", string.Format ("\"{0}\"", VersionParameters.Assets));
			mcArgs = mcArgs.Replace("${user_properties}", "{}");
			mcArgs = mcArgs.Replace("${user_type}", accountType);

			string additionalArgs = "-XX:PermSize=256M -Dfml.ignorePatchDiscrepancies=true -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.log.level=INFO";

            return 
				string.Format ("-Xms{0}M", StartRam) + " " + string.Format("-Xmx{0}M", MaxRam) + " " + additionalArgs + " " + mcNatives + " " + 
					mcLibraries + " " + mcClass + " " + mcArgs;
            
        }

        /// <summary>
        /// Run the command, which should open Minecraft. Assuming everything else is filled in as intended.
        /// </summary>
        /// <returns>Status of exceptions or success</returns>
        private Process RunCommand(MinecraftSession session)
        {
			string args = FormatArguments(session);

			Logger.Debug("Starting minecraft:");
			Logger.Debug ("{0} {1}", JavaFile, args);

            Process mcProc = new Process();
            mcProc.StartInfo.UseShellExecute = false; 
            mcProc.StartInfo.WorkingDirectory = GameLocation;
            mcProc.StartInfo.FileName = JavaFile;
            mcProc.StartInfo.Arguments = args;
			mcProc.StartInfo.RedirectStandardOutput = true;
			mcProc.StartInfo.RedirectStandardError = true;
            mcProc.Start();

            if (CPUPriority == "Realtime") {
                mcProc.PriorityClass = ProcessPriorityClass.RealTime;
            } else if (CPUPriority == "High") {
                mcProc.PriorityClass = ProcessPriorityClass.High;
            } else if (CPUPriority == "Above Normal") {
                mcProc.PriorityClass = ProcessPriorityClass.AboveNormal;
            } else if (CPUPriority == "Below Normal") {
                mcProc.PriorityClass = ProcessPriorityClass.BelowNormal;
            }

			return mcProc;
        }
    }
}
