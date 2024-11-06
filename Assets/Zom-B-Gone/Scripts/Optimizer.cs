using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optimizer : MonoBehaviour
{
    public static List<GameObject> list = new List<GameObject>();
	public Transform PlayerT;
	public float cullDistance = 200f;

	private void Start()
	{
		StartCoroutine(DistanceCheck());
	}

	private IEnumerator DistanceCheck()
	{
		while (true)
		{
			for (int i = list.Count - 1; i >= 0; i--)
			{
				GameObject go = list[i];

				if (go == null)
				{
					list.RemoveAt(i);
					continue;
				}

				float dist = Vector2.Distance(go.transform.position, PlayerT.transform.position);
				if (dist >= cullDistance)
				{
					go.SetActive(false);
				}
				else
				{
					go.SetActive(true);
				}
			}

			yield return new WaitForSeconds(5);
		}
	}
}
