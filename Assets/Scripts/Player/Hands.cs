using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Hands : MonoBehaviour
{
    private TMP_Text _leftAmmoCount;
    private TMP_Text _rightAmmoCount;

    private bool _usingRight;
    public bool UsingRight { get { return _usingRight; }
        set
        {
            _usingRight = value;
            if (_usingRight)
            {
                if (_rightObject.TryGetComponent(out Firearm firearm))
                {
                    _rightAmmoCount.enabled = true;
                }
            }
            else
            {
                _rightAmmoCount.enabled = false;
            }
        }
    }

    private bool _usingLeft;
    public bool UsingLeft
    {
        get { return _usingLeft; }
        set
        {
            _usingLeft = value;
            if (_usingLeft)
            {
                if (_leftObject.TryGetComponent(out Firearm firearm))
                {
                    _leftAmmoCount.enabled = true;
                }
            }
            else
            {
                _leftAmmoCount.enabled = false;
            }
        }
    }


    private GameObject? _leftObject;
    public GameObject? LeftObject
    {
        get { return _leftObject; }
        set { _leftObject = value;
            if(_leftObject != null)
            {
			    if (value.TryGetComponent(out Item leftItem)) _leftItem = leftItem;
            }
            else
            {
                _leftItem = null;
            }
		}
    }

	private GameObject? _rightObject;
    public GameObject? RightObject
    {
        get { return _rightObject; }
        set { _rightObject = value; 
            if(_rightObject != null)
            {
                if(value.TryGetComponent(out Item rightItem)) _rightItem = rightItem;
            }
            else
            {
                _rightItem = null;
            }
        }
    }

    public Item? _leftItem;
    public Item? _rightItem;

    void Awake()
    {
        _leftAmmoCount = GameObject.FindWithTag("LeftAmmoCount").GetComponent<TMP_Text>();
        _rightAmmoCount = GameObject.FindWithTag("RightAmmoCount").GetComponent<TMP_Text>();
        _leftAmmoCount.enabled = false;
        _rightAmmoCount.enabled = false;
    }
}
