using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

namespace LowLevelNetworking
{
	/// <summary>
	/// The NetServer handles client tracking, broadcasting and sending/receiving messages from clients.
	/// </summary>
	public class NetServer : NetHost
	{
	
		public bool mIsRunning = false;
	
		public List<int> mClients = new List<int>();



		/// <summary>
		/// Initializes a new instance of the <see cref="NetServer"/> class. We can simulate real world network conditions by using the simMinTimeout/simMaxTimeout params
		/// to simulate connection lag.
		/// </summary>
		/// <param name="maxConnections">Max connections.</param>
		/// <param name="port">Port.</param>
		/// <param name="simMinTimeout">Minimum lag timeout to simulate in ms. Set to zero for none.</param>
		/// <param name="simMaxTimeout">Maximum lag timeout to simulate in ms. Set to zero for none.</param> 
		/// 
		public NetServer(int maxConnections, int port, int simMinTimeout = 0, int simMaxTimeout = 0)
		{

			if(!NetManager.mIsInitialized)
			{
				Debug.Log("NetServer( ... ) - NetManager was not initialized. Did you forget to call NetManager.Init()?");
				return;
			}

			HostTopology ht = new HostTopology(NetManager.mConnectionConfig, maxConnections);

			if(simMinTimeout != 0 || simMaxTimeout != 0)
			{
				mSocket = NetworkTransport.AddHostWithSimulator(ht, simMinTimeout, simMaxTimeout, port);
			}
			else
			{
				mSocket = NetworkTransport.AddHost(ht, port);
			}

			mPort = port;

			if(!NetUtils.IsSocketValid(mSocket))
			{
				Debug.Log("NetServer::NetServer( " + maxConnections + " , " + port.ToString() + " ) returned an invalid socket ( " + mSocket.ToString() + " )");
			}
		
			mIsRunning = true;
		}

		/// <summary>
		/// Broadcast a stream to all connected clients.
		/// </summary>
		/// <param name="buffer">The bytes to send.</param>
		/// <param name="size">Size of the valid bytes to send in the buffer</param>
		/// <param name="channel">Channel to broadcast on.</param>
		public void BroadcastStream(byte[] buffer, int size, int channel)
		{
			foreach(int connId in mClients)
			{
				SendStream(buffer, size, connId, channel);
			}

			return;
	
		}

		/// <summary>
		/// Send a stream to a single connected client.
		/// </summary>
		/// <returns><c>true</c>, if stream was sent, <c>false</c> otherwise.</returns>
		/// <param name="buffer">The object to serialize and stream.</param>
		/// <param name="size">Max buffer size of object after being serialized.</param>
		/// <param name="channel">Channel to broadcast on.</param>
		/// <param name="connId">Connection ID of client to broadcast to.</param>
		public bool SendStream(byte[] buffer, int size, int connId, int channel)
		{
			byte error;

			NetworkTransport.Send(mSocket, connId, channel, buffer, size, out error);
			if(NetUtils.IsNetworkError(error))
			{
				Debug.Log("NetServer::SendStream( " + buffer.ToString() + " , " + size.ToString() + " ) Failed with reason '" + NetUtils.GetNetworkError(error) + "'.");
				return false;
			}

			return true;

		}

		/// <summary>
		/// Disconnects a client via specified id.
		/// </summary>
		/// <returns><c>true</c>, if client was disconnected, <c>false</c> otherwise.</returns>
		public bool DisconnectClient(int connId)
		{
			if(!HasClient(connId))
			{
				Debug.Log("NetServer::DisconnectClient( " + connId + " ) Failed with reason 'Client with id does not exist!'");
				return false;
			}

			byte error;
			NetworkTransport.Disconnect(mSocket, connId, out error);

			if(NetUtils.IsNetworkError(error))
			{
				Debug.Log("NetServer::DisconnectClient( " + connId + " ) Failed with reason '" + NetUtils.GetNetworkError(error) + "'.");
				return false;
			}

			return true;

		}

		/// <summary>
		/// Disconnects all clients.
		/// </summary>
		public void DisconnectAllClients()
		{

			foreach(int c in mClients)
			{
				DisconnectClient(c);
			}
		}


		/// <summary>
		/// Adds a client connection to the server after making sure it will be unique. 	
		/// </summary>
		/// <returns><c>true</c>, if client was added, <c>false</c> otherwise.</returns>
		/// <param name="connId">Connection ID</param>
		public bool AddClient(int connId)
		{
			if(mClients.Contains(connId))
			{
				Debug.Log("NetServer::AddClient( " + connId.ToString() + " ) - Id already exists!");
				return false;
			}

			mClients.Add(connId);
			return true;
		}

		/// <summary>
		/// Removes a client connection from the server after making sure that it exists.
		/// </summary>
		/// <returns><c>true</c>, if client was removed, <c>false</c> otherwise.</returns>
		/// <param name="connId">Connection identifier.</param>
		public bool RemoveClient(int connId)
		{
			if(!mClients.Exists(element => element == connId))
			{
				Debug.Log("NetServer::RemoveClient( " + connId.ToString() + " ) - Client not connected!");
				return false;
			}

			mClients.Remove(connId);
			return true;
		}

		/// <summary>
		/// Determines whether this instance has a client of the specified connId.
		/// </summary>
		/// <returns><c>true</c> if this instance has a client of the specified connId; otherwise, <c>false</c>.</returns>
		/// <param name="connId">Conn identifier.</param>
		public bool HasClient(int connId)
		{
			return mClients.Exists(element => element == connId);
		}
	}
}