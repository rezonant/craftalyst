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
	/// Implement this interface and pass an instance to Minecraft.Start() to
	/// receive events while Minecraft is running.
	/// </summary>
	public interface IGameProcessMonitor
	{
		void OutputLine(GameMessageType type, string line);
		void GameEnded(int exitCode);
	}

}
