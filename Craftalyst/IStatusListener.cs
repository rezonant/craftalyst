using System;

namespace Craftalyst
{
	public interface IStatusListener
	{
		void Log(string message, params object[] format);
		void SetTitle(string title);
		void SetProgress(double progress);
		void SetStatus(string status);
	}
}

