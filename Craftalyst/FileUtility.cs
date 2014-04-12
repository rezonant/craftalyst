using System;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Text;

namespace Craftalyst
{
	public static class FileUtility
	{
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

