using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Pfz.Collections;
using Pfz.Remoting;
using SecureChat.Common;
using System.IO;
using LumiSoft.Media.Wave;
using Pfz.Extensions.DictionaryExtensions;
using Pfz.Threading;

namespace SecureChat.Client
{
	public partial class FormMain:
		Form
	{
		private RemotingClient fConnection;
		private IServerClient fClient;
		internal static string fNickName;
		private Icon[] fIcons = new Icon[12];
		private int fIconIndex;

		internal static Dictionary<KeyValuePair<string, string>, FormWebCam> fActiveWebCams = new Dictionary<KeyValuePair<string, string>, FormWebCam>();
		
		public FormMain()
		{
			InitializeComponent();
			Disposed += p_Disposed;
			Activated += p_Activated;
			Deactivate += p_Deactivated;
			
			fIcons[0] = this.notifyIcon.Icon;
			fIcons[1] = Icons.Animation01;
			fIcons[2] = Icons.Animation02;
			fIcons[3] = Icons.Animation03;
			fIcons[4] = Icons.Animation04;
			fIcons[5] = Icons.Animation05;
			fIcons[6] = Icons.Animation06;
			fIcons[7] = Icons.Animation07;
			fIcons[8] = Icons.Animation08;
			fIcons[9] = Icons.Animation09;
			fIcons[10] = Icons.Animation10;
			fIcons[11] = Icons.Animation11;

			CreateHandle();
			var host = ConfigurationManager.AppSettings["serverHost"];
			var parameters = new RemotingParameters(host, 570);
			parameters.Cryptography = new RijndaelManaged();
			fConnection = new RemotingClient(parameters);
			fConnection.UserChannelCreated += p_UserChannelCreated;

			var server = (IServer)fConnection.InvokeStaticMethod("GetServer");

			string[] names = WindowsIdentity.GetCurrent().Name.Split('\\');
			fNickName = names[names.Length - 1];
			fClient = server.Connect(fNickName, new Client(this));
		}
		private void p_Disposed(object sender, EventArgs args)
		{
			var connection = fConnection;
			if (connection != null)
			{
				fConnection = null;
				connection.Dispose();
			}
			
			var waveOuts = fWaveOutsByClient;
			if (waveOuts != null)
			{
				lock(waveOuts)
				{
					foreach(var disposable in waveOuts.Values)
						disposable.Dispose();
						
					waveOuts.Clear();
				}
			}
		}
		
		public bool IsActive { get; private set; }
		private void p_Activated(object sender, EventArgs e)
		{
			listUsers.SelectedItems.Clear();
			IsActive = true;
			timerAnimateIcons.Enabled = false;
			notifyIcon.Icon = fIcons[0];
			fIconIndex = 0;
		}
		private void p_Deactivated(object sender, EventArgs e)
		{
			IsActive = false;
		}

		private void textMessage_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Alt)
				return;
			
			if (e.Control)
				return;
			
			if (e.KeyCode != Keys.Return)
				return;
			
			e.SuppressKeyPress = true;

			string text = textMessage.Text;
			string[] users = null;
			var usersList = listUsers.SelectedItems.OfType<string>().ToList();
			int count = usersList.Count;
			
			if ((count == 0 && listUsers.Items.Count > 2) || string.IsNullOrEmpty(text))
			{
				var oldColor = textMessage.BackColor;
				textMessage.BackColor = Color.Red;
				textMessage.Update();
				Thread.Sleep(100);
				textMessage.BackColor = oldColor;
				return;
			}
			
			if (count == 0)
			{
				foreach(string userName in listUsers.Items)
					usersList.Add(userName);
					
				count = listUsers.Items.Count;
			}
			
			for (int i=0; i<count; i++)
				usersList[i] = usersList[i].Substring(0, usersList[i].LastIndexOf(" - "));
		
			if (!usersList.Contains(fNickName))
				usersList.Add(fNickName);
			
			users = usersList.ToArray();
				
			fClient.SendMessage(users, text);
			textMessage.Clear();
		}

		private void clearToolStripMenuItem_Click(object sender, EventArgs e)
		{
			textConversation.Clear();
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason != CloseReason.UserClosing)
				Application.Exit();
			else
			{
				e.Cancel = true;
				IsActive = false;
				Hide();
			}
		}

		private void p_ShowAndActivate()
		{
			Show();

			if (WindowState == FormWindowState.Minimized)
				WindowState = FormWindowState.Normal;
			
			Activate();
		}
		private void showToolStripMenuItem_Click(object sender, EventArgs e)
		{
			p_ShowAndActivate();
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			FormClosing -= Form1_FormClosing;
			Application.Exit();
		}

		private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			p_ShowAndActivate();
		}

		private void timerAnimateIcons_Tick(object sender, EventArgs e)
		{
			fIconIndex++;
			if (fIconIndex == fIcons.Length-1)
				timerAnimateIcons.Interval = 1000;
			else
			if (fIconIndex == fIcons.Length)
				fIconIndex = 0;
			else
			if (fIconIndex == 1)
				timerAnimateIcons.Interval = 26;
			
			notifyIcon.Icon = fIcons[fIconIndex];
		}

		private void clearSelectionsendsToAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			listUsers.ClearSelected();
		}

		private void startWebCamToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// creates a copy of the userList, so the Show of the forms will
			// not cause problems when there are other events (which are processed
			// in the middle of the following foreach.
			var userList = listUsers.SelectedItems.OfType<string>().ToList();
			
			foreach(string userItem in userList)
			{
				string user = userItem.Substring(0, userItem.LastIndexOf(" - "));
				WebCamInfo[] availableWebCams;
				
				if (user == fNickName)
					availableWebCams = ActiveWebCams.ListAllWebCams();
				else
					availableWebCams = fClient.ListWebCams(user);
				
				if (availableWebCams == null)
				{
					MessageBox.Show("User " + user + " is no more connected.");
					continue;
				}
				
				WebCamInfo webCamInfo;
				switch(availableWebCams.Length)
				{
					case 0:
						MessageBox.Show("User " + user + " has no active web-cams.");
						continue;
					
					case 1:
						webCamInfo = availableWebCams[0];
						break;
					
					default:
						webCamInfo = p_SelectWebCam(availableWebCams);
						
						if (webCamInfo == null)
							continue;
						
						break;
				}
				
				string displayName = webCamInfo.DisplayName;
				string monikerName = webCamInfo.InternalName;
				FormWebCam form;
				fActiveWebCams.TryGetValue(new KeyValuePair<string, string>(user, monikerName), out form);
				if (form != null)
				{
					form.BringToFront();
					continue;
				}
				
				IFastEnumerator<byte[]> enumerator;
				try
				{
					if (user == fNickName)
					{
						enumerator = ActiveWebCams.Start(webCamInfo);
						continue;
					}
					else
						enumerator = fClient.ReceiveWebCam(user, webCamInfo);
				}
				catch
				{
					continue;
				}
				
				if (enumerator == null)
					continue;

				form = new FormWebCam(user, displayName, monikerName, enumerator);
				form.Show();
			}
		}

		private WebCamInfo p_SelectWebCam(WebCamInfo[] availableWebCams)
		{
			using(var form = new FormSelectWebCams(availableWebCams))
			{
				form.ShowDialog();
				return form.Result;
			}
		}

		private void listUsers_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				if (listUsers.SelectedItems.Count == 0)
				{
					int index = e.Y / listUsers.ItemHeight;
					if (index >= 0 && index < listUsers.Items.Count)
						listUsers.SelectedIndex = index;
				}

				p_CheckReceiveSoundCapture();
				contextMenuListUsers.Show(listUsers, e.Location);
			}
		}

		private void p_CheckReceiveSoundCapture()
		{
			var selectedItem = listUsers.SelectedItem;
			if (selectedItem == null)
			{
				receiveSourceCaptureToolStripMenuItem.Checked = false;
				receiveSourceCaptureToolStripMenuItem.Enabled = false;
				return;
			}
			
			string selectedUser = selectedItem.ToString();
			int pos = selectedUser.LastIndexOf(" - ");
			if (pos > 0)
				selectedUser = selectedUser.Substring(0, pos);

			receiveSourceCaptureToolStripMenuItem.Enabled = true;
			
			lock(fWaveOutsByClient)
				receiveSourceCaptureToolStripMenuItem.Checked = fWaveOutsByClient.ContainsKey(selectedUser);
		}

		private void listUsers_DragOver(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.None;
		
			string[] data = (string[])e.Data.GetData("FileDrop");
			if (data.Length != 1)
				return;
			
			var point = listUsers.PointToClient(new Point(e.X, e.Y));
			int selectedIndex = point.Y / listUsers.ItemHeight;
			
			if (selectedIndex < 0 || selectedIndex >= listUsers.Items.Count)
				return;
			
			listUsers.SelectedIndex = selectedIndex;
			e.Effect = DragDropEffects.Copy | DragDropEffects.Scroll;
		}

		private void listUsers_DragDrop(object sender, DragEventArgs e)
		{
			string selectedUser = listUsers.SelectedItem.ToString();
			int pos = selectedUser.LastIndexOf(" - ");
			if (pos > 0)
				selectedUser = selectedUser.Substring(0, pos);
				
			string filePath = ((string[])e.Data.GetData("FileDrop"))[0];
			

			FileStream fileStream = null;
			Stream sendStream = null;
			FormTransferFile form = null;
			try
			{
				fileStream = File.OpenRead(filePath);
				SendFileInfo sendFileInfo = new SendFileInfo(selectedUser, Path.GetFileName(filePath), fileStream.Length);

				sendStream = fConnection.CreateUserChannel(sendFileInfo);
				form = new FormTransferFile(sendFileInfo, fileStream, sendStream, true);

				form.Show();
			}
			catch(Exception exception)
			{
				if (fileStream != null)
					fileStream.Dispose();
			
				if (sendStream != null)
					sendStream.Dispose();
					
				if (form != null)
					form.Dispose();
				
				MessageBox.Show(exception.Message);
			}
		}

		private void p_UserChannelCreated(object sender, ChannelCreatedEventArgs e)
		{
			Invoke
			(
				new Action
				(
					delegate
					{
						SendFileInfo sendFileInfo = (SendFileInfo)e.Data;
						
						using(var dialog = new SaveFileDialog())
						{
							dialog.FileName = sendFileInfo.FileName;
							if (dialog.ShowDialog() != DialogResult.OK)
								return;
							
							FormTransferFile form = null;
							FileStream fileStream = null;
							try
							{
								fileStream = File.Create(dialog.FileName);
								form = new FormTransferFile(sendFileInfo, fileStream, e.Channel, false);
								form.Show();
								
								e.CanDisposeChannel = false;
							}
							catch(Exception exception)
							{
								if (fileStream != null)
									fileStream.Dispose();
								
								if (form != null)
									form.Dispose();
							
								MessageBox.Show(exception.Message);
							}
						}
					}
				)
			);
		}

		internal WaveIn fWaveIn;
		private void startSoundCaptureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (fWaveIn != null)
			{
				fWaveIn.Dispose();
				fWaveIn = null;
				startSoundCaptureToolStripMenuItem.Checked = false;
				return;
			}

			var inDevices = WaveIn.Devices;
			if (inDevices.Length == 0)
			{
				MessageBox.Show("There are no audio-input devices installed.");
				return;
			}

			WavInDevice inDevice = inDevices[0];

			WaveIn waveIn = null;
			try
			{
				waveIn = new WaveIn(inDevice, 8000, 16, 1, 400);
				startSoundCaptureToolStripMenuItem.Checked = true;
				waveIn.BufferFull += waveIn_BufferFull;

				waveIn.Start();
			}
			catch (Exception exception)
			{
				if (waveIn != null)
					waveIn.Dispose();

				MessageBox.Show(exception.Message, exception.GetType().FullName);
				return;
			}

			fWaveIn = waveIn;
		}

		void waveIn_BufferFull(byte[] buffer)
		{
			SoundEnumerator.ActualValueEnumerator.ActualValue = buffer;
		}

		private Dictionary<string, IFastEnumerator<byte[]>> fWaveOutsByClient = new Dictionary<string, IFastEnumerator<byte[]>>();
		private void receiveSoundCaptureToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var selectedItem = listUsers.SelectedItem;
			if (selectedItem == null)
				return;
				
			string selectedUser = selectedItem.ToString();
			int pos = selectedUser.LastIndexOf(" - ");
			if (pos > 0)
				selectedUser = selectedUser.Substring(0, pos);
			
			lock(fWaveOutsByClient)
			{
				var fastEnumerator = fWaveOutsByClient.GetValueOrDefault(selectedUser);
				
				if (fastEnumerator != null)
				{
					fastEnumerator.Dispose();
					fWaveOutsByClient.Remove(selectedUser);
					return;
				}
				
				try
				{
					if (selectedUser == fNickName)
						fastEnumerator = SoundEnumerator.Distributor.CreateClient();
					else
						fastEnumerator = fClient.ReceiveSound(selectedUser);
					
					if (fastEnumerator == null)
						return;

					var outDevices = WaveOut.Devices;
					if (outDevices.Length == 0)
					{
						MessageBox.Show("There are no output sound devices installed.");
						return;
					}
					
					var outDevice = outDevices[0];
					var waveOut = new WaveOut(outDevice, 8000, 16, 1);
					
					fWaveOutsByClient.Add(selectedUser, fastEnumerator);
					
					UnlimitedThreadPool.Run
					(
						() =>
						{
							try
							{
								while(true)
								{
									byte[] buffer = fastEnumerator.GetNext();
									
									if (buffer == null)
										return;
									
									waveOut.Play(buffer, 0, buffer.Length);
								}
							}
							catch
							{
							}
						}
					);
				}
				catch
				{
					if (fastEnumerator != null)
					{
						try
						{
							fastEnumerator.Dispose();
						}
						catch
						{
						}
					}
				}
				return;
			}
		}
	}
}
