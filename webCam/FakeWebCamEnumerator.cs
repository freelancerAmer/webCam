using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using Pfz.Collections;
using Pfz.Threading;

namespace SecureChat.Client
{
	public sealed class FakeWebCamEnumerator:
		ActualValueEnumerator<byte[]>
	{
		private volatile Bitmap fImage = new Bitmap(160, 120);
		private int fCount;
		
		public FakeWebCamEnumerator(Color backgroundColor)
		{
			BackgroundColor = backgroundColor;
			UnlimitedThreadPool.Run
			(
				() =>
				{
					while(true)
					{
						var image = fImage;
						if (fImage == null)
							return;

						fCount++;
						using(var font = new Font(FontFamily.GenericSerif, 26, FontStyle.Bold))
						{
							using(var graphics = Graphics.FromImage(image))
							{
								graphics.Clear(BackgroundColor);
								graphics.DrawString(fCount.ToString(), font, Brushes.Red, 0, 0);
							}
						}
						
						using(var stream = new MemoryStream())
						{
							image.Save(stream, ImageFormat.Jpeg);
							ActualValue = stream.ToArray();
						}
						
						// 30 frames per second.
						Thread.Sleep(1000/30);
					}
				}
			);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				fImage = null;
				
			base.Dispose(disposing);
		}
		
		public Color BackgroundColor { get; private set; }
	}
}
