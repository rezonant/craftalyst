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
	class FailedDownloadException : Exception
	{
		public FailedDownloadException (string syncUrl, Exception inner):
			base ("A file failed to download properly", inner)
		{
			DownloadUrl = syncUrl;
		}

		public string DownloadUrl { get; set; }
	}

}
