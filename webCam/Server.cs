using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using Pfz.Extensions.DictionaryExtensions;
using Pfz.Remoting;
using Pfz.Threading;
using SecureChat.Common;

namespace SecureChat.Server
{
	public sealed class Server:
		IServer
	{
		private static Server fInstance = new Server();
		public static Server GetInstance()
		{
			return fInstance;
		}
		
		private RemotingServer fServer;
		public static void Start()
		{
			GC.KeepAlive(GetInstance());
		}
		
		public Server()
		{
			fServer = new RemotingServer();
			fServer.RegisterStaticMethod("GetServer", typeof(Server).GetMethod("GetInstance"));
			fServer.CryptographyMode = CryptographyMode.Required;
			fServer.RegisterAcceptedCryptography<RijndaelManaged>();
			fServer.MustUseAsyncVoidCalls = true;

			fServer.UserChannelCreated += p_UserChannelCreated;
			
			UnlimitedThreadPool.Run
			(
				delegate
				{
					try
					{
						fServer.Run(570);
					}
					catch
					{
					}
				}
			);
		}

		public static void Stop()
		{
			var server = fInstance.fServer;
			if (server != null)
			{
				fInstance.fServer = null;
				server.Dispose();
			}
		}

		internal Dictionary<string, ServerClient> fClients = new Dictionary<string, ServerClient>();
		public IServerClient Connect(string nickName, IClient client)
		{
			if (nickName == null)
				throw new ArgumentNullException("nickName");
			
			if (client == null)
				throw new ArgumentNullException("client");
			
			RemotingClient remotingClient = RemotingClient.GetFromRemoteObject(client);
			if (remotingClient == null)
				throw new ApplicationException("The client is not remote... why?");
				
			TcpClient tcpClient = (TcpClient)remotingClient.BaseConnection;
			string address = tcpClient.Client.RemoteEndPoint.ToString();
			
			ServerClient oldClient;
			lock(fClients)
				oldClient = fClients.GetValueOrDefault(nickName);

			if (oldClient == null)
				return p_Finish(remotingClient, address, nickName, client);
			
			try
			{
				oldClient.fClient.CheckConnection();
			}
			catch(ObjectDisposedException)
			{
				return p_Finish(remotingClient, address, nickName, client);
			}
			
			throw new Exception("Another client with the same nick-name is connected.");
		}

		private ServerClient p_Finish(RemotingClient remotingClient, string address, string nickName, IClient client)
		{
			var result = new ServerClient(address, nickName, client);

			List<KeyValuePair<string, ServerClient>> list;
			lock(fClients)
			{
				fClients[nickName] = result;
				list = new List<KeyValuePair<string, ServerClient>>(fClients);
			}
			
			foreach(var pair in list)
			{
				try
				{
					var itemNickName = pair.Key;
					var itemClient = pair.Value;
					
					itemClient.fClient.UserConnected(address, nickName);
					client.UserConnected(p_GetAddress(itemClient.fClient), itemNickName);
				}
				catch
				{
				}
			}
			
			remotingClient.Disposed += delegate
				{
					List<ServerClient> list2;
					lock(fClients)
					{
						IDictionary<string, ServerClient> dictionary = fClients;
						dictionary.Remove(new KeyValuePair<string, ServerClient>(nickName, result));
						list2 = new List<ServerClient>(fClients.Values);
					}
					
					foreach(var item in list2)
					{
						try
						{
							item.fClient.UserDisconnected(address, nickName);
						}
						catch
						{
						}
					}
				};

			return result;
		}
		
		private string p_GetAddress(object remoteObject)
		{
			RemotingClient remotingClient = RemotingClient.GetFromRemoteObject(remoteObject);
			if (remotingClient == null)
				throw new ApplicationException("The client is not remote... why?");
				
			TcpClient client = (TcpClient)remotingClient.BaseConnection;
			string address = client.Client.RemoteEndPoint.ToString();
			return address;
		}

		private void p_UserChannelCreated(object sender, ChannelCreatedEventArgs e)
		{
			e.CanDisposeChannel = false;
			SendFileInfo info = (SendFileInfo)e.Data;
			
			ServerClient otherClient;
			lock(fClients)
				fClients.TryGetValue(info.To, out otherClient);
			
			if (otherClient == null)
				return;
				
			var otherRemotingClient = RemotingClient.GetFromRemoteObject(otherClient.fClient);
			using(var otherChannel = otherRemotingClient.CreateUserChannel(info))
			{
				UnlimitedThreadPool.Run
				(
					delegate
					{
						try
						{
							while(true)
							{
								int b = otherChannel.ReadByte();
								if (b == -1)
								{
									e.Channel.Dispose();
									return;
								}
								
								e.Channel.WriteByte((byte)b);
								e.Channel.Flush();
							}
						}
						catch
						{
							otherChannel.Dispose();
							e.Channel.Dispose();
						}
					}
				);
				
				byte[] buffer = new byte[8 * 1024];
				while(true)
				{
					int read = e.Channel.Read(buffer, 0, buffer.Length);
					if (read <= 0)
						return;
					
					otherChannel.Write(buffer, 0, read);
					otherChannel.Flush();
				}
			}
		}
	}
}
