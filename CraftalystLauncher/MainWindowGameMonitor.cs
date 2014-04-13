using System;
using Craftalyst;
using Gtk;

namespace CraftalystLauncher
{
	public class MainWindowGameMonitor : IGameProcessMonitor
	{
		public MainWindowGameMonitor (MainWindow window)
		{
			Window = window;
		}

		public MainWindow Window { get; set; }

		#region IGameProcessMonitor implementation

		public void OutputLine (GameMessageType type, string line)
		{
			DedicatedLauncher.Singleton.EnqueueUxJob(delegate () {
				Window.Log(string.Format("[{0}] {1}", type, line));
			});
		}

		public void GameEnded (int exitCode)
		{
			DedicatedLauncher.Singleton.EnqueueUxJob(delegate () {
				Console.WriteLine("Game Ended!");
				Window.GameEnded(exitCode);
			});
		}

		#endregion

	}
}

