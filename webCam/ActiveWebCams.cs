using System.Collections.Generic;
using Pfz.Collections;
using Pfz;
using SecureChat.Common;
using AForge.Video.DirectShow;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using AForge.Video;

namespace SecureChat.Client
{
	public static class ActiveWebCams
	{
		internal static Dictionary<string, FormWebCam> fDictionary =
			new Dictionary<string, FormWebCam>();
		
		public static EnumeratorDistributorClient<byte[]> Start(WebCamInfo webCamInfo)
		{
			string monikerName = webCamInfo.InternalName;
		
			EnumeratorDistributor<byte[]> distributor;
			
			lock(fDictionary)
			{
				FormWebCam form;
				if (fDictionary.TryGetValue(monikerName, out form))
					distributor = form.Distributor;
				else
				{
					IFastEnumerator<byte[]> realEnumerator;
					
					switch(monikerName)
					{
						case "Fake_Black":
							realEnumerator = new FakeWebCamEnumerator(Color.Black);
							break;
						
						case "Fake_Blue":
							realEnumerator = new FakeWebCamEnumerator(Color.Blue);
							break;
						
						default:
							realEnumerator = new WebCamEnumerator(monikerName);
							break;
					}
						
					distributor = new EnumeratorDistributor<byte[]>(realEnumerator);
					form = new FormWebCam(FormMain.fNickName, webCamInfo.DisplayName, monikerName, distributor);
					fDictionary.Add(monikerName, form);
					form.Show();
				}
			}
			
			return distributor.CreateClient();
		}
		public static void Stop(string monikerName)
		{
			lock(fDictionary)
			{
				FormWebCam result;
				if (fDictionary.TryGetValue(monikerName, out result))
				{
					fDictionary.Remove(monikerName);
					result.Dispose();
				}
			}
		}
		public static void Stop()
		{
			lock(fDictionary)
				foreach(var value in fDictionary.Values)
					value.Dispose();
		}

		public static WebCamInfo[] ListAllWebCams()
		{
			var videoInputDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

			int count = videoInputDevices.Count;
			WebCamInfo[] result = new WebCamInfo[count+2];
			for (int i = 0; i < count; i++)
			{
				var videoInputDevice = videoInputDevices[i];
				result[i] = new WebCamInfo(videoInputDevice.Name, videoInputDevice.MonikerString);
			}
			result[count] = new WebCamInfo("Fake WebCam - Only generates a frame count for testing - Black background", "Fake_Black");
			result[count + 1] = new WebCamInfo("Fake WebCam - Only generates a frame count for testing - Blue background", "Fake_Blue");

			return result;
		}

		internal static IFastEnumerator<byte[]> GetWebCamEnumerator(WebCamInfo webCamInfo)
		{
			lock(fDictionary)
			{
				FormWebCam form;
				if (!fDictionary.TryGetValue(webCamInfo.InternalName, out form))
					return null;
				
				var result = form.Distributor.CreateClient();
				return result;
			}
		}
	}
}
