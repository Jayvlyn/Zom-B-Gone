using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UnitCameraFixer : MonoBehaviour
{
	public CinemachineVirtualCamera cam;
	private void Awake()
	{
		var framingTransposer = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
		if (framingTransposer != null)
		{
			framingTransposer.m_ScreenX = 0.78f;
		}

	}
}
