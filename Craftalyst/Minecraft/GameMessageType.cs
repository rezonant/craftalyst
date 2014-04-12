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
using System.Threading;

namespace Craftalyst
{
	/// <summary>
	/// A convenience facade for the various Minecraft APIs included within Craftalyst.
	/// </summary>
	public enum GameMessageType
	{
		Output = 0,
		Error = 1
	}


}
