using GameEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeleteCharacterRaiser : MonoBehaviour
{
	[SerializeField] StringEvent eventToRaise;
	[SerializeField] TMP_Text textMesh;

	public void RaiseEvent()
	{
		eventToRaise.Raise(textMesh.text);
	}
}

