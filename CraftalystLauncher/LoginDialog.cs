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
			} set {
				this.username.Text = value;
			}
		}

		public string Password {
			get {
				return this.password.Text;
			} set {
				this.password.Text = value;
			}
		}

		public bool RememberCredentials {
			get {
				return rememberCreds.Active;
			} set {
				rememberCreds.Active = value;
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

