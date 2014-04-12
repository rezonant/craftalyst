using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Xml.Linq;
using System.Security.AccessControl;

namespace Craftalyst
{
	/// <summary>
	/// A simple IGameProcessMonitor implementation that writes directly to the Console.
	/// </summary>
	class ConsoleGameProcessMonitor : IGameProcessMonitor
	{
		#region IGameProcessMonitor implementation
		public void OutputLine (string line)
		{
			Console.WriteLine(line);
		}

		public void GameEnded (int exitCode)
		{
			Console.WriteLine("[EXIT] Minecraft has exited with code {0}", exitCode);
		}
		#endregion
	}

}
