using System;
using System.IO;

namespace Craftalyst
{
	public class Mod
	{
		public Mod (ModDescription description)
		{
			Description = description;
		}

		public ModDescription Description { get; set; }

		public string StoreName {
			get {
				return Path.Combine (Description.Id, Description.Version, Description.FileName);
			}
		}

		public void DownloadToStore (string modsStore)
		{
			string localFile = Path.Combine (modsStore, StoreName);

			if (File.Exists (localFile))
				return;

			Downloader.Single(Description.Url, localFile);
		}
		
		public void Install (string modsStore, string gameFolder)
		{
			Install (modsStore, gameFolder, new ConsoleStatusListener());
		}

		public void Install (string modsStore, string gameFolder, IStatusListener listener)
		{
			DownloadToStore (modsStore);

			string localFile = Path.Combine (modsStore, StoreName);
			string destFile = Path.Combine (gameFolder, "mods", Description.FileName);

			if (File.Exists (destFile)) {
				File.Delete (destFile);
			}

			Directory.CreateDirectory(Path.GetDirectoryName(destFile));
			File.Copy(localFile, destFile);
		}		

		public void Remove (string gameFolder)
		{
			string filename = Path.Combine(gameFolder, "mods", Description.FileName);

			if (File.Exists(filename))
				File.Delete(filename);
		}
	}
}

