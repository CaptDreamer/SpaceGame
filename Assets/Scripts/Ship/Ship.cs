using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This is the main ship class
/// It handles health, respawn, communication between scripts and synchronizing the ship through the network via OnPhotonSerializeView
/// </summary>
public class Ship : ShipBase
{

	public static Ship LocalPlayer;
	
	public bool IsLocalPlayer = true;

	/// <summary>
	/// This prefab is instantiated for the local player to show the Joystick
	/// </summary>
	public GameObject JoystickPrefab;
	public GameObject RightJoystickPrefab;

	float m_Health = 50;

	/// <summary>
	/// The health is between 50 and 0
	/// </summary>
	/// <value>
	/// The ships health.
	/// </value>
	public float Health
	{
		get
		{
			return m_Health;
		}
	}

	/// <summary>
	/// The string for the kill count custom property
	/// </summary>
	const string KillCountProperty = "KillCount";

	bool m_IsVisible = true;
	/// <summary>
	/// Gets a value indicating whether this instance is visible.
	/// </summary>
	/// <value>
	/// <c>true</c> if this ship is flying; if it's dead, <c>false</c>.
	/// </value>
	public bool IsVisible
	{
		get
		{
			return m_IsVisible;
		}
	}

	void Start()
	{
		if( PhotonView.isMine == true )
		{
			if( IsLocalPlayer == true )
			{
				LocalPlayer = this;
			}
		}
	}

	public void CreateJoystick()
	{
		GameObject newJoystick = (GameObject)Instantiate( JoystickPrefab );
		newJoystick.name = "LeftJoystick";
		newJoystick.transform.parent = null;
		newJoystick.transform.localPosition = Vector3.zero;
		newJoystick.transform.localRotation = Quaternion.identity;
		
		BroadcastMessage( "OnJoystickCreated", SendMessageOptions.DontRequireReceiver );

		GameObject newRightJoystick = (GameObject)Instantiate( RightJoystickPrefab );
		newRightJoystick.name = "RightJoystick";
		newRightJoystick.transform.parent = null;
		newRightJoystick.transform.localPosition = Vector3.zero;
		newRightJoystick.transform.localRotation = Quaternion.identity;

		BroadcastMessage( "OnJoystickCreated", SendMessageOptions.DontRequireReceiver );
	}

	public void DealDamage( float damage, Ship damageDealer )
	{
		m_Health -= damage;

		OnHealthChanged( damageDealer );
	}

//	public void SendHeal( float heal, Ship healer)
//	{
//		m_Health += heal;
//		
//		OnHealthChanged( healer );
//	}

	void OnHealthChanged()
	{
		OnHealthChanged( null );
	}

	void OnHealthChanged( Ship causeShip )
	{
		if( m_Health <= 0 )
		{
			m_Health = 0;
			Debug.Log("Boom, you're dead");
			//Instantiate( ExplosionPrefab, transform.position, Quaternion.identity );
			SetVisibility( false );
		}
		
		if( m_Health >= 50 )
		{
			m_Health = 50;
			Debug.Log("Max health Baby!");
		}
	}

	void SetVisibility( bool visible )
	{
		m_IsVisible = visible;

		Renderer[] renderers = GetComponentsInChildren<Renderer>();

		for( int i = 0; i < renderers.Length; ++i )
		{
			renderers[ i ].enabled = visible;
		}
	}

	void OnPhotonInstantiate( PhotonMessageInfo info )
	{
		//This method gets called right after a GameObject is created through PhotonNetwork.Instantiate
		//The fifth parameter in PhotonNetwork.instantiate sets the instantiationData and every client
		//can access them through the PhotonView. In our case we use this to send which team the ship
		//belongs to. This methodology is very useful to send data that only has to be sent once.
		
		if( PhotonView.isMine == false )
		{
			gameObject.transform.parent = GameObject.Find("Foreground").transform;
		}
	}
	
	void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
	{
		//Multiple components need to synchronize values over the network.
		//The SerializeState methods are made up, but they're useful to keep
		//all the data separated into their respective components
		
		SerializeState( stream, info );
		
		ShipVisuals.SerializeState( stream, info );
		ShipMovement.SerializeState( stream, info );
	}
	
	void SerializeState( PhotonStream stream, PhotonMessageInfo info )
	{
		if( stream.isWriting == true )
		{
			stream.SendNext( m_Health );
		}
		else
		{
			float oldHealth = m_Health;
			m_Health = (float)stream.ReceiveNext();
			
			if( m_Health != oldHealth )
			{
				OnHealthChanged();
			}
		}
	}

	void OnGUI()
	{
		if( PhotonView.isMine == true )
		{
			GUILayout.BeginArea( new Rect( 10, 10, Screen.width, Screen.height ) );
			{
				GUILayout.Label( "Health: " + m_Health);
				GUILayout.Label( "Latency: " + (float)PhotonNetwork.GetPing());
				GUILayout.Label( "Multitouch: " + Input.multiTouchEnabled);
				//GUILayout.Label( "Right Stick Rotation: " + RightJoystick.GetRotation());
			}
			GUILayout.EndArea();
		}
	}
} 
