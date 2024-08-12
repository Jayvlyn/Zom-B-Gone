using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatChanger : MonoBehaviour
{
    public CollectibleContainerSlot headSlot;
    private Head playerHead;

    private void Awake()
    {
        playerHead = FindObjectOfType<Head>();    
    }

    public void CheckHatChange()
    {
        if (headSlot.SlotCollectible == null && playerHead.wornHat != null) // remove hat on head from world (data not lost, exists in inventory)
        {
            Destroy(playerHead.wornHat.gameObject);
        }

        else if (headSlot.SlotCollectible != null && playerHead.wornHat == null) // add hat on head
        {
            SpawnNewHatOnHead();
        }

        else if (headSlot.SlotCollectible != null && playerHead.wornHat != null) // swap hat on head
        {
            if (playerHead.wornHat.hatData.name != headSlot.SlotCollectible.name)
            {
                Destroy(playerHead.wornHat.gameObject);

                SpawnNewHatOnHead();
            }
        }
    }

    private void SpawnNewHatOnHead()
    {
        string hatName = headSlot.SlotCollectible.name;
        GameObject prefab = Resources.Load<GameObject>(hatName);
        GameObject hatObject = Instantiate(prefab, playerHead.transform.position, playerHead.transform.rotation);
        Hat wornHat = hatObject.GetComponent<Hat>();
        wornHat.Interact(playerHead);
    }
}

