using GameEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Hands : MonoBehaviour
{
    public CollectibleContainerData handContainerData;
    public PlayerController playerController;

    [HideInInspector] public TMP_Text leftAmmoCount;
    [HideInInspector] public TMP_Text rightAmmoCount;

    [HideInInspector] public TMP_Text leftReloadingIndicator;
    [HideInInspector] public TMP_Text rightReloadingIndicator;

	[HideInInspector] public float leftLumbering = 1;
	[HideInInspector] public float rightLumbering = 1;
    [SerializeField] private float lumberingLowerBound = 0.8f;

	[HideInInspector] public Item leftItem;
	[HideInInspector] public Item rightItem;

    public Image leftIcon;
    private Obstacle leftObstacle;
    private SpriteRenderer leftObstacleSpriteRenderer;
    public Obstacle LeftObstacle
    {
        set { leftObstacle = value;
            if (leftObstacle)
            {
                leftIcon.enabled = true;
                SetHandIconToObstacle(false);
            }
            else
            {
                leftIcon.enabled = false;
                leftIcon.sprite = null;
                usingLeft = false;
                leftObstacleSpriteRenderer = null;
            }
        }
        get { return leftObstacle; }
    }

    public Image rightIcon;
    private Obstacle rightObstacle;
    private SpriteRenderer rightObstacleSpriteRenderer;
	public Obstacle RightObstacle
    {
		set { rightObstacle = value;
            if (rightObstacle)
            {
                rightIcon.enabled = true;
                SetHandIconToObstacle(true);
            }
            else
            {
                rightIcon.enabled = false;
                rightIcon.sprite = null;
                usingRight = false;
                rightObstacleSpriteRenderer = null;
            }
		}
        get { return rightObstacle; }
	}

	private bool usingRight;
    public bool UsingRight { get { return usingRight; }
        set
        {
            usingRight = value;
            if (usingRight)
            {
                if (rightItem as ProjectileWeapon) rightAmmoCount.enabled = true;
                else rightAmmoCount.enabled = false;

                if (rightItem != null) rightLumbering = Utils.MapWeightToRange(rightItem.itemData.weight, lumberingLowerBound, 1.0f, true);
     //           else if (RightObstacle != null)
     //           {
     //               float weight = RightObstacle.weight;
     //               if (LeftObstacle != null && LeftObstacle == RightObstacle)
     //               {
     //                   weight *= 0.5f; // split weight between hands
					//	leftLumbering = Utils.MapWeightToRange(weight, lumberingLowerBound, 1.0f, true, true);
					//}
     //               rightLumbering = Utils.MapWeightToRange(weight, lumberingLowerBound, 1.0f, true, true);
     //           }
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

				if (leftItem != null) leftLumbering = Utils.MapWeightToRange(leftItem.itemData.weight, lumberingLowerBound, 1.0f, true);
				//else if (LeftObstacle != null)
				//{
				//	float weight = LeftObstacle.weight;
				//	if (RightObstacle != null && RightObstacle == LeftObstacle)
				//	{
				//		weight *= 0.5f; // split weight between hands
				//		rightLumbering = Utils.MapWeightToRange(weight, lumberingLowerBound, 1.0f, true, true);
				//	}
				//	leftLumbering = Utils.MapWeightToRange(weight, lumberingLowerBound, 1.0f, true, true);
				//}
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
                    //handContainerData.Container.collectibleSlots[0].Collectible = leftItem.itemData;
                    handContainerData.Container.collectibleSlots[0].CollectibleName = leftItem.itemData.name;
                    handContainerData.Container.collectibleSlots[0].quantity = leftItem.Quantity;
                }
            }
            else
            {
                //handContainerData.Container.collectibleSlots[0].Collectible = null;
                handContainerData.Container.collectibleSlots[0].CollectibleName = null;
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
                    //handContainerData.Container.collectibleSlots[1].Collectible = rightItem.itemData;
                    handContainerData.Container.collectibleSlots[1].CollectibleName = rightItem.itemData.name;
                    handContainerData.Container.collectibleSlots[1].quantity = rightItem.Quantity;
                }
            }
            else
            {
                //handContainerData.Container.collectibleSlots[1].Collectible = null;
                handContainerData.Container.collectibleSlots[1].CollectibleName = null;
                handContainerData.Container.collectibleSlots[1].quantity = 0;
                rightItem = null;
            }
            handContainerData.onContainerCollectibleUpdated.Raise();
        }
    }

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
        if (handContainerData.Container.collectibleSlots[0].Collectible != null && leftItem == null) // Item missing from left hand
        {
            string itemName = handContainerData.Container.collectibleSlots[0].CollectibleName;
            GameObject prefab = Resources.Load<GameObject>(itemName);
            leftObject = Instantiate(prefab, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
			Optimizer.list.Add(leftObject);
			leftItem = leftObject.GetComponent<Item>();
			leftItem.Interact(false, playerController);
			UsingLeft = true;
            
        }
        if (handContainerData.Container.collectibleSlots[1].Collectible != null && rightItem == null) // Item missing from right hand
        {
            string itemName = handContainerData.Container.collectibleSlots[1].CollectibleName;
            GameObject prefab = Resources.Load<GameObject>(itemName);
			rightObject = Instantiate(prefab, gameObject.transform.position, new Quaternion(0, 0, 0, 0));
            Optimizer.list.Add(rightObject);
            rightItem = rightObject.GetComponent<Item>();
            rightItem.Interact(true, playerController);
            UsingRight = true;
        }

    }

    public void SetHandIconToObstacle(bool rightHand)
    {
        if(rightHand)
        {
            if(rightObstacle)
            {
                if (rightObstacleSpriteRenderer == null) rightObstacleSpriteRenderer = rightObstacle.GetComponent<SpriteRenderer>();
                rightIcon.sprite = rightObstacleSpriteRenderer.sprite;
                rightIcon.enabled = true;
            }
        }

        else
        {
            if (leftObstacle)
            {
                if(leftObstacleSpriteRenderer == null) leftObstacleSpriteRenderer = leftObstacle.GetComponent<SpriteRenderer>();
                
                leftIcon.sprite = leftObstacleSpriteRenderer.sprite;
                leftIcon.enabled = true;
            }
        }
    }

    public void OnHandItemSwapped()
    {
        ItemData leftSlotItem = (ItemData)handContainerData.Container.collectibleSlots[0].Collectible;
        ItemData rightSlotItem = (ItemData)handContainerData.Container.collectibleSlots[1].Collectible;

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
