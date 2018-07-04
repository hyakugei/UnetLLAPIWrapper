# Unet LLAPI Wrapper

This is a small wrapper for the Low Level API functionality in the Unet release for Unity3D 5.1

Usage

Just drop the Net folder into your project and make the calls required as shown below:

```
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkTest : MonoBehaviour {
	
	NetServer mServer;
	NetClient mClient;


	// Use this for initialization
	void Start () {

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
		Debug.Log ("Received message from " + connectionId.ToString () + " : " + mServer.ReceiveStream( buffer ).ToString () );
		mServer.SendStream( "Server says hello" , 1024 , connectionId , channelId );
	}

	public void ServerDisconnect( int connectionId , int channelId , byte[] buffer , int datasize ){
		Debug.Log ("Client: User disconnected from server! ");
	}

	public void ClientConnection( int connectionId , int channelId , byte[] buffer , int datasize ){
		Debug.Log ( "Client: Connection to " + connectionId.ToString () );
		mClient.SendStream(32 + " TestMessage!" , 1024);
	}
	
	public void ClientData( int connectionId , int channelId , byte[] buffer , int datasize ){
		Debug.Log ("Client: Data Received! " + mClient.ReceiveStream (buffer));
	}
	
	public void ClientDisconnect( int connectionId , int channelId , byte[] buffer , int datasize ){
		Debug.Log ("Client: User disconnected from server! ");
	}
}



```

