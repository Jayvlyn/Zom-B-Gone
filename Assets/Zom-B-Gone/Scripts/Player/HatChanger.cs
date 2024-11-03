using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatChanger : MonoBehaviour
{
    public CollectibleContainerSlot headSlot;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = FindFirstObjectByType<PlayerController>();  
    }

    public void CheckHatChange()
    {
        if (headSlot.SlotCollectible == null && playerController.head.wornHat != null) // remove hat on head from world (data not lost, exists in inventory)
        {
            Destroy(playerController.head.wornHat.gameObject);
            if (playerController.head.wornHat.hatData.camo) playerController.gameObject.layer = LayerMask.NameToLayer("Player");
			playerController.head.HatObject = null;
        }

        else if (headSlot.SlotCollectible != null && playerController.head.wornHat == null) // add hat on head
        {
            SpawnNewHatOnHead();
        }

        else if (headSlot.SlotCollectible != null && playerController.head.wornHat != null) // swap hat on head
        {
            if (playerController.head.wornHat.hatData.name != headSlot.SlotCollectible.name)
            {
                Destroy(playerController.head.wornHat.gameObject);
				if (playerController.head.wornHat.hatData.camo) playerController.gameObject.layer = LayerMask.NameToLayer("Player");

				SpawnNewHatOnHead();
            }
        }
    }

    private void SpawnNewHatOnHead()
    {
        string hatName = headSlot.SlotCollectible.name;
        GameObject prefab = Resources.Load<GameObject>(hatName);
        GameObject hatObject = Instantiate(prefab, playerController.head.transform.position, playerController.head.transform.rotation);
        Hat wornHat = hatObject.GetComponent<Hat>();
        wornHat.Interact(false, playerController);
    }
}

