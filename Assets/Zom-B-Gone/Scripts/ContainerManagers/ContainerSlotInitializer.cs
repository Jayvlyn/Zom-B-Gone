using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Should be placed on the slot holder that has all slots as children
public class ContainerSlotInitializer : MonoBehaviour
{
    [SerializeField] public CollectibleContainerData containerData = null;
    [SerializeField] public string slotPrefabName;
    [SerializeField] private float backgroundStartingBottom = 150;
	[SerializeField] public int rowBackgroundIncrement = 130;
	[SerializeField] public int rowSize = 6;

	[HideInInspector] public RectTransform backgroundRect;

	private void Awake()
	{
		backgroundRect = transform.parent.GetComponent<RectTransform>();
		int rowCount = containerData.size / rowSize;

        backgroundRect.sizeDelta = new Vector2(backgroundRect.sizeDelta.x, backgroundStartingBottom + rowBackgroundIncrement * (rowCount-1));

        //for (int i = 1; i < rowCount; i++)
        //{
        //    backgroundRect.sizeDelta = new Vector2(backgroundRect.sizeDelta.x, backgroundRect.sizeDelta.y + rowBackgroundIncrement);
        //    //backgroundRect.offsetMin = new Vector2(backgroundRect.offsetMin.x, backgroundRect.offsetMin.y - rowBackgroundIncrement);
        //}
    }

	void Start()
    {
        // Child count is how many slot prefabs exist right now
        if(transform.childCount < containerData.size)
        {
            // Makes an empty slot with the same allows
            // makes template with last slot, as containers like backpack have a first slot that is different from the rest
            CollectibleSlot slotTemplate = containerData.Container.collectibleSlots[containerData.size-1];
            //slotTemplate.Collectible = null;
            slotTemplate.CollectibleName = null;
            slotTemplate.quantity = 0;

            Object newSlot = Resources.Load(slotPrefabName);

            int missingSlotCount = containerData.size - transform.childCount;
            for (int i = 0; i < missingSlotCount; i++)
            {
                // New slot with this as the parent
                Instantiate(newSlot, transform);
            }

            containerData.onContainerCollectibleUpdated.Raise();
        }
    }

}
