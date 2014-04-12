using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;

namespace Craftalyst
{
	/// <summary>
	/// Represents Mojang's official Version Parameters JSON file format, which is published for each
	/// version in the official versions.json file.
	/// </summary>
	public class MinecraftVersionParameters
	{
		public MinecraftVersionParameters ()
		{
		}

		public class LibraryNatives {

			[JsonProperty("linux")]
			public string Linux { get; set; }

			[JsonProperty("windows")]
			public string Windows { get; set; }

			[JsonProperty("osx")]
			public string OSX { get; set; }
		}

		public class LibraryExtractOptions {
			[JsonProperty("exclude")]
			public List<string> Exclude { get; set; }
		}

		public class LibraryRuleOS {
			[JsonProperty("name")]
			public string Name { get; set; }
		}

		public class LibraryRule {

			public enum ActionType {
				[EnumMember(Value = "allow")]
				Allow,

				[EnumMember(Value = "disallow")]
				Disallow
			}

			[JsonProperty("action")]
			public ActionType Action { get; set; }

			[JsonProperty("os")]
			public LibraryRuleOS OS { get; set; }
		}

		public class Library {
			public Library()
			{
				RequiredForClient = true;
				RequiredForServer = true;
				SourceUrl = null;
			}

			[JsonProperty("name")]
			public string Name { get; set; }

			[JsonProperty("url")]
			public string SourceUrl { get; set; }

			[JsonProperty("comment")]
			public string Comment { get; set; }

			[JsonProperty("serverreq")]
			public bool RequiredForServer { get; set; }

			[JsonProperty("clientreq")]
			public bool RequiredForClient { get; set; }

			[JsonProperty("checksums")]
			public List<string> Checksums { get; set; }

			[JsonProperty("rules")]
			public List<LibraryRule> Rules { get; set; }

			[JsonProperty("natives")]
			public LibraryNatives Natives { get; set; }

			[JsonProperty("extract")]
			public LibraryExtractOptions Extract { get; set; }

			public string NativeComponent {
				get {
					if (Natives == null)
						return "";

					if (OSName == "linux")
						return "-"+Natives.Linux;
					else if (OSName == "osx")
						return "-"+Natives.OSX;
					else
						return "-"+Natives.Windows;
				}
			}

			public string StoreName {
				get {
                    string[] colonSplit = Name.Split(new char[] { ':' }, 3);
					string package = colonSplit[0];
					string name = colonSplit[1];
					string version = colonSplit[2];

					return Path.Combine(package.Replace(".", "/"), name, version, string.Format("{0}-{1}.jar", name, version));
				}
			}

			public string Url {
				get {
                    string[] colonSplit = Name.Split(new char[] { ':' }, 3);
					string package = colonSplit[0];
					string name = colonSplit[1];
					string version = colonSplit[2];

					if (SourceUrl == null)
						SourceUrl = Minecraft.MinecraftLibraryUrl;
					else if (package == "org.scala-lang")
						SourceUrl = "http://repo.maven.apache.org/maven2/";

					string libraryUrl = SourceUrl.Trim('/');
					string nativeComponent = NativeComponent;

					if (name == "minecraftforge") {
						// FML apparently stands for fuck my life
						name = "forge";
					}

					if (name == "forge")
						nativeComponent = "-universal";

					return string.Format ("{4}/{0}/{1}/{2}/{1}-{2}{3}.jar", package.Replace(".", "/"), name, version, nativeComponent, libraryUrl);
				}
			}

			public string JarName {
				get {
                    string[] colonSplit = Name.Split(new char[] { ':' }, 3);
					string name = colonSplit[1];
					string version = colonSplit[2];

					return string.Format ("{0}-{1}{2}.jar", name, version, NativeComponent);
				}
			}

			public string GetPath(Minecraft instance)
			{
				return Path.Combine (instance.LibraryLocation, StoreName);
			}

			public static string OSName {
				get {
					if (Environment.OSVersion.Platform == PlatformID.Unix)
						return "linux";
					else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
						return "osx";
					else
						return "windows";
				}
			}

			public bool AppliesHere { 
				get {

					if (Rules != null) {
						bool allow = false;

						foreach (var rule in Rules) {
							if (rule.OS != null) {
								if (rule.OS.Name != OSName)
									continue;
							}

							// TODO: check OS version as well! Extremely needed for OS X support apparently!

							if (rule.Action == LibraryRule.ActionType.Allow)
								allow = true;
							else
								allow = false;
						}

						return allow;
					}

					return true;
				}
			}
		}

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("gameVersion")]
		public string GameVersion { get; set; }

		[JsonProperty("time")]
		public DateTime Time { get; set; }

		[JsonProperty("releaseTime")]
		public DateTime ReleaseTime { get; set; }

		[JsonProperty("type")]
		public MinecraftVersionType Type { get; set; }

		[JsonProperty("minecraftArguments")]
		public string Arguments { get; set; }

		[JsonProperty("minimumLauncherVersion")]
		public int MinimumLauncherVersion { get; set; }

		[JsonProperty("assets", Required = Required.Default)]
		[DefaultValue("legacy")]
		public string Assets { get; set; }

		[JsonProperty("libraries")]
		public List<Library> Libraries { get; set; }

		[JsonProperty("mainClass")]
		public string MainClass { get; set; }

		public static MinecraftVersionParameters Parse(string content)
		{
			JsonSerializer serializer = new JsonSerializer();

			using (StringReader sr = new StringReader(content))
					return serializer.Deserialize(sr, typeof(MinecraftVersionParameters)) as MinecraftVersionParameters;
		}
	}
}

