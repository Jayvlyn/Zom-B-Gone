using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour
{
    [Tooltip("When player enters this collider, it will bring up the menu to loot this container")]
    public Collider2D proximityCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// slide out existing lootable menu if any

        // slide in new lootable menu (same ui element) but filled with this lootable's collectibles.
	}
}
