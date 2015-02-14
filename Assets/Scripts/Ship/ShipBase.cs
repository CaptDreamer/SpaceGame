using UnityEngine;
using System.Collections;

/// <summary>
/// Every ship component inherits from this one
/// Since they reference each other all the time, I was lazy and setup all the references once
/// </summary>
public class ShipBase : MonoBehaviour
{
	Ship m_Ship;
	public Ship Ship
	{
		get
		{
			return Helper.GetCachedComponent<Ship>( gameObject, ref m_Ship );
		}
	}

	ShipShooting m_ShipShooting;
	public ShipShooting ShipShooting
	{
		get
		{
			return Helper.GetCachedComponent<ShipShooting>( gameObject, ref m_ShipShooting );
		}
	}

	ShipCollision m_ShipCollision;
	public ShipCollision ShipCollision
	{
		get
		{
			return Helper.GetCachedComponent<ShipCollision>( gameObject, ref m_ShipCollision );
		}
	}

	ShipMovement m_Movement;
	public ShipMovement ShipMovement
	{
		get
		{
			return Helper.GetCachedComponent<ShipMovement>( gameObject, ref m_Movement );
		}
	}

	ShipVisuals m_Visuals;
	public ShipVisuals ShipVisuals
	{
		get
		{
			return Helper.GetCachedComponent<ShipVisuals>( gameObject, ref m_Visuals );
		}
	}

	ShipInput m_Input;
	public ShipInput ShipInput
	{
		get
		{
			return Helper.GetCachedComponent<ShipInput>( gameObject, ref m_Input );
		}
	}
	
	Joystick m_LeftJoystick;
	public Joystick LeftJoystick
	{
		get
		{
			return Helper.GetCachedComponent<Joystick>( GameObject.Find("LeftJoystick"), ref m_LeftJoystick );
		}
	}

	Joystick m_RightJoystick;
	public Joystick RightJoystick
	{
		get
		{
			return Helper.GetCachedComponent<Joystick>( GameObject.Find("RightJoystick"), ref m_RightJoystick );
		}
	}

	CameraShipFollow m_Camera;
	public CameraShipFollow GameCamera
	{
		get
		{
			return Helper.GetCachedComponent<CameraShipFollow>( GameObject.Find("Camera"), ref m_Camera );
		}
	}

	PhotonView m_View;
	public PhotonView PhotonView
	{
		get
		{
			return Helper.GetCachedComponent<PhotonView>( gameObject, ref m_View );
		}
	}

}