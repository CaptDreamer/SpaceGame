using UnityEngine;
using System.Collections;

/// <summary>
/// Nothing much happens here. Just some text drawing to make the player push a button and connecting to Photon if the player wants to start
/// </summary>
public class MainMenu : MonoBehaviour
{
	void Start()
	{
		if( PhotonNetwork.playerName == "Pilot XXX" )
		{
			if( PlayerPrefs.HasKey( "LoginUsername" ) == true )
			{
				PhotonNetwork.playerName = PlayerPrefs.GetString( "LoginUsername" );
			}
			else
			{
				PhotonNetwork.playerName = "Pilot " + System.Environment.TickCount % 1000;
			}
		}
	}
	
	void OnGUI()
	{
		DrawUsernameTextField();
		DrawConnectButton();
		DrawPulsingLabel();
	}
	
	void DrawUsernameTextField()
	{
		float textFieldWidth = 300;
		float textFieldHeight = 40;
		
		PhotonNetwork.playerName = GUI.TextField (new Rect ((Screen.width - textFieldWidth) * 0.5f, (Screen.height - textFieldHeight) * 0.5f + 150, textFieldWidth, textFieldHeight), PhotonNetwork.playerName);
	}
	
	void DrawConnectButton()
	{
		float buttonWidth = 300;
		float buttonHeight = 40;
		
		if( GUI.Button( new Rect( ( Screen.width - buttonWidth ) * 0.5f, ( Screen.height - buttonHeight ) * 0.5f + 200, buttonWidth, buttonHeight ), "Connect") )
		{
			if( PhotonNetwork.playerName != "" )
			{
				PlayerPrefs.SetString( "LoginUsername", PhotonNetwork.playerName );
				MultiplayerConnector.Instance.Connect();
			}
		}
	}
	
	void DrawPulsingLabel()
	{
		GUI.color = new Color( 1f, 1f, 1f, Mathf.Sin( Time.realtimeSinceStartup * 4f ) * 0.4f + 0.6f );
		
		float labelWidth = 300;
		float labelHeight = 100;
		
		
		string label = "";
		
		switch( PhotonNetwork.connectionState )
		{
		case ConnectionState.Disconnected:
			label = "Insert your name";
			break;
		default:
			label = "Connecting...\n" + PhotonNetwork.connectionStateDetailed;
			break;
		}
		
		GUI.Label( new Rect( ( Screen.width - labelWidth ) * 0.5f, ( Screen.height - labelHeight ) * 0.5f, labelWidth, labelHeight ), label);
	}
}
