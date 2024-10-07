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

		float roundedPosX = Mathf.Round(position.x * 10000f) / 10000f;
		float roundedPosY = Mathf.Round(position.y * 10000f) / 10000f;

		float roundedRotX = Mathf.Round(normalizedRotation.x * 10000f) / 10000f;
		float roundedRotY = Mathf.Round(normalizedRotation.y * 10000f) / 10000f;
		float roundedRotZ = Mathf.Round(normalizedRotation.z * 10000f) / 10000f;
		float roundedRotW = Mathf.Round(normalizedRotation.w * 10000f) / 10000f;

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