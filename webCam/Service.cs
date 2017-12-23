using System.ServiceProcess;

namespace SecureChat.Server
{
	public partial class service: ServiceBase
	{
		public service()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			Server.Start();
		}

		protected override void OnStop()
		{
			Server.Stop();
		}
	}
}
