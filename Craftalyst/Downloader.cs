using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace Craftalyst
{
    public class Downloader
    {
		public static string ActiveFile { get; private set; }
		public static string ActiveURL { get; private set; }
		public static int Progress { get; private set; }

		public static void Single(string url, string file)
		{
			Single(url, file, new ConsoleStatusListener());
		}

		public static Stream Open(string url)
		{
			var webclient = new WebClient();
			return webclient.OpenRead(new Uri(url));
		}

		/// <summary>
		/// Download a single URL to a given file location.
		/// </summary>
		/// <param name='urlFilePATH'>
		/// URL file PAT.
		/// </param>
		/// <param name='filePATH'>
		/// File PAT.
		/// </param>
		public static void Single (string urlFilePATH, string filePATH, IStatusListener listener)
		{
			var loader = new Downloader();
			loader.Download (urlFilePATH, filePATH, listener);
		}

        /// <summary>
        /// Input a single file to download. Does not stop until error or completed.
        /// </summary>
        /// <param name="urlFilePATH">Example: "http://www.webaddress.com/file.zip" </param>
        /// <param name="filePATH">Example: "C:\LOCATION\file.zip" </param>
        public void Download(string urlFilePATH, string filePATH, IStatusListener listener)
        {
			ActiveFile = filePATH;
			ActiveURL = urlFilePATH;

			listener.Log("[Downloader] {0} ==> {1}", urlFilePATH, filePATH);

            Directory.CreateDirectory(Path.GetDirectoryName(filePATH));
            using (var webConnect = new WebClient()) {
				webConnect.Headers["User-Agent"] = "Craftalyst/0.1";
				webConnect.DownloadProgressChanged += HandleProgress;
                webConnect.DownloadFile(new Uri(urlFilePATH), filePATH);
            }

			ActiveFile = "";
			ActiveURL = "";
			Progress = 0;
        }
		

		public event ProgressChangedEventHandler OnProgress;

        /// <summary>
        /// The method used by the AsyncDownload to show progress.
        /// </summary>
        /// <param name="sender">Object Sender</param>
        /// <param name="e">Download Progress Changed Event Args - e</param>
        private void HandleProgress(object sender, DownloadProgressChangedEventArgs e)
        {
			Progress = e.ProgressPercentage;
			this.OnProgress(sender, e);
        }
    }
}
