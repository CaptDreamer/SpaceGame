using UnityEngine;
using System.Collections;

/// <summary>
/// Defines base functionality that is used in all projectiles. This demo only has one type of projectile, but try to add more yourself :)
/// </summary>
public class ProjectileBase : MonoBehaviour
{
	/// <summary>
	/// How fast is the projectile flying?
	/// </summary>
	public float Speed;

	/// <summary>
	/// How long is the projectile alive until it is destroyed automatically
	/// </summary>
	public float LifeTime;

	/// <summary>
	/// Reference to the hit effect that is played when the projectile hits a ship or the environment
	/// </summary>
	//public GameObject HitFX;

	double m_CreationTime;
	Vector3 m_StartPosition;
	int m_ProjectileId;

	Ship m_Owner;

	/// <summary>
	/// The owner of this projectile is the ship who fired it
	/// </summary>
	public Ship Owner
	{
		get
		{
			return m_Owner;
		}
		set
		{
			m_Owner = value;
		}
	}


	public int ProjectileId
	{
		get
		{
			return m_ProjectileId;
		}
	}

	void Start()
	{
		//Do Physics.SphereCastAll
	}

	public void SetStartPosition( Vector3 position )
	{
		m_StartPosition = position;
	}

	public void SetCreationTime( double time )
	{
		m_CreationTime = time;
	}

	public void SetProjectileId( int id )
	{
		m_ProjectileId = id;
	}

	void Update()
	{
		float timePassed = (float)( PhotonNetwork.time - m_CreationTime );
		transform.position = m_StartPosition + transform.up * Speed * timePassed;

		if( timePassed > LifeTime )
		{
			Destroy( gameObject );
		}
		if( transform.position.y < -57 || transform.position.y > 57 || transform.position.x < -124 || transform.position.x > 124 )
		{
			Destroy( gameObject );
			//CreateHitFx();
		}
	}

//	void CreateHitFx()
//	{
//		Instantiate( HitFX, transform.position, transform.rotation );
//	}

	public void OnProjectileHit()
	{
		Destroy( gameObject );
		//CreateHitFx();
	}

	void OnTriggerEnter2D( Collider2D collider )
	{
		if( collider.tag == "Obstacle" )
		{
			OnProjectileHit();
		}
		else if( collider.tag == "Ship" )
		{
			Ship ship = collider.GetComponent<Ship>();

			if( ship.PhotonView.isMine == false )
			{
				return;
			}

			if( m_Owner != ship )
			{
				ship.ShipCollision.OnProjectileHit( this );
				OnProjectileHit();
				m_Owner.ShipShooting.SendProjectileHit( m_ProjectileId );
			}
		}
	}
}