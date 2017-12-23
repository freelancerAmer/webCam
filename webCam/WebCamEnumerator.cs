using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AForge.Video;
using AForge.Video.DirectShow;
using Pfz.Collections;

namespace SecureChat.Client
{
	public sealed class WebCamEnumerator:
		ActualValueEnumerator<byte[]>
	{
		private volatile VideoCaptureDevice fDevice;
		public WebCamEnumerator(string monikerString)
		{
			var device = new VideoCaptureDevice(monikerString);
			fDevice = device;
			device.DesiredFrameSize = new Size(160, 120);
			device.NewFrame += p_NewFrame;
			device.Start();
		}
		protected override void Dispose(bool disposing)
		{
			var device = fDevice;
			if (device != null)
			{
				fDevice = null;
				device.SignalToStop();
				device.WaitForStop();
			}

 			 base.Dispose(disposing);
		}

		private void p_NewFrame(object sender, NewFrameEventArgs eventArgs)
		{
			var image = (Bitmap)eventArgs.Frame;
			
			using(var stream = new MemoryStream())
			{
				image.Save(stream, ImageFormat.Jpeg);
				ActualValue = stream.ToArray();
			}
		}
	}
}
