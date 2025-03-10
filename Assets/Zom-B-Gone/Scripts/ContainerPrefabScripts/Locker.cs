using GameEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider2D))]
public class Locker : MonoBehaviour, IInteractable
{
	[SerializeField] private CollectibleContainerData lockerData;
	[SerializeField] private VoidEvent lockerOpened;

	public void Interact(bool rightHand, PlayerController playerController)
	{
		OpenLockerUI();
		gameObject.layer = LayerMask.NameToLayer("World");
    }

	private void OpenLockerUI()
	{
		lockerOpened.Raise();
    }

	public void OnLockerClosed()
	{
		gameObject.layer = LayerMask.NameToLayer("Interactable");
	}
}
