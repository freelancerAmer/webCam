using System;
using System.IO;
using System.Windows.Forms;
using Pfz.Threading;
using SecureChat.Common;
using Pfz.Extensions.StreamExtensions;

namespace SecureChat.Client
{
	public partial class FormTransferFile:
		Form
	{
		private SendFileInfo fSendFileInfo;
		private FileStream fFileStream;
		private Stream fSendOrReceiveStream;
		private bool fMustSend;

		public FormTransferFile(SendFileInfo sendFileInfo, FileStream fileStream, Stream sendOrReceiveStream, bool mustSend)
		{
			InitializeComponent();
			
			fSendFileInfo = sendFileInfo;
			fFileStream = fileStream;
			fSendOrReceiveStream = sendOrReceiveStream;
			fMustSend = mustSend;
			
			if (mustSend)
				Text = "Waiting response to send " + sendFileInfo.FileName;
			else
				Text = "Receiving " + sendFileInfo.FileName;
			
			fMaximum = progressBar1.Width;
			fFileLength = sendFileInfo.Length;
			progressBar1.Maximum = fMaximum;
			
			Disposed += new EventHandler(p_Disposed);
		}
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			if (fMustSend)
				UnlimitedThreadPool.Run(p_Send);
			else
				UnlimitedThreadPool.Run(p_Receive);
		}

		void p_Disposed(object sender, EventArgs e)
		{
			if (fSendOrReceiveStream != null)
				fSendOrReceiveStream.Dispose();
			
			if (fFileStream != null)
				fFileStream.Dispose();
		}
		
		private int fMaximum;
		private int fLastPosition = 0;
		private long fFileLength;
		private void p_Send()
		{
			try
			{
				fSendOrReceiveStream.ReadByte();
			}
			catch
			{
				try
				{
					Invoke
					(
						new Action
						(
							() => Text = fSendFileInfo.FileName + ": The other side can't receive this file."
						)
					);
				}
				catch
				{
				}
				
				return;
			}
			
			try
			{
				Invoke
				(
					new Action
					(
						delegate
						{
							Text = "Sending file " + fSendFileInfo.FileName;
						}
					)
				);

				long expectedLength = fSendFileInfo.Length;
				
				byte[] buffer;
				if (expectedLength < 8 * 1024)
					buffer = new byte[(int)expectedLength];
				else
					buffer = new byte[8 * 1024];
				
				long pos = 0;
				while(pos < expectedLength)
				{
					int mustSend = 8 * 1024;
					long diff = expectedLength - pos;
					if (diff < mustSend)
						mustSend = (int)diff;
						
					fFileStream.FullRead(buffer, 0, mustSend);
					
					fSendOrReceiveStream.Write(buffer, 0, mustSend);
					fSendOrReceiveStream.Flush();
					fSendOrReceiveStream.ReadByte();
					pos += mustSend;
					
					int position = (int)(pos * fMaximum / fFileLength);
					if (position != fLastPosition)
					{
						fLastPosition = position;
						
						BeginInvoke
						(
							new Action
							(
								delegate
								{
									progressBar1.Value = position;
								}
							)
						);
					}
				}
				
				Invoke
				(
					new Action
					(
						delegate
						{
							Text = "Finished " + fSendFileInfo.FileName;
							progressBar1.Value = progressBar1.Maximum;
							
							fFileStream.Dispose();
							fSendOrReceiveStream.Dispose();
						}
					)
				);
			}
			catch(Exception exception)
			{
				try
				{
					BeginInvoke
					(
						new Action
						(
							() => Text = fSendFileInfo.FileName + ": " + exception.Message
						)
					);
				}
				catch
				{
				}
			}
		}
		
		private void p_Receive()
		{
			try
			{
				// signals that we are starting to receive the file.
				fSendOrReceiveStream.WriteByte(0);
				fSendOrReceiveStream.Flush();

				long expectedLength = fSendFileInfo.Length;

				byte[] buffer;
				if (expectedLength < 8 * 1024)
					buffer = new byte[(int)expectedLength];
				else
					buffer = new byte[8 * 1024];
					
				long pos = 0;
				while(pos < expectedLength)
				{
					int mustReceive = 8 * 1024;
					long diff = expectedLength - pos;
					if (diff < mustReceive)
						mustReceive = (int)diff;

					int partialPos = 0;
					while(partialPos < mustReceive)
					{
						int read = fSendOrReceiveStream.Read(buffer, 0, mustReceive-partialPos);
						if (read <= 0)
							throw new ApplicationException("Connection closed from the other side.");
						
						fFileStream.Write(buffer, 0, read);
						partialPos += read;
						pos += read;

						int position = (int)(pos * fMaximum / fFileLength);
						if (position != fLastPosition)
						{
							fLastPosition = position;
							BeginInvoke
							(
								new Action
								(
									delegate
									{
										progressBar1.Value = position;
									}
								)
							);
						}
					}
					fSendOrReceiveStream.WriteByte(0);
					fSendOrReceiveStream.Flush();
				}

				fSendOrReceiveStream.WriteByte(0);
				fSendOrReceiveStream.Flush();

				Invoke
				(
					new Action
					(
						delegate
						{
							Text = "Finished " + fSendFileInfo.FileName;
							progressBar1.Value = progressBar1.Maximum;

							fFileStream.Dispose();
							fSendOrReceiveStream.Dispose();
						}
					)
				);
			}
			catch(Exception exception)
			{
				try
				{
					BeginInvoke
					(
						new Action
						(
							() => Text = fSendFileInfo.FileName + ": " + exception.Message
						)
					);
				}
				catch
				{
				}
			}
		}
	}
}
