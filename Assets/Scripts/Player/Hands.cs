using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Hands : MonoBehaviour
{
    private TMP_Text _leftAmmoCount;
    private TMP_Text _rightAmmoCount;

    private bool _usingRight;
    private bool _usingLeft;

    public bool UsingRight { get { return _usingRight;  } 
        set 
        {
            _usingRight = value;
            if(_usingRight)
            {
                if(_rightObject.TryGetComponent(out Firearm firearm))
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

    public GameObject _leftObject;
    public GameObject _rightObject;

    void Awake()
    {
        _leftAmmoCount = GameObject.FindWithTag("LeftAmmoCount").GetComponent<TMP_Text>();
        _rightAmmoCount = GameObject.FindWithTag("RightAmmoCount").GetComponent<TMP_Text>();
        _leftAmmoCount.enabled = false;
        _rightAmmoCount.enabled = false;
    }

    void Update()
    {
        
    }
}
