using System.Collections;

using UnityEngine;
using UnityEngine.Networking;


namespace LowLevelNetworking
{
	/// <summary>
	/// The net client handles connecting to a server as well as sending and receiving messages. 
	/// </summary>
	public class NetClient : NetHost
	{
		public int mConnection = -1;
		public string mServerIP = null;
		public bool mIsConnected = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="NetClient"/> class.
		/// </summary>
		/// <param name="socket">Valid socket id for the NetClient. Given by NetManager.</param>
		public NetClient()
		{
			HostTopology ht = new HostTopology(NetManager.mConnectionConfig, 1); // Clients only need 1 connection
			int csocket = NetworkTransport.AddHost(ht);
		
			if(!NetUtils.IsSocketValid(csocket))
			{
				Debug.Log("NetManager::CreateClient() returned an invalid socket ( " + csocket + " )");
			}

			mSocket = csocket;
		}

		/// <summary>
		/// Connect the specified ip and port.
		/// </summary>
		/// <param name="ip">Ip.</param>
		/// <param name="port">Port.</param>
		public bool Connect(string ip, int port)
		{

			byte error;
			mConnection = NetworkTransport.Connect(mSocket, ip, port, 0, out error);

			if(NetUtils.IsNetworkError(error))
			{
				Debug.Log("NetClient::Connect( " + ip + " , " + port.ToString() + " ) Failed with reason '" + NetUtils.GetNetworkError(error) + "'.");
				return false;
			}

			mServerIP = ip;
			mPort = port;

			return true;
		}

		/// <summary>
		/// Disconnect the client from the server.
		/// </summary>
		public bool Disconnect()
		{

			if(!mIsConnected)
			{
				Debug.Log("NetClient::Disconnect() Failed with reason 'Not connected to server!");
				return false;
			}

			byte error;

			NetworkTransport.Disconnect(mSocket, mConnection, out error);

			if(NetUtils.IsNetworkError(error))
			{
				Debug.Log("NetClient::Disconnect() Failed with reason '" + NetUtils.GetNetworkError(error) + "'.");
				return false;
			}

			mIsConnected = false;
		
			return true;
		}

		/// <summary>
		/// Sends the stream.
		/// </summary>
		/// <returns><c>true</c>, if stream was sent, <c>false</c> otherwise.</returns>
		/// <param name="buffer">The object you wish to send as a serialized stream.</param>
		/// <param name="size">Max buffer size for your data.</param>
		/// <param name="channel">The channel to send the data on.</param>
		public bool SendStream(byte[] buffer, int size, int channel)
		{

			byte error;
			NetworkTransport.Send(mSocket, mConnection, channel, buffer, size, out error);
		
			if(NetUtils.IsNetworkError(error))
			{
				Debug.Log("NetClient::SendStream( " + buffer.ToString() + " , " + size.ToString() + " ) Failed with reason '" + NetUtils.GetNetworkError(error) + "'.");
				return false;
			}

			return true;
		}

	}
}
