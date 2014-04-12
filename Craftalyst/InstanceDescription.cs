using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Craftalyst
{
	public class InstanceDescription
	{
		public InstanceDescription ()
		{
		}

		/// <summary>
		/// Specifies the server that this instance should connect to for updates.
		/// This should only be specified when connecting to Craftalyst-compatible 
		/// servers. If your Minecraft server does not expose Craftalyst-compatible 
		/// resources (thus allowing your clients to automatically update based on 
		/// what you are hosting) then do not specify this or leave it blank.
		/// </summary>
		/// <value>
		/// The server.
		/// </value>
		public string SyncUrl { get; set; }

		/// <summary>
		/// Specifies the version of the instance configuration. If this number increases
		/// during syncing, it will cause all configuration files to be redownloaded from the
		/// server. 
		/// </summary>
		/// <value>
		/// The sync version.
		/// </value>
		public int ConfigVersion { get; set; }

		/// <summary>
		/// The version of Minecraft this instance description targets.
		/// </summary>
		/// <value>
		/// The version.
		/// </value>
		public string Version { get; set; }

		/// <summary>
		/// Description to show the user when selecting this instance description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		public string Description { get; set; }

		/// <summary>
		/// Name to show the user when selecting this instance description.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the message of the day.
		/// </summary>
		/// <value>
		/// The message of the day.
		/// </value>
		public string MessageOfTheDay { get; set; }

		/// <summary>
		/// The set of ModDescriptions for this instance description.
		/// Each ModDescription describes a Forge-compatible ("Mods Folder") mod.
		/// No other types are currently supported.
		/// </summary>
		/// <value>
		/// The mods.
		/// </value>
		public List<ModDescription> Mods { get; set; }

		/// <summary>
		/// A list of files beneath "config" which should be synced from server to
		/// client upon launching the instance. You do not need to add config files for
		/// mods here. That should be done in the ModsDescriptions instead (they will be
		/// included when Craftalyst syncs your instance)
		/// </summary>
		/// <value>
		/// The configs.
		/// </value>
		public List<string> SyncConfigs { get; set; }

		/// <summary>
		/// The RAM to reserve for Minecraft when it starts
		/// </summary>
		/// <value>
		/// The start ram.
		/// </value>
		public string StartRam { get; set; }

		/// <summary>
		/// The maximum RAM (in MB) to allow Minecraft to use
		/// </summary>
		/// <value>
		/// The max ram.
		/// </value>
		public string MaxRam { get; set; }

		public static InstanceDescription Parse (string str)
		{
			JsonSerializer serializer = new JsonSerializer();
			using (StringReader sr = new StringReader(str))
				return serializer.Deserialize<InstanceDescription>(new JsonTextReader(sr));
		}

		public static InstanceDescription FromFile(string file)
		{
			using (var sr = new StreamReader(file))
				return InstanceDescription.Parse(sr.ReadToEnd());
		}

		public string ToJson()
		{
			JsonSerializer serializer = new JsonSerializer();
			var sb = new StringBuilder();

			using (StringWriter sw = new StringWriter(sb))
				serializer.Serialize(sw, this);

			return sb.ToString();
		}
	}
}

