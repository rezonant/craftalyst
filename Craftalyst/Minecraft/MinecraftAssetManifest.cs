using System;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Craftalyst
{
	public class MinecraftAssetManifest
	{
		public MinecraftAssetManifest ()
		{
		}

		public class AssetDescription {
			public string FileName { get; set; }
			public string Hash { get; set; }
			public int Size { get; set; }

			public string ObjectFilename {
				get {
					return Path.Combine(Hash.Substring(0, 2), Hash);
				}
			}

			public string Url {
				get {
					return string.Format ("{0}/{1}/{2}", Minecraft.MinecraftAssetUrl, Hash.Substring(0, 2), Hash);
				}
			}

			public string GetAssetFile (string assetsDirectory)
			{
				string objectDirectory = Path.Combine (assetsDirectory, "objects");
				return Path.Combine (objectDirectory, ObjectFilename);
			}

			public bool IsInstalled (string assetsDirectory)
			{
				string file = GetAssetFile(assetsDirectory);
				if (!File.Exists (file))
					return false;

				if (FileUtility.Hash(file) != this.Hash)
					return false;

				return true;
			}

			public void Install (MinecraftAssetManifest manifest, string assetsDirectory, IStatusListener listener)
			{
				string assetFile = GetAssetFile(assetsDirectory);
				Directory.CreateDirectory(Path.GetDirectoryName(assetFile));

				if (!File.Exists (assetFile)) 
					Download (assetsDirectory, listener);

				// If this manifest is virtual, then keep the assets/legacy/virtual directly up to date with the correct
				// version of the file.

				if (manifest.Virtual) {
					string virtualAssetFile = Path.Combine (assetsDirectory, "legacy", "virtual", FileName);
					if (!File.Exists (virtualAssetFile) || FileUtility.Hash (virtualAssetFile) != Hash) {
						if (File.Exists(virtualAssetFile))
							File.Delete (virtualAssetFile);
						Directory.CreateDirectory(Path.GetDirectoryName(virtualAssetFile));
						File.Copy (assetFile, virtualAssetFile);
					}
				}
			}

			private void Download (string assetsDirectory, IStatusListener listener)
			{
				string objectDirectory = Path.Combine (assetsDirectory, "objects");
				Directory.CreateDirectory(objectDirectory);

				string assetFile = Path.Combine (objectDirectory, ObjectFilename);

				for (int i = 0, max = 5; i < max; ++i) {
					Downloader.Single (Url, assetFile);
					string actualHash = FileUtility.Hash (assetFile);

					if (actualHash == null)
						throw new InvalidOperationException(string.Format ("Failed to create hash to of '{0}' (with expected hash {1})", FileName, Hash));

					if (actualHash != Hash) {
						if (i + 1 == max) {
							throw new Exception(string.Format ("Failed to verify downloaded asset '{0}' (expected hash {1}, got hash {2})", 
							                                   FileName, Hash, actualHash));
						} else {
								listener.Log ("Error: File '{0}' failed hash check (expected {1}, got {2}) after downloading, retrying (attempt #{3})", 
								                   FileName, Hash, actualHash, i);
						}
						continue;
					} else {
						//listener.Log ("Verified hash for asset {0} was exactly that hash!", Hash);
					}

					break;
				}
			}
		}

		public bool Virtual { get; set; }
		public List<AssetDescription> Objects { get; set; }

		public static MinecraftAssetManifest Parse (string content)
		{
			var jobj = JObject.Parse (content);
			var mam = new MinecraftAssetManifest();

			mam.Virtual = false;

			if (jobj["virtual"] != null)
				mam.Virtual = jobj["virtual"].Value<bool>();

			mam.Objects = new List<AssetDescription>();

			foreach (JProperty item in ((JObject)jobj["objects"]).Properties()) {
				mam.Objects.Add(new AssetDescription() {
					FileName = item.Name,
					Hash = item.Value["hash"].Value<string>(),
					Size = item.Value["size"].Value<int>()
				});
			}

			return mam;
		}
	}
}

