using System;
using System.Timers;

namespace CraftalystLauncher
{
	public partial class LoginDialog : Gtk.Dialog
	{
		public LoginDialog ()
		{
			this.Build ();
		}

		public string Username {
			get {
				return this.username.Text;
			}
		}

		public string Password {
			get {
				return this.password.Text;
			}
		}

		protected void OnButtonCancelClicked (object sender, EventArgs e)
		{
			this.Respond(Gtk.ResponseType.Cancel);
		}		

		protected void OnButtonOkActivated (object sender, EventArgs e)
		{
			this.Respond(Gtk.ResponseType.Ok);
		}

		protected void OnUsernameActivated (object sender, EventArgs e)
		{
			OnButtonOkActivated(sender, e);
		}

		protected void OnPasswordActivated (object sender, EventArgs e)
		{
			OnButtonOkActivated(sender, e);
		}


	}
}

