using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace LowLevelNetworking
{

	public delegate void NetMessageHandler(NetworkEventType net, int connectionId, int channelId, byte[] buffer, int dataSize);
	public delegate void NetEventHandler(int connectionId, int channelId, byte[] buffer, int dataSize);

	public abstract class NetHost
	{
		
		public int mSocket = -1;
		public int mPort = -1;

		/// <summary>
		/// Our delegate that gets called to handle processing data 	
		/// </summary>
		public NetMessageHandler OnMessage = null;

		public NetEventHandler OnConnection = null;
		public NetEventHandler OnData = null;
		public NetEventHandler OnDisconnection = null;
	}
}