using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Should be placed on the slot holder that has all slots as children
public class ContainerSlotInitializer : MonoBehaviour
{
    [SerializeField] public CollectibleContainerData containerData = null;
    [SerializeField] private string slotPrefabName;
    [SerializeField] private float backgroundStartingBottom = 850;
	[SerializeField] public int rowBackgroundIncrement = 130;
	[SerializeField] public int rowSize = 6;

	[HideInInspector] public RectTransform backgroundRect;

	private void Awake()
	{
		backgroundRect = transform.parent.GetComponent<RectTransform>();
        backgroundRect.offsetMin = new Vector2(backgroundRect.offsetMin.x, backgroundStartingBottom);

		int rowCount = containerData.size / rowSize;
        for (int i = 1; i < rowCount; i++)
        {
            backgroundRect.offsetMin = new Vector2(backgroundRect.offsetMin.x, backgroundRect.offsetMin.y - rowBackgroundIncrement);
        }
    }

	void Start()
    {
        // Child count is how many slot prefabs exist right now
        if(transform.childCount < containerData.size)
        {
            // Makes an empty slot with the same allows
            // makes template with last slot, as containers like backpack have a first slot that is different from the rest
            CollectibleSlot slotTemplate = containerData.Container.collectibleSlots[containerData.size-1]; 
            slotTemplate.collectible = null;
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
