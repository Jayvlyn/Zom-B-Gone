using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOptionsInitializer : MonoBehaviour
{
	[SerializeField] RectTransform loadOptionsBackground;
	[SerializeField] float loadOptionsHeight = 100;

	private void Awake()
	{
		Object loadOption = Resources.Load("LoadOption");
		foreach(string name in SaveManager.saves.lootrunnerSaves.Keys)
		{
			// Increase scrollable background size
			loadOptionsBackground.sizeDelta = new Vector2(loadOptionsBackground.sizeDelta.x, loadOptionsBackground.sizeDelta.y + loadOptionsHeight);
		}
	}
}
