using System;
using System.Collections.Generic;
using Pfz.Collections;
using Pfz.Extensions.DictionaryExtensions;
using SecureChat.Common;

namespace SecureChat.Server
{
	public sealed class ServerClient:
		IServerClient
	{
		internal IClient fClient;

		public ServerClient(string address, string nickName, IClient client)
		{
			Address = address;
			NickName = nickName;
			fClient = client;
		}
		
		public string Address { get; private set; }
		public string NickName { get; private set; }

		public string SendMessage(string[] toNickNames, string message)
		{
			var server = Server.GetInstance();
			
			List<ServerClient> clients;
			lock(server.fClients)
			{
				if (toNickNames == null)
					clients = new List<ServerClient>(server.fClients.Values);
				else
				{
					clients = new List<ServerClient>(toNickNames.Length);
					foreach(var name in toNickNames)
					{
						var client = server.fClients.GetValueOrDefault(name);
						if (client != null)
							clients.Add(client);
					}
				}
			}
			
			foreach(var item in clients)
			{
				try
				{
					item.fClient.MessageReceived(Address, NickName, message);
				}
				catch
				{
				}
			}
			
			return null;
		}
		public WebCamInfo[] ListWebCams(string nickName)
		{
			var server = Server.GetInstance();
			
			ServerClient otherClient;
			lock(server.fClients)
				otherClient = server.fClients.GetValueOrDefault(nickName);
			
			if (otherClient == null)
				return null;
				
			return otherClient.fClient.ListWebCams();
		}
		
		private Dictionary<WebCamInfo, EnumeratorDistributor<byte[]>> fWebCamDistributors = new Dictionary<WebCamInfo,EnumeratorDistributor<byte[]>>();
		public IFastEnumerator<byte[]> ReceiveWebCam(string fromNickName, WebCamInfo webCamInfo)
		{
			if (fromNickName == null)
				return null;

			var server = Server.GetInstance();
			while (true)
			{
				try
				{
					ServerClient webCamSenderClient;
					lock(server.fClients)
						server.fClients.TryGetValue(fromNickName, out webCamSenderClient);
						
					if (webCamSenderClient == null)
						return null;

					EnumeratorDistributor<byte[]> result;
					lock(fWebCamDistributors)
					{
						result = fWebCamDistributors.GetValueOrDefault(webCamInfo);
						if (result == null || result.WasDisposed)
						{
							var enumeratorResult = webCamSenderClient.fClient.GetWebCamEnumerator(webCamInfo);
							if (enumeratorResult == null)
								return null;
								
							result = new EnumeratorDistributor<byte[]>(enumeratorResult);
							fWebCamDistributors[webCamInfo] = result;
						}
					}

					return result.CreateClient();
				}
				catch(ObjectDisposedException)
				{
				}
			}
		}

		public IFastEnumerator<byte[]> ReceiveSound(string nickName)
		{
			var server = Server.GetInstance();
			
			ServerClient otherClient;
			lock(server.fClients)
				otherClient = server.fClients.GetValueOrDefault(nickName);
			
			if (otherClient == null)
				return null;
			
			return otherClient.EnumerateSound();
		}

		private EnumeratorDistributor<byte[]> fSoundEnumerator;
		private object fSoundEnumeratorLock = new object();
		private IFastEnumerator<byte[]> EnumerateSound()
		{
			if (fSoundEnumerator == null || fSoundEnumerator.WasDisposed)
				lock(fSoundEnumeratorLock)
					if (fSoundEnumerator == null || fSoundEnumerator.WasDisposed)
						fSoundEnumerator = new EnumeratorDistributor<byte[]>(fClient.GetSoundEnumerator());
			
			return fSoundEnumerator.CreateClient();
		}
	}
}
