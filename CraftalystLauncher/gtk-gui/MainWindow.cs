
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.UIManager UIManager;
	private global::Gtk.VBox vbox1;
	private global::Gtk.HPaned hpaned1;
	private global::Gtk.VBox vbox4;
	private global::Gtk.HBox hbox4;
	private global::Gtk.Image image1;
	private global::Gtk.Label titleLabel;
	private global::Gtk.Button mapButton;
	private global::Gtk.Button tsButton;
	private global::Gtk.Label minecraftLabel;
	private global::Gtk.ScrolledWindow GtkScrolledWindow;
	private global::Gtk.TextView minecraftLog;
	private global::Gtk.VBox vbox3;
	private global::Gtk.Label usernameLabel;
	private global::Gtk.ScrolledWindow GtkScrolledWindow1;
	private global::Gtk.TextView overviewAndStatus;
	private global::Gtk.HBox hbox5;
	private global::Gtk.Button websiteButton;
	private global::Gtk.Button wikiButton;
	private global::Gtk.Button forumsButton;
	private global::Gtk.Button donateButton;
	private global::Gtk.Label newsLabel;
	private global::Gtk.ScrolledWindow GtkScrolledWindow2;
	private global::Gtk.TextView serverNews;
	private global::Gtk.HBox hbox3;
	private global::Gtk.Label newsLabel1;
	private global::Gtk.Button reportServerDownButton;
	private global::Gtk.HBox hbox2;
	private global::Gtk.Button playButton;
	private global::Gtk.Label minecraftStatus;
	
	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.UIManager = new global::Gtk.UIManager ();
		global::Gtk.ActionGroup w1 = new global::Gtk.ActionGroup ("Default");
		this.UIManager.InsertActionGroup (w1, 0);
		this.AddAccelGroup (this.UIManager.AccelGroup);
		this.Name = "MainWindow";
		this.Title = global::Mono.Unix.Catalog.GetString ("Craftalyst");
		this.WindowPosition = ((global::Gtk.WindowPosition)(1));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox1 = new global::Gtk.VBox ();
		this.vbox1.Name = "vbox1";
		this.vbox1.Spacing = 6;
		this.vbox1.BorderWidth = ((uint)(5));
		// Container child vbox1.Gtk.Box+BoxChild
		this.hpaned1 = new global::Gtk.HPaned ();
		this.hpaned1.CanFocus = true;
		this.hpaned1.Name = "hpaned1";
		this.hpaned1.Position = 697;
		// Container child hpaned1.Gtk.Paned+PanedChild
		this.vbox4 = new global::Gtk.VBox ();
		this.vbox4.Name = "vbox4";
		this.vbox4.Spacing = 6;
		// Container child vbox4.Gtk.Box+BoxChild
		this.hbox4 = new global::Gtk.HBox ();
		this.hbox4.Name = "hbox4";
		this.hbox4.Spacing = 6;
		// Container child hbox4.Gtk.Box+BoxChild
		this.image1 = new global::Gtk.Image ();
		this.image1.Name = "image1";
		this.image1.Pixbuf = global::Gdk.Pixbuf.LoadFromResource ("CraftalystLauncher.icon.png");
		this.hbox4.Add (this.image1);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.image1]));
		w2.Position = 0;
		w2.Expand = false;
		w2.Fill = false;
		w2.Padding = ((uint)(10));
		// Container child hbox4.Gtk.Box+BoxChild
		this.titleLabel = new global::Gtk.Label ();
		this.titleLabel.Name = "titleLabel";
		this.titleLabel.Xalign = 0F;
		this.titleLabel.LabelProp = "<big><big><big><big>Tirrin</big></big></big></big>";
		this.titleLabel.UseMarkup = true;
		this.hbox4.Add (this.titleLabel);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.titleLabel]));
		w3.Position = 1;
		// Container child hbox4.Gtk.Box+BoxChild
		this.mapButton = new global::Gtk.Button ();
		this.mapButton.CanFocus = true;
		this.mapButton.Name = "mapButton";
		this.mapButton.UseUnderline = true;
		this.mapButton.Label = global::Mono.Unix.Catalog.GetString ("Map");
		this.hbox4.Add (this.mapButton);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.mapButton]));
		w4.Position = 2;
		w4.Expand = false;
		w4.Fill = false;
		// Container child hbox4.Gtk.Box+BoxChild
		this.tsButton = new global::Gtk.Button ();
		this.tsButton.CanFocus = true;
		this.tsButton.Name = "tsButton";
		this.tsButton.UseUnderline = true;
		this.tsButton.Label = global::Mono.Unix.Catalog.GetString ("Teamspeak");
		this.hbox4.Add (this.tsButton);
		global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.hbox4 [this.tsButton]));
		w5.Position = 3;
		w5.Expand = false;
		w5.Fill = false;
		this.vbox4.Add (this.hbox4);
		global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.hbox4]));
		w6.Position = 0;
		w6.Expand = false;
		w6.Fill = false;
		// Container child vbox4.Gtk.Box+BoxChild
		this.minecraftLabel = new global::Gtk.Label ();
		this.minecraftLabel.Name = "minecraftLabel";
		this.minecraftLabel.Xalign = 0F;
		this.minecraftLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Minecraft:");
		this.vbox4.Add (this.minecraftLabel);
		global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.minecraftLabel]));
		w7.Position = 1;
		w7.Expand = false;
		w7.Fill = false;
		// Container child vbox4.Gtk.Box+BoxChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.minecraftLog = new global::Gtk.TextView ();
		this.minecraftLog.Buffer.Text = "Click Play below to start Minecraft!\n";
		this.minecraftLog.CanFocus = true;
		this.minecraftLog.Name = "minecraftLog";
		this.minecraftLog.Editable = false;
		this.minecraftLog.CursorVisible = false;
		this.minecraftLog.WrapMode = ((global::Gtk.WrapMode)(2));
		this.GtkScrolledWindow.Add (this.minecraftLog);
		this.vbox4.Add (this.GtkScrolledWindow);
		global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.vbox4 [this.GtkScrolledWindow]));
		w9.Position = 2;
		this.hpaned1.Add (this.vbox4);
		global::Gtk.Paned.PanedChild w10 = ((global::Gtk.Paned.PanedChild)(this.hpaned1 [this.vbox4]));
		w10.Resize = false;
		// Container child hpaned1.Gtk.Paned+PanedChild
		this.vbox3 = new global::Gtk.VBox ();
		this.vbox3.Name = "vbox3";
		this.vbox3.Spacing = 6;
		// Container child vbox3.Gtk.Box+BoxChild
		this.usernameLabel = new global::Gtk.Label ();
		this.usernameLabel.Name = "usernameLabel";
		this.usernameLabel.Xalign = 1F;
		this.usernameLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("Playing as <b>rezonaut</b>, because you know he's cool!");
		this.usernameLabel.UseMarkup = true;
		this.usernameLabel.Wrap = true;
		this.vbox3.Add (this.usernameLabel);
		global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.usernameLabel]));
		w11.Position = 0;
		w11.Expand = false;
		w11.Fill = false;
		// Container child vbox3.Gtk.Box+BoxChild
		this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow1.HeightRequest = 200;
		this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
		this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
		this.overviewAndStatus = new global::Gtk.TextView ();
		this.overviewAndStatus.Buffer.Text = "Please wait...";
		this.overviewAndStatus.CanFocus = true;
		this.overviewAndStatus.Name = "overviewAndStatus";
		this.overviewAndStatus.Editable = false;
		this.overviewAndStatus.CursorVisible = false;
		this.overviewAndStatus.WrapMode = ((global::Gtk.WrapMode)(2));
		this.GtkScrolledWindow1.Add (this.overviewAndStatus);
		this.vbox3.Add (this.GtkScrolledWindow1);
		global::Gtk.Box.BoxChild w13 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.GtkScrolledWindow1]));
		w13.Position = 1;
		w13.Expand = false;
		// Container child vbox3.Gtk.Box+BoxChild
		this.hbox5 = new global::Gtk.HBox ();
		this.hbox5.Name = "hbox5";
		this.hbox5.Spacing = 6;
		// Container child hbox5.Gtk.Box+BoxChild
		this.websiteButton = new global::Gtk.Button ();
		this.websiteButton.CanFocus = true;
		this.websiteButton.Name = "websiteButton";
		this.websiteButton.UseUnderline = true;
		this.websiteButton.Label = global::Mono.Unix.Catalog.GetString ("Website");
		this.hbox5.Add (this.websiteButton);
		global::Gtk.Box.BoxChild w14 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.websiteButton]));
		w14.Position = 0;
		w14.Expand = false;
		w14.Fill = false;
		// Container child hbox5.Gtk.Box+BoxChild
		this.wikiButton = new global::Gtk.Button ();
		this.wikiButton.CanFocus = true;
		this.wikiButton.Name = "wikiButton";
		this.wikiButton.UseUnderline = true;
		this.wikiButton.Label = global::Mono.Unix.Catalog.GetString ("Wiki");
		this.hbox5.Add (this.wikiButton);
		global::Gtk.Box.BoxChild w15 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.wikiButton]));
		w15.Position = 1;
		w15.Expand = false;
		w15.Fill = false;
		// Container child hbox5.Gtk.Box+BoxChild
		this.forumsButton = new global::Gtk.Button ();
		this.forumsButton.CanFocus = true;
		this.forumsButton.Name = "forumsButton";
		this.forumsButton.UseUnderline = true;
		this.forumsButton.Label = global::Mono.Unix.Catalog.GetString ("Forums");
		this.hbox5.Add (this.forumsButton);
		global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.forumsButton]));
		w16.Position = 2;
		w16.Expand = false;
		w16.Fill = false;
		// Container child hbox5.Gtk.Box+BoxChild
		this.donateButton = new global::Gtk.Button ();
		this.donateButton.CanFocus = true;
		this.donateButton.Name = "donateButton";
		this.donateButton.UseUnderline = true;
		this.donateButton.Label = global::Mono.Unix.Catalog.GetString ("Donate");
		this.hbox5.Add (this.donateButton);
		global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.hbox5 [this.donateButton]));
		w17.Position = 3;
		w17.Expand = false;
		w17.Fill = false;
		this.vbox3.Add (this.hbox5);
		global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox5]));
		w18.Position = 2;
		w18.Expand = false;
		w18.Fill = false;
		// Container child vbox3.Gtk.Box+BoxChild
		this.newsLabel = new global::Gtk.Label ();
		this.newsLabel.Name = "newsLabel";
		this.newsLabel.LabelProp = global::Mono.Unix.Catalog.GetString ("News of the Day");
		this.vbox3.Add (this.newsLabel);
		global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.newsLabel]));
		w19.Position = 3;
		w19.Expand = false;
		w19.Fill = false;
		// Container child vbox3.Gtk.Box+BoxChild
		this.GtkScrolledWindow2 = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow2.Name = "GtkScrolledWindow2";
		this.GtkScrolledWindow2.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow2.Gtk.Container+ContainerChild
		this.serverNews = new global::Gtk.TextView ();
		this.serverNews.Buffer.Text = "Things and news and things!";
		this.serverNews.CanFocus = true;
		this.serverNews.Name = "serverNews";
		this.serverNews.Editable = false;
		this.serverNews.CursorVisible = false;
		this.serverNews.WrapMode = ((global::Gtk.WrapMode)(2));
		this.GtkScrolledWindow2.Add (this.serverNews);
		this.vbox3.Add (this.GtkScrolledWindow2);
		global::Gtk.Box.BoxChild w21 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.GtkScrolledWindow2]));
		w21.Position = 4;
		// Container child vbox3.Gtk.Box+BoxChild
		this.hbox3 = new global::Gtk.HBox ();
		this.hbox3.Name = "hbox3";
		this.hbox3.Spacing = 6;
		// Container child hbox3.Gtk.Box+BoxChild
		this.newsLabel1 = new global::Gtk.Label ();
		this.newsLabel1.Name = "newsLabel1";
		this.newsLabel1.LabelProp = global::Mono.Unix.Catalog.GetString ("Internet: Yes");
		this.hbox3.Add (this.newsLabel1);
		global::Gtk.Box.BoxChild w22 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.newsLabel1]));
		w22.Position = 0;
		// Container child hbox3.Gtk.Box+BoxChild
		this.reportServerDownButton = new global::Gtk.Button ();
		this.reportServerDownButton.CanFocus = true;
		this.reportServerDownButton.Name = "reportServerDownButton";
		this.reportServerDownButton.UseUnderline = true;
		this.reportServerDownButton.Label = global::Mono.Unix.Catalog.GetString ("Report Server Down");
		this.hbox3.Add (this.reportServerDownButton);
		global::Gtk.Box.BoxChild w23 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.reportServerDownButton]));
		w23.Position = 1;
		w23.Expand = false;
		w23.Fill = false;
		this.vbox3.Add (this.hbox3);
		global::Gtk.Box.BoxChild w24 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox3]));
		w24.Position = 5;
		w24.Expand = false;
		w24.Fill = false;
		this.hpaned1.Add (this.vbox3);
		this.vbox1.Add (this.hpaned1);
		global::Gtk.Box.BoxChild w26 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hpaned1]));
		w26.Position = 0;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hbox2 = new global::Gtk.HBox ();
		this.hbox2.Name = "hbox2";
		this.hbox2.Spacing = 6;
		// Container child hbox2.Gtk.Box+BoxChild
		this.playButton = new global::Gtk.Button ();
		this.playButton.WidthRequest = 130;
		this.playButton.HeightRequest = 40;
		this.playButton.CanFocus = true;
		this.playButton.Name = "playButton";
		this.playButton.UseUnderline = true;
		// Container child playButton.Gtk.Container+ContainerChild
		global::Gtk.Alignment w27 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w28 = new global::Gtk.HBox ();
		w28.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w29 = new global::Gtk.Image ();
		w29.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-about", global::Gtk.IconSize.Menu);
		w28.Add (w29);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w31 = new global::Gtk.Label ();
		w31.LabelProp = global::Mono.Unix.Catalog.GetString ("Play");
		w31.UseUnderline = true;
		w28.Add (w31);
		w27.Add (w28);
		this.playButton.Add (w27);
		this.hbox2.Add (this.playButton);
		global::Gtk.Box.BoxChild w35 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.playButton]));
		w35.Position = 0;
		w35.Expand = false;
		w35.Fill = false;
		w35.Padding = ((uint)(10));
		// Container child hbox2.Gtk.Box+BoxChild
		this.minecraftStatus = new global::Gtk.Label ();
		this.minecraftStatus.Name = "minecraftStatus";
		this.minecraftStatus.Xalign = 0F;
		this.minecraftStatus.LabelProp = global::Mono.Unix.Catalog.GetString ("Minecraft is not yet running.");
		this.hbox2.Add (this.minecraftStatus);
		global::Gtk.Box.BoxChild w36 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.minecraftStatus]));
		w36.Position = 1;
		this.vbox1.Add (this.hbox2);
		global::Gtk.Box.BoxChild w37 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox2]));
		w37.Position = 1;
		w37.Expand = false;
		w37.Fill = false;
		this.Add (this.vbox1);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 987;
		this.DefaultHeight = 521;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.Shown += new global::System.EventHandler (this.OnShown);
		this.ConfigureEvent += new global::Gtk.ConfigureEventHandler (this.OnShown);
		this.mapButton.Activated += new global::System.EventHandler (this.OnMapButtonActivated);
		this.mapButton.Clicked += new global::System.EventHandler (this.OnMapButtonActivated);
		this.tsButton.Activated += new global::System.EventHandler (this.OnTsButtonActivated);
		this.tsButton.Clicked += new global::System.EventHandler (this.OnTsButtonActivated);
		this.websiteButton.Activated += new global::System.EventHandler (this.OnWebsiteButtonActivated);
		this.websiteButton.Clicked += new global::System.EventHandler (this.OnWebsiteButtonActivated);
		this.wikiButton.Activated += new global::System.EventHandler (this.OnWikiButtonActivated);
		this.wikiButton.Clicked += new global::System.EventHandler (this.OnWikiButtonActivated);
		this.forumsButton.Activated += new global::System.EventHandler (this.OnForumsButtonActivated);
		this.forumsButton.Clicked += new global::System.EventHandler (this.OnForumsButtonActivated);
		this.donateButton.Activated += new global::System.EventHandler (this.OnDonateButtonActivated);
		this.donateButton.Clicked += new global::System.EventHandler (this.OnDonateButtonActivated);
		this.reportServerDownButton.Activated += new global::System.EventHandler (this.OnReportServerDownButtonActivated);
		this.reportServerDownButton.Clicked += new global::System.EventHandler (this.OnReportServerDownButtonActivated);
		this.playButton.Activated += new global::System.EventHandler (this.OnPlayButtonActivated);
		this.playButton.Clicked += new global::System.EventHandler (this.OnPlayButtonActivated);
	}
}
