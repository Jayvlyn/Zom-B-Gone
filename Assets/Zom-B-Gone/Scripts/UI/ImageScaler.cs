using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageScaler : MonoBehaviour
{
	public RectTransform[] imageRects;  // Assign in the inspector or dynamically
	public float screenPadding = 20f;  // Padding from the edges of the screen

	void Start()
	{
		ScaleAndArrangeImages();
	}

	void ScaleAndArrangeImages()
	{
		// Get screen dimensions minus padding
		float maxWidth = Screen.width - 2 * screenPadding;
		float maxHeight = Screen.height - 2 * screenPadding;

		// Calculate the appropriate size for each image based on their layout
		float targetWidth = maxWidth / imageRects.Length;  // Adjust for a horizontal arrangement
		float targetHeight = Mathf.Min(maxHeight, targetWidth);  // Keep it within screen height bounds

		foreach (RectTransform rect in imageRects)
		{
			// Set the sizeDelta to scale the images while preserving arrangement
			rect.sizeDelta = new Vector2(targetWidth, targetHeight);
		}

		ArrangeImages();
	}

	void ArrangeImages()
	{
		// Example: Simple horizontal arrangement
		float xOffset = screenPadding;
		foreach (RectTransform rect in imageRects)
		{
			rect.anchoredPosition = new Vector2(xOffset, -screenPadding);  // Adjust for vertical offset as well
			xOffset += rect.sizeDelta.x + screenPadding;  // Update position for the next image
		}
	}
}