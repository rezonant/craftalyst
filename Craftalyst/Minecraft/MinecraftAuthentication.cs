using System;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Craftalyst
{
	/// <summary>
	/// Handles authentication of Minecraft users via the official Mojang authentication API.
	/// </summary>
	public class MinecraftAuthentication
	{
		/// <summary>
		/// Login with the Mojang authentication API, using the specified username and password.
		/// </summary>
		/// <param name='username'>
		/// Mojang account email address or for legacy accounts, the Minecraft username
		/// </param>
		/// <param name='password'>
		/// The user-entered password. It is passed over HTTPS directly to Mojang.
		/// </param>
		public MinecraftSession Login (string username, string password)
		{
			JObject responseJson = RequestLogin (username, password);

			if (responseJson["accessToken"] != null)
				return new MinecraftSession (
					responseJson["accessToken"].Value<string>(), 
					responseJson["clientToken"].Value<string>(), 
					responseJson["selectedProfile"]["id"].Value<string>(),
					responseJson["selectedProfile"]["name"].Value<string>());

			if (responseJson["errorMessage"] == null)
				throw new Exception ("Failed to login: No access token nor error message available (Likely the response failed to parse)");
			
			throw new Exception(responseJson["errorMessage"].Value<string>());
		}

		/// <summary>
		/// Sends the raw request to Mojang and returns a dynamic containing the JSON object response.
		/// </summary>
		/// <returns>
		/// The login.
		/// </returns>
		/// <param name='username'>
		/// Username.
		/// </param>
		/// <param name='password'>
		/// Password.
		/// </param>
		public JObject RequestLogin (string username, string password)
		{
			try {
	            WebRequest request = WebRequest.Create("https://authserver.mojang.com/authenticate");   //Start WebRequest
	            request.Method = "POST";                                                                //Method type, POST
	            string json = Newtonsoft.Json.JsonConvert.SerializeObject(new                           //Object to Upload
	            {
	                agent = new                 // optional /                                           //This seems to be required for minecraft despite them saying its optional.
	                {                           //          /
	                    name = "Minecraft",     // -------- / So far this is the only encountered value
	                    version = 1             // -------- / This number might be increased by the vanilla client in the future
	                },                          //          /
	                username = username,   // Can be an email address or player name for unmigrated accounts
	                password = password,
	                //clientToken = "TOKEN"     // Client Identifier: optional
	            });

				// Write JSON to stream

				byte[] postData = Encoding.UTF8.GetBytes(json);

	            request.ContentType = "application/json";
	            request.ContentLength = postData.Length;
				request.Timeout = 99999;
					
				using (Stream dataStream = request.GetRequestStream())
					dataStream.Write (postData, 0, postData.Length);

				// Serialize response into a dynamic.

				JObject data;

	            using (WebResponse response = request.GetResponse())
	            using (Stream dataStream = response.GetResponseStream())
	            using (StreamReader reader = new StreamReader(dataStream))
						data = JObject.Parse (reader.ReadToEnd());

				return data;

			} catch (WebException webEx) {
                try {
					
					JObject error;

                    using (Stream dataStream = webEx.Response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(dataStream))
						error = JObject.Parse(reader.ReadToEnd());

					return error;
					//throw new Exception(error["errorMessage"].Value<string>());
                } catch (Exception subEx) {
					throw new Exception(
						"Web exception: "+webEx.Message+". Also experienced a '"+subEx.GetType().Name +": "+subEx.Message+
							"' while attempting to parse error message.", subEx);
                }
				//throw new Exception(responseJson.errorMessage);
            } catch (Exception ex) {
				string message;
                if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                    message = "Error: Internet Disconnected. Use Offline Mode in order to play without internet.";
                else
                    message = "Error: Web: " + ex.Message;

				throw new Exception(message);
            }
		}
	}
}