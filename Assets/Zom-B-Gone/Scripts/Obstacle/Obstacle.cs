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

	public bool moveable = true;
	public bool variedRotation = false;
	public Rigidbody2D rb;
	public HingeJoint2D joint;
	public Collider2D coll;
	public float grabRange = 0.6f;
	[Header("Optional Lootable Component")]
	public Lootable lootable;

	[HideInInspector] public bool twoHandsOn;
	private float baseDensity;

	private Hands playerHands;

    private void Awake()
    {
		if (!moveable)
		{
			rb.bodyType = RigidbodyType2D.Static;
			if(lootable == null)gameObject.layer = LayerMask.NameToLayer("Obstacle");
		}

		baseDensity = coll.density;

		if(variedRotation)
		{
			float rotation = Random.Range(-5, 5);
			transform.Rotate(0,0,rotation);
		}
    }

    public void ChangeState(ObstacleState state)
	{
		switch(state)
		{
			case ObstacleState.FREE:
				//gameObject.layer = LayerMask.NameToLayer("Interactable");
				joint.connectedBody = null;
				joint.enabled = false;
				coll.density = baseDensity;
                twoHandsOn = false;
				playerHands = null;
                break;

			case ObstacleState.GRABBED:
                //gameObject.layer = LayerMask.NameToLayer("Obstacle");
                joint.enabled = true;
				OnOneHandOn();
				break;
		}
		currentState = state;
	}

	public void Interact(bool rightHand, PlayerController playerController)
	{
		ChangeState(ObstacleState.GRABBED);
		playerHands = playerController.hands;
	}

	public void BeFreed()
	{
		ChangeState(ObstacleState.FREE);
	}

	public void OnTwoHandsOn()
	{
		coll.density = baseDensity * 0.12f;
		twoHandsOn = true;
	}

	public void OnOneHandOn()
	{
		coll.density = baseDensity * 0.25f;
		twoHandsOn = false;
	}

	public void ChangeMotorSpeed(float speed = 0)
	{
		if (twoHandsOn) speed *= 2;
		JointMotor2D motor = joint.motor;
        motor.motorSpeed = speed;
        joint.motor = motor;
    }

    private void OnDestroy()
    {
		if(playerHands)
		{
			// could be both
			if(playerHands.RightObstacle == this)
			{
				playerHands.RightObstacle = null;
			}
			if(playerHands.LeftObstacle == this)
			{
				playerHands.LeftObstacle = null;
			}
		}
    }
}
