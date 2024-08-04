using GameEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Hands : MonoBehaviour
{
    public CollectibleContainerData handContainerData;

    [HideInInspector] public TMP_Text leftAmmoCount;
    [HideInInspector] public TMP_Text rightAmmoCount;

    [HideInInspector] public TMP_Text leftReloadingIndicator;
    [HideInInspector] public TMP_Text rightReloadingIndicator;

	[HideInInspector] public float leftLumbering = 1;
	[HideInInspector] public float rightLumbering = 1;
    [SerializeField] private float lumberingLowerBound = 0.8f;


    private bool usingRight;
    public bool UsingRight { get { return usingRight; }
        set
        {
            usingRight = value;
            if (usingRight)
            {
                if (rightItem as ProjectileWeapon) rightAmmoCount.enabled = true;
                else rightAmmoCount.enabled = false;

                rightLumbering = Utils.MapWeightToRange(rightItem.itemData.weight, lumberingLowerBound, 1.0f, true);
            }
            else
            {
                rightLumbering = 1;
                rightAmmoCount.enabled = false;
            }
        }
    }

    private bool usingLeft;
    public bool UsingLeft
    {
        get { return usingLeft; }
        set
        {
            usingLeft = value;
            if (usingLeft)
            {
                if (leftItem as ProjectileWeapon) leftAmmoCount.enabled = true;
                else leftAmmoCount.enabled = false;

                leftLumbering = Utils.MapWeightToRange(leftItem.itemData.weight, lumberingLowerBound, 1.0f, true);
            }
            else
            {
                leftLumbering = 1;
                leftAmmoCount.enabled = false;
            }

        }
    }


    private GameObject leftObject;
    public GameObject LeftObject
    {
        get { return leftObject; }
        set { leftObject = value;
            if (leftObject != null)
            {
                if (value.TryGetComponent(out Item leftItem))
                {
                    this.leftItem = leftItem;
                    handContainerData.Container.collectibleSlots[0].collectible = leftItem.itemData;
                    handContainerData.Container.collectibleSlots[0].quantity = 1;
                }
            }
            else
            {
                handContainerData.Container.collectibleSlots[0].collectible = null;
                handContainerData.Container.collectibleSlots[0].quantity = 0;
                leftItem = null;
            }
            handContainerData.onContainerCollectibleUpdated.Raise();
        }
    }

    private GameObject rightObject;
    public GameObject RightObject
    {
        get { return rightObject; }
        set { rightObject = value;
            if (rightObject != null)
            {
                if (value.TryGetComponent(out Item rightItem))
                {
                    this.rightItem = rightItem;
                    handContainerData.Container.collectibleSlots[1].collectible = rightItem.itemData;
                    handContainerData.Container.collectibleSlots[1].quantity = 1;
                }
            }
            else
            {
                handContainerData.Container.collectibleSlots[1].collectible = null;
                handContainerData.Container.collectibleSlots[1].quantity = 0;
                rightItem = null;
            }
            handContainerData.onContainerCollectibleUpdated.Raise();
        }
    }

    [HideInInspector] public Item leftItem;
	[HideInInspector] public Item rightItem;

    void Awake()
    {
        // Ammo count
        leftAmmoCount = GameObject.FindWithTag("LeftAmmoCount").GetComponent<TMP_Text>();
        rightAmmoCount = GameObject.FindWithTag("RightAmmoCount").GetComponent<TMP_Text>();
		leftReloadingIndicator = GameObject.FindWithTag("LeftReloadingIndicator").GetComponent<TMP_Text>();
		rightReloadingIndicator = GameObject.FindWithTag("RightReloadingIndicator").GetComponent<TMP_Text>();
		leftAmmoCount.enabled = false;
        rightAmmoCount.enabled = false;

        // Initialize Hand Items
        if (handContainerData.Container.collectibleSlots[0].collectible != null && leftItem == null) // Item missing from left hand
        {
            string itemName = handContainerData.Container.collectibleSlots[0].collectible.name;
            GameObject prefab = Resources.Load<GameObject>(itemName);
            leftObject = Instantiate(prefab, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
            leftItem = leftObject.GetComponent<Item>();
            leftItem.PickUp(gameObject.transform, false);
            UsingLeft = true;

			StartCoroutine(DelayedLeftInit());
            
        }
        if (handContainerData.Container.collectibleSlots[1].collectible != null && rightItem == null) // Item missing from right hand
        {
            string itemName = handContainerData.Container.collectibleSlots[1].collectible.name;
            GameObject prefab = Resources.Load<GameObject>(itemName);
            rightObject = Instantiate(prefab, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
            rightItem = rightObject.GetComponent<Item>();
            rightItem.PickUp(gameObject.transform, true);
            UsingRight = true;
            StartCoroutine(DelayedRightInit());
        }

    }

    private IEnumerator DelayedLeftInit()
    {
        yield return new WaitForSeconds(0.5f);
        
    }

    private IEnumerator DelayedRightInit()
    {
        yield return new WaitForSeconds(0.5f);
        
    }

    public void OnHandItemSwapped()
    {
        ItemData leftSlotItem = (ItemData)handContainerData.Container.collectibleSlots[0].collectible;
        ItemData rightSlotItem = (ItemData)handContainerData.Container.collectibleSlots[1].collectible;

        Item cachedLeftItem = leftItem;
        Item cachedRightItem = rightItem;

        GameObject cachedLeftObject = leftObject;
        GameObject cachedRightObject = rightObject;

        bool swapLeftToRightIndicator = false;

        if(leftItem) leftItem.useHeld = false;
        if(rightItem) rightItem.useHeld = false;

		if (rightSlotItem != null && leftItem != null && cachedLeftItem.itemData == rightSlotItem)
		{ // swap left item to right hand
            rightItem = cachedLeftItem;
            rightObject = cachedLeftObject;

			UsingRight = true;
            if (leftSlotItem == null)
            {
                UsingLeft = false;
                leftItem = null;
                leftObject = null;
            }

            rightItem.inRightHand = true;
            rightItem.PositionInHand();
            if (rightItem as ProjectileWeapon)
            {
                ((ProjectileWeapon)rightItem).UpdateAmmoCount();
                swapLeftToRightIndicator = ((ProjectileWeapon)rightItem).reloading;
				if (swapLeftToRightIndicator)
                {
                    rightReloadingIndicator.enabled = true;
                    leftReloadingIndicator.enabled = false;
                }
            }
        }

		if (leftSlotItem != null && rightItem != null && cachedRightItem.itemData == leftSlotItem)
		{ // swap right item to left hand
			leftItem = cachedRightItem;
			leftObject = cachedRightObject;

			UsingLeft = true;
			if (rightSlotItem == null)
            {
                UsingRight = false;
                rightItem = null;
                leftObject = null;
            }

            leftItem.inRightHand = false;
            leftItem.PositionInHand();
            if (leftItem as ProjectileWeapon)
            {
                ((ProjectileWeapon)leftItem).UpdateAmmoCount();
				if (((ProjectileWeapon)leftItem).reloading)
				{
					leftReloadingIndicator.enabled = true;
					if (!swapLeftToRightIndicator) rightReloadingIndicator.enabled = false;
				}
			}
		}

	}
}
