using System;
using System.Windows.Forms;

namespace SecureChat.Client
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			try
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				using (var form = new FormMain())
					Application.Run();
			}
			finally
			{
				ActiveWebCams.Stop();
				SoundEnumerator.Stop();
			}
		}
	}
}
