using UnityEngine;
using System.Collections;

/// <summary>
/// This class deals with the visual representation of the ship
/// I created it as a child to the actual ship object because I wanted to be able to
/// rotate and roll the visual representation further, without affecting the movement direction
/// As a general tip, I found that it's always better to make the actual mesh of the object a child of the main object
/// because it makes a lot of stuff easier once the object get more complicated
/// </summary>
public class ShipVisuals : ShipBase
{
	/// <summary>
	/// The visual parent is a child object of the main gameObject this component is attached to
	/// so we need to know which of the child objects it is
	/// </summary>
	public Transform VisualParent;

	/// <summary>
	/// How fast should the visual object turn
	/// </summary>
	public float TurnAcceleration;

	/// <summary>
	/// When the ship turns, the visual representation turns even further. This gives the player a bigger range where he can shoot
	/// </summary>
	public float MaximumTurnAngle;

	float m_VisualTurn;

	void Start()
	{
	}

	void Update()
	{
//		UpdateVisualTurn();
//		UpdateVisualRotation();
	}

	void UpdateVisualTurn()
	{
		m_VisualTurn = Mathf.Lerp( m_VisualTurn, ShipMovement.GetTurnDelta(), TurnAcceleration * Time.deltaTime );
	}

	void UpdateVisualRotation()
	{
		//Since the VisualParent is just a child of the main object, we can rotate it however we want without affecting its actual movement
		//This is just eye candy rotation to make the flying feel better

		VisualParent.transform.localRotation = Quaternion.identity;
		
		VisualParent.transform.Rotate( Vector3.up, m_VisualTurn * MaximumTurnAngle );
	}

	public void SerializeState( PhotonStream stream, PhotonMessageInfo info )
	{

	}
}