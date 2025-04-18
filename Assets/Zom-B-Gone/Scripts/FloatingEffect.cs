using UnityEngine;

// by chat gpt
public class FloatingEffect : MonoBehaviour
{
	public RectTransform rectTransform;  // Assign your RectTransform here
	public float floatRange = 5f;        // Max distance to float in each direction
	public float floatSpeed = 1f;        // Speed of floating
	public float rotationRange = 5f;     // Max rotation in degrees
	public float rotationSpeed = 1f;     // Speed of rotation
	public float manualRotationOffset = 0f;


	public Vector2 originalPosition = Vector2.zero;
	private float timeOffsetX;
	private float timeOffsetY;
	private float rotationOffset;

	void Start()
	{
		// Save the original position to offset from it
		if(originalPosition == Vector2.zero)
		{
			originalPosition = rectTransform.anchoredPosition;
		}

		// Generate random time offsets to make the movement unique
		timeOffsetX = Random.Range(0f, 100f);
		timeOffsetY = Random.Range(0f, 100f);
		rotationOffset = Random.Range(0f, 100f);
	}

	void Update()
	{
		// Use Perlin noise for smooth random movement
		float offsetX = Mathf.PerlinNoise(Time.time * floatSpeed + timeOffsetX, 0f) * 2f - 1f;
		float offsetY = Mathf.PerlinNoise(Time.time * floatSpeed + timeOffsetY, 0f) * 2f - 1f;

		// Calculate new position
		Vector2 newPosition = originalPosition + new Vector2(offsetX, offsetY) * floatRange;

		// Apply new position to RectTransform
		rectTransform.anchoredPosition = newPosition;

		// Use Perlin noise for smooth random rotation
		float rotation = Mathf.PerlinNoise(Time.time * rotationSpeed + rotationOffset, 0f) * 2f - 1f;
		float newRotation = (rotation * rotationRange) + manualRotationOffset;

		// Apply rotation to RectTransform
		rectTransform.localRotation = Quaternion.Euler(0f, 0f, newRotation);
	}
}
