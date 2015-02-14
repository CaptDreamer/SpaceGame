using UnityEngine;
using System.Collections;

/// <summary>
/// This class checks the user input and sends the data on to the components that need it
/// </summary>
public class ShipInput : ShipBase
{
	public bool UseJoystickMovement;

	void Start()
	{
	}

	void Update()
	{
		if( PhotonView.isMine == true )
		{
			UpdateTurn();
			UpdateSpeed();
			UpdateIsShooting();
		}

	}

	void UpdateIsShooting()
	{
		ShipShooting.IsShooting = Ship.RightJoystick.position.magnitude > 0;
	}
	
	void UpdateTurn()
	{;
		float targetTurn = 0;
		if (UseJoystickMovement)
		{
			if(Vector3.Cross(LeftJoystick.position, ShipMovement.GetForwardVector()).z > 0)
			{
				targetTurn = Vector2.Angle(LeftJoystick.position,ShipMovement.GetForwardVector())/180;
			} 
			else if (Vector3.Cross(LeftJoystick.position, ShipMovement.GetForwardVector()).z < 0)
			{
				targetTurn = (Vector2.Angle(LeftJoystick.position,ShipMovement.GetForwardVector())/180) * -1;
			}
		}
		else
		{
			targetTurn = Input.GetAxisRaw( "Horizontal" );
		}
		ShipMovement.TargetTurn = targetTurn;
	}
	
	void UpdateSpeed()
	{
		float targetBoost = 0;
		if (UseJoystickMovement) 
		{
			//float angle = Vector2.Angle(LeftJoystick.position,ShipMovement.GetForwardVector());
//			if(angle < 90)
//			{
				float Amount =  Mathf.Clamp(LeftJoystick.position.magnitude,0,1);
				//float AdjustAmount = Mathf.Lerp(1,0,angle/90);
			targetBoost = Amount; //* AdjustAmount;
			//}
//			else if (Vector2.Angle(LeftJoystick.position,ShipMovement.GetForwardVector()) >= 90)
//			{
//				float Amount =  Mathf.Clamp(LeftJoystick.position.magnitude,0,1) * -1;
//				float AdjustAmount = Mathf.Lerp(0,1,angle/90-1);
//				targetBoost = Amount * AdjustAmount;
//			}

		}
		else
		{
			targetBoost = Input.GetAxisRaw( "Vertical" );
		}
		ShipMovement.TargetBoost = targetBoost;
	}
}