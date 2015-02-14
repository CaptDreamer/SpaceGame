using UnityEngine;
using System.Collections;

/// <summary>
/// This script wraps photons connect functionality and defines several of the callbacks Photon invokes
/// </summary>
public class MultiplayerConnector : MonoBehaviour
{
	public static bool IsHost = false;
	public static bool QuitOnLogout = false;
	
	public static bool IsConnected
	{
		get
		{
			return PhotonNetwork.offlineMode == false && PhotonNetwork.connectionState == ConnectionState.Connected;
		}
	}

	static TypedLobby m_Lobby;
	public static TypedLobby Lobby
	{
		get
		{
			if( m_Lobby == null )
			{
				m_Lobby = new TypedLobby( "GeneralLobby", LobbyType.Default );
			}
			
			return m_Lobby;
		}
	}
	
	void Start()
	{
		DontDestroyOnLoad( gameObject );
	}
	
	/// <summary>
	/// Call this to start the connection process to the Photon Cloud
	/// </summary>
	public void Connect()
	{
		//If we are in any other state than disconnected, that means we are either already
		//connected, or in the process of connecting. So we don't want to do it again
		if( PhotonNetwork.connectionState != ConnectionState.Disconnected )
		{
			return;
		}
		
		//This tells Photon to make sure all players - which are in the same room - always load the same scene
		PhotonNetwork.automaticallySyncScene = true;
		
		//Don't join the default lobby on start, we do this ourselves in OnConnectedToMaster()
		PhotonNetwork.autoJoinLobby = false;
		
		try
		{
			PhotonNetwork.ConnectUsingSettings( "1.0" );
		}
		catch
		{
			Debug.LogWarning( "Couldn't connect to server" );
		}
	}
	
	/// <summary>
	/// For when you are done with Photon
	/// </summary>
	public void Disconnect()
	{
		if( PhotonNetwork.connected == false )
		{
			return;
		}
		
		PhotonNetwork.Disconnect();
	}
	
	/// <summary>
	/// Called when we are connected to Photon
	/// </summary>
	void OnConnectedToPhoton()
	{
		if( IsHost == true )
		{
			return;
		}
		
		PhotonNetwork.playerName = "Player" + Random.Range(1,1000);
	}
	
	/// <summary>
	/// Called when we are connected to Photons Master Server
	/// </summary>
	void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby (Lobby);
	}

	/// <summary>
	/// When we joined the lobby after connecting to Photon, we want to immediately join the demo room, or create it if it doesn't exist
	/// </summary>
	void OnJoinedLobby()
	{
		if( IsHost == true )
		{
			return;
		}
		
		if( QuitOnLogout == true )
		{
			Application.Quit();
			return;
		}
		
		if( Application.loadedLevelName == "MainMenu" )
		{
			RoomOptions roomOptions = new RoomOptions();			
			roomOptions.customRoomProperties = new ExitGames.Client.Photon.Hashtable();
		
			PhotonNetwork.JoinOrCreateRoom ("TestRoom", roomOptions, TypedLobby.Default);
		}
		else
		{
			//If we join the lobby while not being in the MainMenu scene, something went wrong. We disconnect from Photon and go back to the main menu
			PhotonNetwork.Disconnect();
			Application.LoadLevel( "MainMenu" );
		}
	}


//		
//		if( Application.loadedLevelName == "MainMenu" )
//		{
//			Application.LoadLevel( "Test1" );
//		}

	//}
	
	void OnPhotonCreateRoomFailed()
	{
		if( IsHost == true )
		{
			return;
		}
	}
	
	void OnPhotonJoinRoomFailed()
	{
		if( IsHost == true )
		{
			return;
		}
	}
	
	/// <summary>
	/// Called when we created a Photon room.
	/// </summary>
	void OnCreatedRoom()
	{
		Debug.Log( "OnCreatedRoom" );
	}


	
	/// <summary>
	/// Called when we successfully joined a room. It is also called if we created the room.
	/// </summary>
	void OnJoinedRoom()
	{
		//Pause the message queue. While unity is loading a new level, updates from Photon are skipped.
		//So we have to tell Photon to wait until we resume the queue again after the level is loaded. See MultiplayerConnector.OnLevelWasLoaded
		PhotonNetwork.isMessageQueueRunning = false;

		Debug.Log( "OnJoinedRoom. Loading map: " + Application.loadedLevelName );
		
		Application.LoadLevel( "Test1" );

	}
	
	/// <summary>
	/// Called by Unity after Application.LoadLevel is completed
	/// </summary>
	/// <param name="level">The index of the level that was loaded</param>
	void OnLevelWasLoaded( int level )
	{
		Debug.Log( "OnLevelWasLoaded: " + Application.loadedLevelName );
		
		//Resume the Photon message queue so we get all the updates.
		//All updates that were sent during the level load were cached and are dispatched now so we can handle them properly.
		PhotonNetwork.isMessageQueueRunning = true;
	}
	
	void OnDisconnectedFromPhoton()
	{
	}
	
	void OnFailedToConnectToPhoton( DisconnectCause cause )
	{
		if( IsHost == true )
		{
			return;
		}
		
		Debug.LogWarning( "OnFailedToConnectToPhoton: " + cause );
	}
	
	void OnConnectionFail( DisconnectCause cause )
	{
		if( IsHost == true )
		{
			return;
		}
		
		Debug.LogWarning( "OnConnectionFail: " + cause );
	}
	
	void OnLeftRoom()
	{
		Debug.Log( "OnLeftRoom" );
		
		if( IsHost == true )
		{
			return;
		}
		
		Application.LoadLevel( "RoomBrowser" );
	}
	
	public static MultiplayerConnector instance;
	public static MultiplayerConnector Instance
	{
		get
		{
			if( instance == null )
			{
				CreateInstance();
			}
			
			return instance;
		}
	}
	
	public static void CreateInstance()
	{
		if( instance == null )
		{
			GameObject connectorObject = GameObject.Find( "MultiplayerConnector" );
			
			if( connectorObject == null )
			{
				connectorObject = new GameObject( "MultiplayerConnector" );
				connectorObject.AddComponent<MultiplayerConnector>();
			}
			
			instance = connectorObject.GetComponent<MultiplayerConnector>();
		}
	}
}
