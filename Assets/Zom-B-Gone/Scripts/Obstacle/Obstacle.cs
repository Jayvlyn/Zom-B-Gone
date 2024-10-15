using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Obstacle : MonoBehaviour, IInteractable
{
	public enum ObstacleState
	{
		FREE, GRABBED
	}
	public ObstacleState currentState;

	[Range(20000f, 75000f)]public float weight = 20000f;
	public Rigidbody2D rb;
	public Joint2D joint;

	public void ChangeState(ObstacleState state)
	{
		switch(state)
		{
			case ObstacleState.FREE:
				if(joint)
				{
					Destroy(joint.gameObject);
					joint = null;
				}
				rb.bodyType = RigidbodyType2D.Dynamic;
				break;

			case ObstacleState.GRABBED:
				rb.bodyType = RigidbodyType2D.Kinematic;
				//joint.
				break;
		}
	}

	public void Interact(bool rightHand)
	{
		Debug.Log("interact with obstacle");
	}

	public void Interact(Head head)
	{
		throw new System.NotImplementedException();
	}
}
