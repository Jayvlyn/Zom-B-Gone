using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookaheadChanger : MonoBehaviour
{
	public CinemachineTargetGroup targetGroup;
	public int mosueIndex = 1; 
	public float lookaheadRadius = 30;
	public float regularRadius = 1;

	public void ActivateLookahead()
	{
		targetGroup.m_Targets[mosueIndex].radius = lookaheadRadius;
	}

	public void DeactivateLookahead()
	{
		targetGroup.m_Targets[mosueIndex].radius = regularRadius;
	}
}
