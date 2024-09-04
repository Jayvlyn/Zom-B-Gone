using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider2D))]
public class Locker : MonoBehaviour, IInteractable
{
	[SerializeField] private CollectibleContainerData lockerData;
	[SerializeField] private VoidEvent lockerOpened;

	public void Interact(bool rightHand)
	{
		OpenLockerUI();
		gameObject.layer = LayerMask.NameToLayer("Default");
    }

	private void OpenLockerUI()
	{
		lockerOpened.Raise();
    }

	public void OnLockerClosed()
	{
		gameObject.layer = LayerMask.NameToLayer("Interactable");
	}

	public void Interact(Head head)
	{
		throw new System.NotImplementedException();
	}
}
