using UnityEngine;
using UnityEngine.UI;

public class DynamicVerticalSpacing : MonoBehaviour
{
	public VerticalLayoutGroup verticalLayoutGroup; // Reference to the Vertical Layout Group
	public float baseSpacing = 10f; // Default spacing value for a standard screen height
	public float referenceScreenHeight = 1080f; // Reference screen height for base spacing
	public float spacingMod = 2;

	void Start()
	{
		AdjustSpacing();
	}

	void AdjustSpacing()
	{
		// Get the current screen height
		float currentScreenHeight = Screen.height;

		// Calculate a scaling factor based on the screen height compared to the reference screen height
		float scaleFactor = (currentScreenHeight * spacingMod / referenceScreenHeight);

		// Adjust the spacing in the Vertical Layout Group based on the scaling factor
		verticalLayoutGroup.spacing = baseSpacing * scaleFactor;
	}

	// Optional: Update the spacing dynamically if the screen resolution changes (for example, on window resize)
	void Update()
	{
		AdjustSpacing();
	}
}
