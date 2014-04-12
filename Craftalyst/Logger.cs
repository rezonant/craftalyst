using System;
using System.IO;

namespace Craftalyst
{
	public class Logger
	{
		private static Stream LogStream { get; set; }
		private static StreamWriter LogWriter { get; set; }
		
		public static void Debug (Exception e)
		{
			Console.WriteLine(e);
			LogWriter.WriteLine(e);
		}

		public static void Debug(string message, params object[] args)
		{
			Console.WriteLine(string.Format (message, args));
			Output (string.Format (message, args));
		}

		public static void Error(string message, params object[] args)
		{
			Console.WriteLine(string.Format (message, args));
			Output (string.Format (message, args));
		}

		public static void Warning(string message, params object[] args)
		{
			Console.WriteLine(string.Format (message, args));
			Output (string.Format (message, args));
		}

		private static void Output (string message)
		{
			string logFile = Path.Combine (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CraftalystLauncherLog.txt");

			if (LogStream == null) {
				Directory.CreateDirectory(Path.GetDirectoryName(logFile));
				LogStream = File.Open (logFile, FileMode.Create);
				LogWriter = new StreamWriter(LogStream);
				LogWriter.AutoFlush = true;
			}

			LogWriter.WriteLine(message);
			LogWriter.Flush();
		}
	}
}

