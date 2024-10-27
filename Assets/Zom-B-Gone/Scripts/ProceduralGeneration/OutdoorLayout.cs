using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutdoorLayout : MonoBehaviour
{
	public Transform pivot;

	public void RotateLayout(float rotation)
	{
		transform.RotateAround(pivot.position, new Vector3(0, 0, 1), rotation);
	}
}
