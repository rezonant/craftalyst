using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Linq;

namespace Craftalyst
{
	public class SavedCredentials
	{
		public SavedCredentials ()
		{
		}

		public string AutoLoginUser { get; set; }
		public List<SavedCredential> Credentials { get; set; }

		public SavedCredential GetCredentialsFor(string user)
		{
			return Credentials.Where(x => x.UserName == user).SingleOrDefault();
		}

		public static SavedCredentials Parse(string json)
		{
			var serializer = new JsonSerializer();
			using (StringReader sr = new StringReader(json))
				return serializer.Deserialize<SavedCredentials>(new JsonTextReader(sr));
		}

		public string ToJson()
		{
			var serializer = new JsonSerializer();
			StringBuilder sb = new StringBuilder();
			using (StringWriter sw = new StringWriter(sb))
				serializer.Serialize(new JsonTextWriter(sw), this);
			return sb.ToString();
		}
	}
}

