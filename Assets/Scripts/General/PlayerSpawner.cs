using UnityEngine;
using System.Collections;

/// <summary>
/// Creates the synchronized ship objects
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
	void Start()
	{
		//if we are not connected, than we probably pressed play in a level in editor mode.
		//In this case go back to the main menu to connect to the server first
//		if( PhotonNetwork.connected == false )
//		{
//			Application.LoadLevel( "MainMenu" );
//			return;
//		}

		CreateLocalPlayer ();
	}
	
	public void CreateLocalPlayer( )
	{
		object[] instantiationData = new object[] {  } ;
		
		//Notice the differences from PhotonNetwork.Instantiate to Unitys GameObject.Instantiate
		GameObject newShipObject = PhotonNetwork.Instantiate( 
		                                                     "Ship", 
		                                                     Vector3.zero, 
		                                                     Quaternion.identity, 
		                                                     0,
		                                                     instantiationData
		                                                     );
		
		Ship newShip = newShipObject.GetComponent<Ship>();
		newShipObject.transform.parent = GameObject.Find ("Foreground").transform;
		//Since this function is called on every machine to create it's one and only local player, the new ship is always the camera target
		newShip.CreateJoystick ();
		GameObject.Find("Camera").GetComponent<CameraShipFollow>().SetTarget( newShip );
	}
	
}