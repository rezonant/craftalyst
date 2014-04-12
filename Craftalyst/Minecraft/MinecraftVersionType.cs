using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

namespace Craftalyst
{
	public enum MinecraftVersionType {
		[EnumMember(Value = "old_alpha")]
		OldAlpha,

		[EnumMember(Value = "old_beta")]
		OldBeta,

		[EnumMember(Value = "snapshot")]
		Snapshot,

		[EnumMember(Value = "release")]
		Release
	}
    
}
