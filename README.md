# Unet LLAPI Wrapper

This is a small wrapper for the Low Level API functionality in the Unet release for Unity3D 5.6.x

Usage

Just drop the Net folder into your project and make the calls required as shown below. Note, this class exists in the `~Examples` folder.


```
using System.Text;

using UnityEngine;
using UnityEngine.Networking;

namespace LowLevelNetworking
{
	public class NetworkingTest : MonoBehaviour
	{

		NetServer mServer;
		NetClient mClient;

		void Start () {

			var config = new ConnectionConfig();
			config.AddChannel(QosType.Reliable);

			NetManager.mConnectionConfig = config;

			NetManager.Init ();

			mServer = NetManager.CreateServer( 10 , 7777 );
			mClient = NetManager.CreateClient ();
			mClient.Connect( "127.0.0.1" , 7777 );

			// Server Event Callbacks
			mServer.OnConnection = ServerConnection;
			mServer.OnData = ServerData;
			mServer.OnDisconnection = ServerDisconnect;

			// Client Event Callbacks
			mClient.OnConnection = ClientConnection;
			mClient.OnData = ClientData;
			mClient.OnDisconnection = ClientDisconnect;

		}

		void Update() {
			NetManager.PollEvents();
		}

		public void ServerConnection( int connectionId , int channelId , byte[] buffer , int datasize ){
			Debug.Log ( "Server: Connection to " + connectionId.ToString () );
		}

		public void ServerData( int connectionId , int channelId , byte[] buffer , int datasize ){
			string msg = Encoding.ASCII.GetString(buffer, 0, datasize);
			Debug.Log ("Server: Received message from " + connectionId.ToString () + " : " + msg );
			var bytes = System.Text.Encoding.ASCII.GetBytes("Server says hello");
			mServer.BroadcastStream(bytes, bytes.Length, channelId );
		}

		public void ServerDisconnect( int connectionId , int channelId , byte[] buffer , int datasize ){
			Debug.Log ("Server: User disconnected from server! " + connectionId);
		}

		public void ClientConnection( int connectionId , int channelId , byte[] buffer , int datasize ){
			Debug.Log ( "Client: Connection to " + connectionId.ToString () );
			var bytes = Encoding.ASCII.GetBytes("TestMessage!");
			mClient.SendStream(bytes, bytes.Length, 0);
		}

		public void ClientData( int connectionId , int channelId , byte[] buffer , int datasize ){
			var msg = Encoding.ASCII.GetString(buffer, 0, datasize);
			Debug.Log ("Client: Data Received! " + msg);
			mClient.Disconnect();
		}

		public void ClientDisconnect( int connectionId , int channelId , byte[] buffer , int datasize ){
			Debug.Log ("Client: Disconnected from server! " + connectionId);
		}
	}
}



```

