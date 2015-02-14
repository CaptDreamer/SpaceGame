using UnityEngine;
using System.Collections;

public class Helper 
{
	public static T GetCachedComponent<T>( GameObject gameObject, ref T cachedComponent ) where T : MonoBehaviour
	{
		if( cachedComponent == null )
		{
			cachedComponent = gameObject.GetComponent<T>();
		}

		return cachedComponent;
	}

//	public static Ship GetPlayerShip( Ship cachedComponent)
//	{
//		if( cachedComponent == null)
//		{
//			cachedComponent = GameObject.Find("Ship").GetComponent<Ship>();
//		}
//
//		return cachedComponent;
//	}
}
