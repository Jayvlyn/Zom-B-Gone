using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public GameObject targetIndicatorPrefab;
    public Camera mainCamera;
    public float offset = 10f;

    public List<TargetIndicator> targetIndicators = new List<TargetIndicator>();

	private void Start()
	{
		//FindTargetsByTag("enemy");
	}

	private void Update()
	{
		foreach (var target in targetIndicators)
		{
			target.UpdateTargetPosition();
		}
	}

	void FindTargetsByTag(string tag)
	{
		GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        foreach (var item in objects)
        {
			AddTarget(item.transform);
        }
    }

	public void AddTarget(Transform targetTransform)
	{
		if (!HasTargetIndicator(targetTransform))
		{
			GameObject indicatorObj = Instantiate(targetIndicatorPrefab, transform);
			TargetIndicator targetIndicator = indicatorObj.GetComponent<TargetIndicator>();
			targetIndicator.Initialize(targetTransform, mainCamera, offset);

			targetIndicators.Add(targetIndicator);
		}
	}

	public void RemoveTarget(Transform targetTransform)
	{
		TargetIndicator targetIndicator = targetIndicators.Find(indicator => indicator.Target == targetTransform);
		if(targetIndicator != null)
		{
			targetIndicators.Remove(targetIndicator);
			Destroy(targetIndicator.gameObject);
		}
	}

	bool HasTargetIndicator(Transform targetTransform)
	{
		return targetIndicators.Exists(indicator => indicator.Target == targetTransform);
	}
}

