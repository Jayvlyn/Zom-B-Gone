using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PosRot
{
	public Vector2 position;
	public Quaternion rotation;

	public PosRot(Vector2 position, Quaternion rotation)
	{
		this.position = position;
		this.rotation = rotation;
	}

	public string ToStringKey()
	{
		Quaternion normalizedRotation = NormalizeQuaternion(rotation);

		float roundedPosX = Mathf.Round(position.x * 1000f) / 1000f;
		float roundedPosY = Mathf.Round(position.y * 1000f) / 1000f;

		float roundedRotX = Mathf.Round(normalizedRotation.x * 1000f) / 1000f;
		float roundedRotY = Mathf.Round(normalizedRotation.y * 1000f) / 1000f;
		float roundedRotZ = Mathf.Round(normalizedRotation.z * 1000f) / 1000f;
		float roundedRotW = Mathf.Round(normalizedRotation.w * 1000f) / 1000f;

		return $"{roundedPosX},{roundedPosY},{roundedRotX},{roundedRotY},{roundedRotZ},{roundedRotW}";
	}

	private Quaternion NormalizeQuaternion(Quaternion q)
	{
		// If the w component is negative, flip all components to ensure consistency.
		if (q.w < 0f)
		{
			q.x = -q.x;
			q.y = -q.y;
			q.z = -q.z;
			q.w = -q.w;
		}
		return q;
	}
}