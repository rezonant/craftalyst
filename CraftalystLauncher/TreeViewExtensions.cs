using System;

namespace CraftalystLauncher
{
	public static class TreeViewExtensions
	{
		public static void AddColumn(this Gtk.TreeView self, string name, int index)
		{
			var nameRenderer = new Gtk.CellRendererText();
			var nameColumn = new Gtk.TreeViewColumn(name, nameRenderer);
			nameColumn.AddAttribute(nameRenderer, "text", index);
			self.AppendColumn(nameColumn);
		}
	}
}

