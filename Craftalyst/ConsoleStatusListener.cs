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
	class ConsoleStatusListener : IStatusListener
	{
		public void Log (string message, params object[] format)
		{
			Console.WriteLine (message, format);
		}

		public void SetTitle (string title)
		{
			Console.WriteLine ();
			Console.WriteLine ("Task: {0}", title);
		}

		public void SetProgress (double progress)
		{
			Console.WriteLine ("Progress: {0}%", progress*100);
		}

		public void SetStatus (string status)
		{
			Console.WriteLine (status);
		}
	}

}
