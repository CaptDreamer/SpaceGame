using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class handles all the position and rotation updates for local and remote ships
/// It keeps track of how fast the ship is going, where it is turning, and if its bouncing off of an obstacle
/// </summary>
public class ShipMovement : ShipBase
{	
	/// <summary>
	/// The current speed of the ship
	/// </summary>
	public float Speed;
	
	/// <summary>
	/// The max speed of the ship
	/// </summary>
	public float MaxSpeed;

	/// <summary>
	/// How fast should the ship go from slow to fast
	/// </summary>
	public float Acceleration;

	/// <summary>
	/// How fast should the ship go from fast to slow
	/// </summary>
	public float Deceleration;

	/// <summary>
	/// How fast should the ship turn after the player sends the input
	/// </summary>
	public float TurnAcceleration;

	/// <summary>
	/// How fast is the ship turning
	/// </summary>
	public float TurnSpeed;

	/// <summary>
	/// This is set by the input class to tell where the player wants to go
	/// </summary>
	/// <value>
	/// The target turn.
	/// </value>
	public float TargetTurn
	{
		get;
		set;
	}

	/// <summary>
	/// This is set by the input class to tell how fast the player wants to go
	/// </summary>
	/// <value>
	/// The target boost power.
	/// </value>
	public float TargetBoost
	{
		get;
		set;
	}

	/// <summary>
	/// This value smoothly moves towards the turn that is send from the users input
	/// </summary>
	float m_TurnDelta;

	/// <summary>
	/// This is how fast the ship is currently turning and in what direction
	/// </summary>
	float m_TurnAngle;

	/// <summary>
	/// This value smoothly moves towards the speed that is send from the users input
	/// </summary>
	float m_Speed;

	/// <summary>
	/// This is the position where the network tells us the player is
	/// But since this is only updated 10 times a second, we store it so we can calculate the
	/// real position of a remote ship by applying it's known speed and turn angle
	/// </summary>
	Vector3 m_NetworkPosition;
	
	/// <summary>
	/// Same for the rotation. The ship should rotate smoothly, so we store the received value
	/// and interpolate towards it slowly to smoothen out any stutter
	/// </summary>
	Quaternion m_NetworkRotation;
	
	/// <summary>
	/// We need to know how old the last NetworkPosition and Rotation is so we can move the
	/// ship forward more, the older the data is
	/// </summary>
	double m_LastNetworkDataReceivedTime;

	void Start()
	{
		m_Speed = Speed;
	}

	void FixedUpdate()
	{
		if( PhotonView.isMine == true )
		{
			if( Ship.Health > 0 )
			{
				UpdateSpeed();
				UpdateTurn();
				UpdateTurnAngle();
				UpdateRotation();
				UpdatePosition();
			}
		}
		else
		{
			UpdateNetworkedPosition();
			UpdateNetworkedRotation();
		}
	}

	void UpdateNetworkedPosition()
	{
		//Here we try to predict where the player actually is depending on the data we received through the network
		//Check out Part 1 Lesson 2 http://youtu.be/7hWuxxm6wsA for more detailed explanations
		float pingInSeconds = (float)PhotonNetwork.GetPing() * 0.001f;
		float timeSinceLastUpdate = (float)( PhotonNetwork.time - m_LastNetworkDataReceivedTime );
		float totalTimePassed = pingInSeconds + timeSinceLastUpdate;
		
		Vector3 exterpolatedTargetPosition = m_NetworkPosition
			+ transform.forward * m_Speed * totalTimePassed;
		
		
		Vector3 newPosition = Vector3.MoveTowards( transform.position
		                                          , exterpolatedTargetPosition
		                                          , m_Speed * Time.deltaTime );
		
		if( Vector3.Distance( transform.position, exterpolatedTargetPosition ) > 2f )
		{
			newPosition = exterpolatedTargetPosition;
		}
		
		transform.position = newPosition;
	}
	
	void UpdateNetworkedRotation()
	{
		transform.rotation = Quaternion.RotateTowards(
			transform.rotation,
			m_NetworkRotation, 180f * Time.deltaTime
			);
	}

	void UpdateSpeed()
	{
		//The player speed input ranges from [-1;1]
		//If its lower than zero, we want to apply the breaks
		//If its bigger than zero, we want to boost

		float targetSpeed = Speed;
		
		targetSpeed = MaxSpeed * Mathf.Clamp(LeftJoystick.position.magnitude,0,1);
		
		// if( TargetBoost > 0 )
		// {
			
		// }
		// else if( TargetBoost < 0 )
		// {
			// targetSpeed = Mathf.Lerp( Speed, BreakSpeed, Mathf.Abs( TargetBoost ) );
		// }

		if( targetSpeed > m_Speed )
		{
			m_Speed = Mathf.Lerp( m_Speed, targetSpeed, Acceleration * Time.deltaTime );
		}
		else
		{
			m_Speed = Mathf.Lerp( m_Speed, targetSpeed, Deceleration * Time.deltaTime );
		}
	}

	void UpdateTurn()
	{
		m_TurnDelta = Mathf.Lerp( m_TurnDelta, TargetTurn, TurnAcceleration * Time.deltaTime );
	}


	void UpdateRotation()
	{
		transform.rotation = Quaternion.identity;

		transform.Rotate( Vector3.forward, -m_TurnAngle );
	}

	void UpdatePosition()
	{
		//Calculate the new position based on speed and possible impact movement
		Vector3 newPosition = (Vector3)transform.position + (Vector3)GetForwardVector() * m_Speed * Time.deltaTime;

		//124,57
		newPosition.x = Mathf.Clamp (newPosition.x,-124,124);
		newPosition.y = Mathf.Clamp (newPosition.y,-57,57);

		transform.position = newPosition;
	}

	void UpdateTurnAngle()
	{
		float turnSpeed = TurnSpeed;
		
		m_TurnAngle += m_TurnDelta * turnSpeed * Time.deltaTime;
	}

	public float GetCurrentSpeed()
	{
		return m_Speed;
	}

	public float GetTurnDelta()
	{
		return m_TurnDelta;
	}

	public Vector2 GetForwardVector()
	{
		return new Vector2 (transform.up.x, transform.up.y);
	}

	//Debug Display of test variables
//	void OnGUI()
//	{
//		GUILayout.BeginArea( new Rect( 10, 10, Screen.width, Screen.height ) );
//		{
//			//GUILayout.Label( "Turn Delta: " + m_TurnDelta );
//			GUILayout.Label( "Speed: " + m_Speed );
//			GUILayout.Label( "Ship Direction: " + GetForwardVector());
//			GUILayout.Label( "Ship Position: " + (Vector2)transform.position);
//			GUILayout.Label( "Push Angle: " + Vector2.Angle(GetForwardVector(),LeftJoystick.position));
//			GUILayout.Label( "Push Mag: " + Mathf.Clamp(LeftJoystick.position.magnitude,0,1));
//		}
//		GUILayout.EndArea();
//	}

	public void SerializeState( PhotonStream stream, PhotonMessageInfo info )
	{
		//We only need to synchronize a couple of variables to be able to recreate a good
		//approximation of the ships position on each client
		//There is a lot of smoke and mirrors happening here
		//Check out Part 1 Lesson 2 http://youtu.be/7hWuxxm6wsA for more detailed explanations
		if( stream.isWriting == true )
		{
			stream.SendNext( transform.position );
			stream.SendNext( transform.rotation );
			stream.SendNext( m_Speed );
		}
		else
		{
			m_NetworkPosition = (Vector3)stream.ReceiveNext();
			m_NetworkRotation = (Quaternion)stream.ReceiveNext();
			m_Speed = (float)stream.ReceiveNext();
			
			m_LastNetworkDataReceivedTime = info.timestamp;
		}
	}
}