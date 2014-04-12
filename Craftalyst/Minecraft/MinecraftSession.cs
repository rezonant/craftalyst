using System;

namespace Craftalyst
{
	public class MinecraftSession
	{
		public MinecraftSession ()
		{
		}

		public MinecraftSession (string accessToken, string clientToken, string profileID, string username)
		{
			AccessToken = accessToken;
			ClientToken = clientToken;
			ProfileId = profileID;
			Username = username;
			OriginalUsername = username;
		}

		public string AccessToken { get; set; }
		public string ClientToken { get; set; }
		public string ProfileId { get; set; }
		public string Username { get; set; }
		public string OriginalUsername { get; set; }

	}
}

