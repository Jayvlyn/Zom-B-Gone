using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locker : MonoBehaviour, IInteractable
{
	public GameObject ContainerUI;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Interact(bool rightHand)
	{
		// Pull up locker menu on left side
	}

	public void Interact(Head head)
	{
		throw new System.NotImplementedException();
	}
}
