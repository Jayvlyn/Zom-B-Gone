using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optimizer : MonoBehaviour
{
    public static List<GameObject> list = new List<GameObject>();
	public Transform PlayerT;
	public float cullDistance = 200f;

	public static int maxActiveEnemies;
	public static int currentActiveEnemies;

	public LayerMask enemyLm;

	private void Start()
	{
		enemyLm = LayerMask.GetMask("Enemy");
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
					if ((enemyLm.value & (1 << go.gameObject.layer)) != 0) currentActiveEnemies--;
					go.SetActive(false);
				}
				else
				{
					if ((enemyLm.value & (1 << go.gameObject.layer)) != 0) currentActiveEnemies++;
					go.SetActive(true);
				}
			}

			yield return new WaitForSeconds(5);
		}
	}
}
