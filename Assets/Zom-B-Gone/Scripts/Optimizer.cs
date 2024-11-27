using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optimizer : MonoBehaviour
{
    public static List<GameObject> list = new List<GameObject>();
	public Transform PlayerT;
	public float cullDistance = 200f;

	public static int maxActiveEnemies = 200;
	public static int currentActiveEnemies = 0;

	public LayerMask enemyLm;

	private void Start()
	{
		currentActiveEnemies = 0;
		//enemyLm = LayerMask.GetMask("Enemy");
		StartCoroutine(DistanceCheck());
	}

	//private void Update()
	//{
	//	Debug.Log(currentActiveEnemies);
	//}

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
					if(go.activeSelf)
					{
						if ((enemyLm.value & (1 << go.gameObject.layer)) != 0) currentActiveEnemies--;
						go.SetActive(false);
					}
				}
				else if (!go.activeSelf)
				{
					if ((enemyLm.value & (1 << go.gameObject.layer)) != 0)
					{
						if (currentActiveEnemies < maxActiveEnemies) ;
						{
							currentActiveEnemies++;
							go.SetActive(true);
						}
					}
					else
					{
						go.SetActive(true);
					}

				}
			}

			yield return new WaitForSeconds(2.5f);
		}
	}
}
