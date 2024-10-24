using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class LoadOptionsInitializer : MonoBehaviour
{
	[SerializeField] RectTransform loadOptionsBackground;
	[SerializeField] float loadOptionsHeight = 100;

	private void OnEnable()
	{
		Object loadOption = Resources.Load("LoadOption");
		foreach(string name in SaveManager.saves.lootrunnerSaves.Keys)
		{
			// Increase scrollable background size
			loadOptionsBackground.sizeDelta = new Vector2(loadOptionsBackground.sizeDelta.x, loadOptionsBackground.sizeDelta.y + loadOptionsHeight);

			Object option = Instantiate(loadOption, this.transform);

			option.GetComponentInChildren<TMP_Text>().text = name;
		}
	}

    private void OnDisable()
    {
        for (int i = transform.childCount - 1; i >= 0; i--) 
			Destroy(transform.GetChild(i).gameObject);
    }

	public void ReduceSize()
	{
        loadOptionsBackground.sizeDelta = new Vector2(loadOptionsBackground.sizeDelta.x, loadOptionsBackground.sizeDelta.y - loadOptionsHeight);
    }
}
