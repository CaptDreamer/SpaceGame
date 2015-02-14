using UnityEngine;
using System.Collections;

public class ParallaxEffect : MonoBehaviour {

	public float Speed;

	float m_LayerZ;
	
	Transform m_Target;
	
	Vector3 m_TargetPosition;

	void Start()
	{
		SetTarget(GameObject.Find("Camera").transform);
		m_LayerZ = gameObject.transform.position.z;
	}
	
	void LateUpdate()
	{
		if(m_Target == null)
		{
			return;
		}
		UpdatePosition ();
	}

	public Transform GetTarget()
	{
		return m_Target;
	}
	
	/// <summary>
	/// Sets the target ship which the camera should follow. This is always the active player in the game
	/// </summary>
	/// <param name="target">The target ship which should be followed</param>
	public void SetTarget( Transform target )
	{
		m_Target = target;
	}
	
	void UpdatePosition()
	{
		m_TargetPosition = m_Target.transform.position * Speed;
		m_TargetPosition.z = m_LayerZ;
		transform.position = m_TargetPosition;
	}
}
