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
	public class SyncSummary
	{
		public List<ModDescription> AddedMods { get; set; }
		public List<ModDescription> RemovedMods { get; set; }
		public List<ModUpgradeSummary> UpgradedMods { get; set; }
		public bool NewConfiguration { get; set; }
	}

}
