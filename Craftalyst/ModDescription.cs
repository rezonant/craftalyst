using System;
using System.Collections.Generic;
using System.IO;

namespace Craftalyst {
	public class ModDescription {
		public bool Enabled { get; set; }

		public string Id { get; set; }
		public string Version { get; set; }
		public string FileName { get; set; }
		public string Url { get; set; }

		public List<string> ConfigFiles { get; set; }
		public bool RequiresUser { get; set; }
		
		public string ArtifactId {
			get {
				return string.Format ("{0}:{1}:{2}", Id, FileName, Version);
			}
		}

		public string Name { get; set; }
		public string Description { get; set; }
		public string Changelog { get; set; }
	}
}

