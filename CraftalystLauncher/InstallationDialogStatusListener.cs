using System;
using Craftalyst;
using Gtk;
using System.Threading;

namespace CraftalystLauncher
{
	public class InstallationDialogStatusListener : IStatusListener
	{
		public InstallationDialogStatusListener (InstallationDialog dialog)
		{
			Dialog = dialog;
		}

		private InstallationDialog Dialog { get; set; }

		#region IStatusListener implementation
		public void Log (string message, params object[] format)
		{
			Console.WriteLine (message, format);
			DedicatedLauncher.Singleton.EnqueueUxJob(delegate() {
				Dialog.Log (message, format);
			});
		}

		public void SetTitle (string title)
		{
			DedicatedLauncher.Singleton.EnqueueUxJob(delegate() {
				Dialog.TitleLabel = title;
			});
		}

		public void SetProgress (double progress)
		{
			DedicatedLauncher.Singleton.EnqueueUxJob(delegate() {
				Dialog.Progress = progress;
			});
		}

		public void SetStatus (string status)
		{
			DedicatedLauncher.Singleton.EnqueueUxJob(delegate() {
				Dialog.Status = status;
			});
		}
		#endregion
	}
}

