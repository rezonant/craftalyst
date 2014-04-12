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
	public class ModUpgradeSummary
	{
		public ModDescription OldVersion { get; set; }
		public ModDescription NewVersion { get; set; }
	}


}
