using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Hands : MonoBehaviour
{
    public CollectibleContainerData handContainerData;

    private TMP_Text leftAmmoCount;
    private TMP_Text rightAmmoCount;


    private bool usingRight;
    public bool UsingRight { get { return usingRight; }
        set
        {
            usingRight = value;
            if (usingRight)
            {
                if (rightObject.TryGetComponent(out ProjectileWeapon projectileWeapon))
                {
                    rightAmmoCount.enabled = true;
                }
            }
            else
            {
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
                if (leftObject.TryGetComponent(out ProjectileWeapon projectileWeapon))
                {
                    leftAmmoCount.enabled = true;
                }
            }
            else
            {
                leftAmmoCount.enabled = false;
            }
        }
    }


    private GameObject leftObject;
    public GameObject LeftObject
    {
        get { return leftObject; }
        set { leftObject = value;
            if(leftObject != null)
            {
			    if (value.TryGetComponent(out Item leftItem))
                {
                    this.leftItem = leftItem;
                    handContainerData.Container.collectibleSlots[0].collectible = leftItem.itemData;
                    handContainerData.onContainerCollectibleUpdated.Raise();
                }
            }
            else
            {
                handContainerData.Container.collectibleSlots[0].collectible = null;
                handContainerData.onContainerCollectibleUpdated.Raise();
                leftItem = null;
            }
		}
    }

	private GameObject rightObject;
    public GameObject RightObject
    {
        get { return rightObject; }
        set { rightObject = value; 
            if(rightObject != null)
            {
                if(value.TryGetComponent(out Item rightItem))
                {
                    this.rightItem = rightItem;
                    handContainerData.Container.collectibleSlots[1].collectible = rightItem.itemData;
                    handContainerData.onContainerCollectibleUpdated.Raise();
                }
            }
            else
            {
                handContainerData.Container.collectibleSlots[1].collectible = null;
                handContainerData.onContainerCollectibleUpdated.Raise();
                rightItem = null;
            }
        }
    }

    public Item leftItem;
    public Item rightItem;

    void Awake()
    {
        // Ammo count
        leftAmmoCount = GameObject.FindWithTag("LeftAmmoCount").GetComponent<TMP_Text>();
        rightAmmoCount = GameObject.FindWithTag("RightAmmoCount").GetComponent<TMP_Text>();
        leftAmmoCount.enabled = false;
        rightAmmoCount.enabled = false;

        // Initialize Hand Items
        if (handContainerData.Container.collectibleSlots[0].collectible != null && leftItem == null) // Item missing from left hand
        {
            Debug.Log(handContainerData.Container.collectibleSlots[0].collectible.name);

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

    private void Start()
    {
        
    }
}
