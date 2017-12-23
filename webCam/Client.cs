using System;
using System.Drawing;
using Pfz.Collections;
using SecureChat.Common;
using System.Threading;

namespace SecureChat.Client
{
	public sealed class Client:
		IClient
	{
		private FormMain fForm;
		public Client(FormMain form)
		{
			fForm = form;
		}
	
		public void MessageReceived(string fromAddress, string fromNickName, string message)
		{
			fForm.Invoke
			(
				new Action
				(
					delegate
					{
						fForm.textConversation.Select(fForm.textConversation.TextLength, 0);
						fForm.textConversation.SelectionColor = Color.Brown;
						fForm.textConversation.AppendText(fromNickName);

						fForm.textConversation.Select(fForm.textConversation.TextLength, 0);
						fForm.textConversation.SelectionColor = Color.Black;
						fForm.textConversation.AppendText(" - ");
						
						fForm.textConversation.Select(fForm.textConversation.TextLength, 0);
						fForm.textConversation.SelectionColor = Color.Blue;
						fForm.textConversation.AppendText(fromAddress);

						fForm.textConversation.Select(fForm.textConversation.TextLength, 0);
						fForm.textConversation.SelectionColor = Color.Black;
						fForm.textConversation.AppendText(":\r\n" + message + "\r\n\r\n");

						fForm.textConversation.Select(fForm.textConversation.TextLength, 0);
						fForm.textConversation.ScrollToCaret();
						
						if (!fForm.IsActive)
							fForm.timerAnimateIcons.Enabled = true;
					}
				)
			);
		}

		public void UserConnected(string fromAddress, string fromNickName)
		{
			fForm.Invoke
			(
				new Action
				(
					delegate
					{
						fForm.listUsers.Items.Remove(fromNickName + " - " + fromAddress);
						fForm.listUsers.Items.Add(fromNickName + " - " + fromAddress);
					}
				)
			);
		}

		public void UserDisconnected(string fromAddress, string fromNickName)
		{
			fForm.Invoke
			(
				new Action
				(
					delegate
					{
						fForm.listUsers.Items.Remove(fromNickName + " - " + fromAddress);
					}
				)
			);
		}

		public string CheckConnection()
		{
			// Does nothing.
			// If the call returns, the connection is ok.
			return null;
		}
		
		public IFastEnumerator<byte[]> GetWebCamEnumerator(WebCamInfo webCamInfo)
		{
			return ActiveWebCams.GetWebCamEnumerator(webCamInfo);
		}

		public WebCamInfo[] ListWebCams()
		{
			WebCamInfo[] result = null;
			
			fForm.Invoke
			(
				new Action
				(
					delegate
					{
						lock(ActiveWebCams.fDictionary)
						{
							int count = ActiveWebCams.fDictionary.Count;
							int index = -1;
							result = new WebCamInfo[count];
							foreach(var form in ActiveWebCams.fDictionary.Values)
							{
								index++;
								result[index] = new WebCamInfo(form.DisplayName, form.MonikerName);
							}
						}
					}
				)
			);
			
			return result;
		}

		public IFastEnumerator<byte[]> GetSoundEnumerator()
		{
			return SoundEnumerator.Distributor.CreateClient();
		}
	}
}
