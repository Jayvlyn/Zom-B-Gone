using System.Collections;
using UnityEngine;

public class MainMenuSequence : MonoBehaviour
{
	public Transform explosionSource;
	public GameObject victimZombie;

	public GameObject zombieObject;
	public GameObject armObject;
	public GameObject blood;

	private void Start()
	{
		StartCoroutine(Sequence());
	}


	public IEnumerator Sequence()
    {
		yield return new WaitForSeconds(3);

		Utils.CreateExplosion(explosionSource.position, 30, 700, 99, true);
		Destroy(victimZombie);

		zombieObject.SetActive(true);
		armObject.SetActive(true);
		blood.SetActive(true);
    }
}
