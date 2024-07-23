using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Hands : MonoBehaviour
{
    private TMP_Text leftAmmoCount;
    private TMP_Text rightAmmoCount;

    private bool usingRight;
    public bool UsingRight { get { return usingRight; }
        set
        {
            usingRight = value;
            if (usingRight)
            {
                if (rightObject.TryGetComponent(out Firearm firearm))
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
                if (leftObject.TryGetComponent(out Firearm firearm))
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
			    if (value.TryGetComponent(out Item leftItem)) this.leftItem = leftItem;
            }
            else
            {
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
                if(value.TryGetComponent(out Item rightItem)) this.rightItem = rightItem;
            }
            else
            {
                rightItem = null;
            }
        }
    }

    public Item leftItem;
    public Item rightItem;

    void Awake()
    {
        leftAmmoCount = GameObject.FindWithTag("LeftAmmoCount").GetComponent<TMP_Text>();
        rightAmmoCount = GameObject.FindWithTag("RightAmmoCount").GetComponent<TMP_Text>();
        leftAmmoCount.enabled = false;
        rightAmmoCount.enabled = false;
    }
}
