using System;
using System.Net;
using System.Threading;
using Lidgren.Network;

namespace Tiler
{
	public enum NetworkRealm
	{
		Server,
		Client
	}

	public static class Networking
	{
		public const ushort DefaultPort = 1337;

		private static NetPeerConfiguration NetConfig = null;
		private static NetPeer Peer = null;
		private static NetworkRealm Realm;

		public static event EventHandler<NetIncomingMessage> ConnectionApproval;
		public static event EventHandler<NetIncomingMessage> Data;
		public static event EventHandler<NetIncomingMessage> StatusChanged;

		public static bool IsServer
		{
			get
			{
				return Realm == NetworkRealm.Server;
			}
		}

		public static bool IsClient
		{
			get
			{
				return Realm == NetworkRealm.Client;
			}
		}

		static Networking()
		{
			NetConfig = new NetPeerConfiguration("Tiler");
			//NetConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
		}

		public static void InitClient()
		{
			if (Peer != null)
				throw new Exception("Network can not be initialized more than once");

			Realm = NetworkRealm.Client;

			Peer = new NetClient(NetConfig);
			Peer.Start();
		}

		public static void InitServer(ushort port)
		{
			if (Peer != null)
				throw new Exception("Network can not be initialized more than once");

			Realm = NetworkRealm.Server;

			NetConfig.Port = port;
			Peer = new NetServer(NetConfig);
			Peer.Start();
		}

		public static void Connect(string host, ushort port)
		{
			if (!IsClient)
				throw new Exception("Networking.Connect is only available for clients");

			Peer.Connect(host, port);
		}

		public static void Connect(IPEndPoint ep)
		{
			Connect(ep.Address.ToString(), (ushort)ep.Port);
		}

		public static void Disconnect()
		{
			Disconnect("Disconnected");
		}

		public static void Disconnect(string message)
		{
			if (!IsClient)
				throw new Exception("Networking.Disconnect is only available for clients");

			(Peer as NetClient).Disconnect(message);
		}

		public static void Shutdown()
		{
			Peer.Shutdown(IsServer ? "Server Shutting Down" : "Disconnected");

			while (IsServer ? Peer.ConnectionsCount > 0 : (Peer as NetClient).ServerConnection != null)
			{
				Thread.Sleep(0);
			}
		}

		public static void Update()
		{
			NetIncomingMessage im;
			while ((im = Peer.ReadMessage()) != null)
			{
				switch (im.MessageType)
				{
				case NetIncomingMessageType.ConnectionApproval:
					ConnectionApproval?.Invoke(null, im);
					break;

				case NetIncomingMessageType.Data:
					Data?.Invoke(null, im);

					break;

				case NetIncomingMessageType.StatusChanged:
					StatusChanged?.Invoke(null, im);

					break;
				}

				Peer.Recycle(im);
			}
		}
	}
}
