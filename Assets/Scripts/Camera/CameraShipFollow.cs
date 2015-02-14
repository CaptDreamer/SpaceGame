using UnityEngine;
using System.Collections;

/// <summary>
/// CameraShipFollow is attached to the main camera and controls its movement.
/// After a target ship has been set with SetTarget(), the camera will follow
/// this target and lean intro curves to provide more visibility in a dogfight
/// </summary>
public class CameraShipFollow : MonoBehaviour
{
	/// <summary>
	/// The position relative to the target ship which the camera looks at
	/// </summary>
	//public Vector3 LookAtTarget;
	
	/// <summary>
	/// This is the speed at which the camera moves forward and back when the player changes speed
	/// </summary>
	public float DistanceLerpSpeed;

	public float CameraLayerZ;

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	//public Transform target;

	Ship m_Target;
	
	//Vector3 m_TargetPosition;
	Vector3 m_LookAtPosition;



	float m_DistanceLerp;

	void Start()
	{

	}
	
	void FixedUpdate()
	{
		if(m_Target == null)
		{
			return;
		}
		//UpdateDistanceLerp ();
		UpdatePosition ();
	}
	
	/// <summary>
	/// The distance lerp is used to determine at what offset position the camera currently is. Offset, BreakOffset or BoostOffset
	/// </summary>
	void UpdateDistanceLerp()
	{
		m_DistanceLerp = Mathf.Lerp( m_DistanceLerp, m_Target.ShipMovement.GetCurrentSpeed(), DistanceLerpSpeed * Time.deltaTime );
	}
	
	public Ship GetTarget()
	{
		return m_Target;
	}
	
	/// <summary>
	/// Sets the target ship which the camera should follow. This is always the active player in the game
	/// </summary>
	/// <param name="target">The target ship which should be followed</param>
	public void SetTarget( Ship target )
	{
		m_Target = target;
	}
	
	void UpdatePosition()
	{
		Vector3 point = camera.WorldToViewportPoint(m_Target.transform.position);
		Vector3 delta = m_Target.transform.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
		Vector3 destination = transform.position + delta;
		transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);


		//If the player is applying boost
//		if( m_DistanceLerp > 0 )
//		{
//			realOffset = Vector3.Lerp( Offset, BoostOffset, m_DistanceLerp );
//		}
//		//if the player is breaking
//		else if( m_DistanceLerp < 0 )
//		{
//			realOffset = Vector3.Lerp( Offset, BreakOffset, Mathf.Abs( m_DistanceLerp ) );
//		}
		
//		m_TargetPosition = Vector3.Lerp (m_TargetPosition, m_Target.transform.position, m_DistanceLerp);
//		m_TargetPosition.z = CameraLayerZ;
//		
//		transform.position = m_TargetPosition;
	}
//
//	void OnGUI()
//	{
//		GUILayout.BeginArea( new Rect( 10, 10, Screen.width, Screen.height ) );
//		{
//			GUILayout.Label( "Ship Speed: " + m_Target.ShipMovement.GetCurrentSpeed() );
//			GUILayout.Label( "Distance Lerp Speed: " + DistanceLerpSpeed );
//			GUILayout.Label( "Delta Time: " + Time.deltaTime );
//			GUILayout.Label( "Camera Distance Lerp: " + m_DistanceLerp );
//			//GUILayout.Label( "Camera Target Position: " + m_TargetPosition );
//		}
//		GUILayout.EndArea();
//	}
}
