using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace Craftalyst {
    public class MinecraftVersionList {
		public MinecraftVersionList ()
		{

		}

		#region Constants

		public const string VersionListUrl = "http://s3.amazonaws.com/Minecraft.Download/versions/versions.json";

		#endregion
		#region Inner Types

		public class LatestVersions {
			[JsonProperty("snapshot")]
			public string Snapshot { get; set; }

			[JsonProperty("release")]
			public string Release { get; set; }
		}

		public class Version {
			public Version (string id, DateTime time, DateTime releaseTime, MinecraftVersionType type)
			{
				Id = id;
				Time = time;
				ReleaseTime = releaseTime;
				Type = type;
			}

			[JsonProperty("id")]
			public string Id { get; set; }
			
			[JsonProperty("time")]
			public DateTime Time { get; set; }
			
			[JsonProperty("releaseTime")]
			public DateTime ReleaseTime { get; set; }
			
			[JsonProperty("type")]
			public MinecraftVersionType Type { get; set; }
		}

		#endregion
		#region Fields

		[JsonProperty("latest")]
		public LatestVersions Latest { get; set; }

		[JsonProperty("versions")]
		public List<Version> Versions { get; set; }

		#endregion
		#region Methods

		public static MinecraftVersionList Get()
		{
			string cacheFolder = Path.Combine (Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Craftalyst");
			string fileName = Path.Combine(cacheFolder, "versions.json");
			
			Directory.CreateDirectory(cacheFolder);

            if (!File.Exists(fileName) || (DateTime.Now - File.GetLastWriteTime(fileName)).TotalHours > 1) {
                try {
                    Downloader.Single(VersionListUrl, fileName);
                } catch (Exception ex) {
					throw new Exception("Failed to download versions.json", ex);
                }
            }

            if (!File.Exists(fileName))
				throw new FileNotFoundException(fileName);

			return ParseVersionList(fileName);
		}

        /// <summary>
        /// Parses the minecraft json related to a specific version.
        /// </summary>
        /// <param name="jsonFile">The json file for a sepcific version. Example: "C:\LOCATION\1.6.2.json"</param>
        /// <param name="useNightly">Whether to use the nightly builds or not. Could be unstable.</param>
        /// <returns>Returns a dictonary with all of the elements.</returns>
        public static Dictionary<string, string[]> GetVersionData(string jsonFile, bool useNightly = false)
        {
            var json = System.IO.File.ReadAllText(jsonFile);
            dynamic version = JsonConvert.DeserializeObject(json);
            string[] libraries = { "" };
            int l = 0;
            string[] natives = { "" };
            int n = 0;
            foreach (var param in version.libraries)
            {
                bool isNative = false;
                bool addFile = true;
                if (param.rules != null)
                {
                    if (param.rules[0].action == "allow")
                    {
                        if (useNightly)
                        {
                            if (param.rules[0].os == null)
                            {
                                addFile = false;
                            }
                        }
                        else
                        {
                            if (param.rules[0].os != null)
                            {
                                addFile = false;
                            }
                        }
                    }
                }

                if (addFile)
                {
                    string fileName = param.name;
                    string[] colonSplit = fileName.Split(new char[] { ':' }, 3);
                    string[] folderSplit = colonSplit[0].Split(new char[] { '.' });
                    string compileFolder = "";
                    for (int a = 0; a < folderSplit.Length; a++)
                    {
                        if (a == 0)
                        {
                            compileFolder = folderSplit[a];
                        }
                        else
                        {
                            compileFolder = compileFolder + @"\" + folderSplit[a];
                        }
                    }
                    if (param.natives != null)
                    {
                        isNative = true;
                        compileFolder = compileFolder + @"\" + colonSplit[1] + @"\" + colonSplit[2] + @"\" + colonSplit[1] + "-" + colonSplit[2] + "-" + param.natives.windows + ".jar";
                    }
                    else
                    {
                        compileFolder = compileFolder + @"\" + colonSplit[1] + @"\" + colonSplit[2] + @"\" + colonSplit[1] + "-" + colonSplit[2] + ".jar";
                    }
                    if (l > libraries.Length - 1)
                    {
                        Array.Resize(ref libraries, libraries.Length + 1);
                    }
                    libraries[l] = compileFolder;
                    l++;
                    if (isNative)
                    {
                        if (n > natives.Length - 1)
                        {
                            Array.Resize(ref natives, natives.Length + 1);
                        }
                        natives[n] = compileFolder;
                        n++;
                    }
                }
            }
            Dictionary<string, string[]> dict = new Dictionary<string, string[]>{
                {"id"                  , new string[] { version.id }},
                {"time"                , new string[] { version.time }},
                {"releaseTime"         , new string[] { version.releaseTime }},
                {"Type"                , new string[] { version.type }},
                {"minecraftArguments"  , new string[] { version.minecraftArguments }},
                {"mainClass"           , new string[] { version.mainClass }},
              //{"libraries"           , new string[] { "net/sf/jopt-simple/jopt-simple/4.5/jopt-simple-4.5.jar" "etc" "etc" }},
                {"libraries"           , libraries },
              //{"natives"             , new string[] { "net/sf/jopt-simple/jopt-simple/4.5/jopt-simple-4.5.jar" "etc" "etc" }},
                {"natives"             , natives   }
            };
            return dict;
        }

        /// <summary>
        /// Parses the minecraft verion list.
        /// </summary>
        /// <param name="jsonFile">The json file for the list of versions. Example: "C:\LOCATION\verions.json"</param>
        /// <returns>Returns a dictonary with all of the elements.</returns>
        public static MinecraftVersionList ParseVersionList(string jsonFile)
        {
			var serializer = new JsonSerializer();

			using (StreamReader sr = new StreamReader(jsonFile))
				return serializer.Deserialize(sr, typeof(MinecraftVersionList)) 
					as MinecraftVersionList;
        }

		#endregion
    }
}
