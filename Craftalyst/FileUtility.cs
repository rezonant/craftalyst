using System;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Craftalyst
{
	public static class FileUtility
	{
		public static string FindOnPath(string exe)
		{
			Logger.Debug("The PATH variable:");
			Logger.Debug(Environment.GetEnvironmentVariable("PATH"));

			var options = Environment.GetEnvironmentVariable("PATH")
				.Split(Path.PathSeparator)
				.Where(x => !string.IsNullOrEmpty(x.Trim()))
				.Select(dir => Path.Combine(dir, exe));

			if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
				options = options.Concat(
					Environment.GetEnvironmentVariable("PATH")
					.Split(Path.PathSeparator)
					.Where(x => !string.IsNullOrEmpty(x.Trim()))
					.Where(x => Environment.OSVersion.Platform == PlatformID.Win32NT)
					.Select(dir => Path.Combine(dir, string.Format("{0}.exe", exe)))
						);
				options = options.Select(x => x.ToLower().Replace("c:\\windows\\system32", "c:\\windows\\sysnative"));
			}

			Logger.Debug("");
			Logger.Debug("JAVA OPTIONS:");
			foreach (var option in options) {
				Logger.Debug(" - {0}", option);
				Logger.Debug("   Exists: {0}", File.Exists(option));
			}

			string result = options
				.Where(file => File.Exists(file))
				.FirstOrDefault();

			Logger.Debug("System: "+Environment.OSVersion.Platform);
			Logger.Debug("Best guess to Java: '{0}'", result);

			if (result == null) {
				Logger.Debug("Result does equal null...");
			}

			return result;
		}

		/// <summary>
		/// Generate an SHA1 hash string (with lower case hexits!)
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance hash filename; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='filename'>
		/// Filename.
		/// </param>
		public static string Hash (string filename)
		{
			var crypto = new SHA1CryptoServiceProvider ();

			if (!File.Exists (filename))
				return null;

			using (Stream fstream = File.Open (filename, FileMode.Open)) {
				byte[] hash = crypto.ComputeHash (fstream);
				StringBuilder hashString = new StringBuilder();

				foreach (byte hbyte in hash)
					hashString.Append(hbyte.ToString("x2"));

				return hashString.ToString();
			}
		}

        public static bool DeleteSafely(string pathFILE)
        {
			for (int iteration = 0, max = 10; iteration < max; ++iteration) {
				try {
                	File.Delete(pathFILE);
					return true;
				} catch (Exception) { }

                Thread.Sleep(1000);
            }

			return false;
        }
	}
}

