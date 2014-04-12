using System;
using Gtk;
using System.Threading;

namespace CraftalystLauncher
{
	public partial class InstallationDialog : Gtk.Dialog
	{
		public InstallationDialog ()
		{
			this.Build ();
		}

		public string Status {
			get {
				return status.Text;
			} set {
				status.Text = value;
			}
		}

		public double Progress {
			get {
				return progress.Fraction;
			} set {
				progress.Fraction = value;
			}
		}

		public string TitleLabel {
			get {
				return titleLabel.Text;
			} set {
				titleLabel.Text = value;
			}
		}

		public void Log (string message, params object[] args)
		{
			log.Buffer.Text = log.Buffer.Text + string.Format (message, args) + "\n";
			log.ScrollToIter(log.Buffer.EndIter, 0, false, 0, 0);
		}
	}
}

