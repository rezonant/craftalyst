using System;

namespace CraftalystLauncher
{
	public partial class ChooseServerDialog : Gtk.Dialog
	{
		public ChooseServerDialog ()
		{
			this.Build ();
		}

		public string Server {
			get {
				return serverHost.Text;
			}
		}
	}
}

