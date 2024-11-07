using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{
    public CollectibleContainerData headContainerData;
    public PlayerController pc;
    public Transform hatTransform;
    public SpriteRenderer hairRenderer;
    [HideInInspector] public Hat wornHat;

    [HideInInspector] public GameObject lastHatObject;
    [HideInInspector] public GameObject hatObject;
    public GameObject HatObject
    {
        get { return hatObject; }
        set {
            lastHatObject = hatObject;
            hatObject = value;
            if (hatObject != null && hatObject.TryGetComponent(out Hat hat)) // Hat added or swapped
            {
                wornHat = hat;

				headContainerData.Container.collectibleSlots[0].CollectibleName = wornHat.hatData.name;
				headContainerData.Container.collectibleSlots[0].quantity = 1;
			}
            else // Hat taken off
            {
                // Hat can only be removed by dragging off, which will handle setting the slot to null and quantity 0
                wornHat = null;
                hairRenderer.enabled = true;
			}
			headContainerData.onContainerCollectibleUpdated.Raise();
        }
    }

	private void Awake()
	{
		// Initialize hat
		if (headContainerData.Container.collectibleSlots[0].Collectible != null && wornHat == null) // Hat missing from head
		{
			string hatName = headContainerData.Container.collectibleSlots[0].Collectible.name;
			GameObject prefab = Resources.Load<GameObject>(hatName);
			hatObject = Instantiate(prefab, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
			Optimizer.list.Add(hatObject);
			wornHat = hatObject.GetComponent<Hat>();
			wornHat.Interact(false, pc);
		}
	}
}
