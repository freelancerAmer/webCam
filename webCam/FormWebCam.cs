using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Pfz.Collections;
using Pfz.Threading;
using System.Collections.Generic;

namespace SecureChat.Client
{
	public partial class FormWebCam: Form
	{
		private volatile IFastEnumerator<byte[]> fEnumerator;
		private volatile Image fImage;
		private string fUser;
		
		public FormWebCam(string user, string displayName, string monikerName, EnumeratorDistributor<byte[]> distributor)
		{
			InitializeComponent();
			
			Distributor = distributor;
			var enumerator = distributor.CreateClient();
			p_Initialize(user, displayName, monikerName, enumerator);
		}
		public FormWebCam(string user, string displayName, string monikerName, IFastEnumerator<byte[]> enumerator)
		{
			InitializeComponent();

			p_Initialize(user, displayName, monikerName, enumerator);
		}

		private void p_Initialize(string user, string displayName, string monikerName, IFastEnumerator<byte[]> enumerator)
		{
			ClientSize = new Size(160, 120);

			DisplayName = displayName;
			MonikerName = monikerName;
			
			fUser = user;
			fEnumerator = enumerator;
			Text = user;

			FormMain.fActiveWebCams.Add(new KeyValuePair<string, string>(fUser, MonikerName), this);
			
			Disposed += p_Disposed;

			UnlimitedThreadPool.Run(p_Receive);
		}
		private void p_Disposed(object sender, EventArgs e)
		{
			FormMain.fActiveWebCams.Remove(new KeyValuePair<string, string>(fUser, MonikerName));
			
			var distributor = Distributor;
			if (distributor != null)
			{
				Distributor = null;
				ActiveWebCams.Stop(MonikerName);
				distributor.Dispose();
			}
		
			var enumerator = fEnumerator;
			if (enumerator != null)
			{
				fEnumerator = null;
				
				try
				{
					enumerator.Dispose();
				}
				catch
				{
				}
			}
			
			base.OnClosed(e);
		}
		
		public EnumeratorDistributor<byte[]> Distributor { get; private set; }
		public string DisplayName { get; private set; }
		public string MonikerName { get; private set; }
		
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			
			var image = fImage;
			if (image != null)
				e.Graphics.DrawImage(image, 0, 0);
		}
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			if (fImage == null)
				base.OnPaintBackground(e);
		}
		private void p_Receive()
		{
			while(true)
			{
				try
				{
					var enumerator = fEnumerator;
					if (enumerator == null)
						break;
						
					var image = enumerator.GetNext();
					if (image == null)
						break;
					
					using(var stream = new MemoryStream(image))
						fImage = Image.FromStream(stream);
				
					Invalidate();
				}
				catch
				{
					return;
				}
			}
		}
	}
}
