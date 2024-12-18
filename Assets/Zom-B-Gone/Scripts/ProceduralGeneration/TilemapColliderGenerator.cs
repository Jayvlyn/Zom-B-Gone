using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEditor;
using System.Collections;

// SCRIPT GENERATED BY CHATGPT-4

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(PolygonCollider2D))]
public class TilemapColliderGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public PolygonCollider2D polygonCollider;
    public Sprite ceilingSprite;
    public SpriteRenderer ceilingRenderer;

    [ContextMenu("Generate Collider")]
    public void GenerateCollider()
    {
        // Clear existing points
        polygonCollider.points = new Vector2[0];

        // Get the tilemap bounds
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;

        // Create a list to hold the outline points
        List<Vector2> outlinePoints = new List<Vector2>();

        // Loop through the bounds of the tilemap
        for (int x = bounds.x; x < bounds.x + bounds.size.x; x++)
        {
            for (int y = bounds.y; y < bounds.y + bounds.size.y; y++)
            {
                if (tilemap.GetTile(new Vector3Int(x, y, 0)) != null)
                {
                    // Calculate the world position of the tile
                    Vector3 tileWorldPosition = tilemap.GetCellCenterWorld(new Vector3Int(x, y, 0));
                    Vector2 point = new Vector2(tileWorldPosition.x, tileWorldPosition.y);

                    // Add corners of the tile
                    outlinePoints.Add(point + new Vector2(-0.5f, -0.5f)); // Bottom left
                    outlinePoints.Add(point + new Vector2(0.5f, -0.5f));  // Bottom right
                    outlinePoints.Add(point + new Vector2(0.5f, 0.5f));   // Top right
                    outlinePoints.Add(point + new Vector2(-0.5f, 0.5f));  // Top left
                }
            }
        }

        // Create a bounding box around the outline points
        if (outlinePoints.Count > 0)
        {
            // Get the minimum and maximum bounds
            Vector2 min = outlinePoints[0];
            Vector2 max = outlinePoints[0];

            foreach (Vector2 point in outlinePoints)
            {
                min = Vector2.Min(min, point);
                max = Vector2.Max(max, point);
            }

            // Define the four corners of the bounding box
            polygonCollider.points = new Vector2[]
            {
                new Vector2(min.x, min.y), // Bottom left
                new Vector2(max.x, min.y), // Bottom right
                new Vector2(max.x, max.y), // Top right
                new Vector2(min.x, max.y)  // Top left
            };
        }

        // Re-enable the collider
        polygonCollider.enabled = true;

        GenerateCeilingMesh();

        // Save changes to the prefab
#if UNITY_EDITOR
        // This ensures that changes are marked as dirty and saved in the prefab
        EditorUtility.SetDirty(polygonCollider);
        EditorUtility.SetDirty(this);
#endif
    }

    private void GenerateCeilingMesh()
    {
		// Get the bounds of the collider
		Bounds bounds = polygonCollider.bounds;
		Vector3 center = bounds.center;
		Vector3 size = bounds.size;

		// Create a new GameObject with a SpriteRenderer for the ceiling
		GameObject ceiling = new GameObject("Ceiling");
		ceiling.transform.SetParent(transform);
		ceiling.transform.position = new Vector3(center.x, center.y, transform.position.z - 0.1f); // Slight offset on Z-axis

		// Add and configure the SpriteRenderer
		ceilingRenderer = ceiling.AddComponent<SpriteRenderer>();
		ceilingRenderer.sprite = ceilingSprite;
		ceilingRenderer.drawMode = SpriteDrawMode.Sliced; // Enables scaling without distortion
		ceilingRenderer.size = new Vector2(size.x, size.y);
		ceilingRenderer.color = new Color(1, 1, 1, 1);
		ceilingRenderer.sortingLayerName = "Ceilings";
	}

	private Coroutine hideRoofCoroutine;
	private Coroutine showRoofCoroutine;

	private IEnumerator HideRoof(float speed)
	{
		while (ceilingRenderer.color.a > 0.1f)
		{
			ceilingRenderer.color = new Color(ceilingRenderer.color.r, ceilingRenderer.color.g, ceilingRenderer.color.b, ceilingRenderer.color.a - Time.deltaTime * speed);
			yield return null;
		}

		hideRoofCoroutine = null;
	}

	private IEnumerator ShowRoof(float speed)
	{
		while (ceilingRenderer.color.a < 1f)
		{
			ceilingRenderer.color = new Color(ceilingRenderer.color.r, ceilingRenderer.color.g, ceilingRenderer.color.b, ceilingRenderer.color.a + Time.deltaTime * speed);
			yield return null;
		}

		showRoofCoroutine = null;
	}

	private void OnTriggerEnter2D(Collider2D collision)
    {
		if (collision.CompareTag("Player"))
		{
			FootstepManager.roomTilemap = tilemap;
			if (showRoofCoroutine != null)
			{
				StopCoroutine(showRoofCoroutine);
				showRoofCoroutine = null;
			}

			if (hideRoofCoroutine == null && gameObject.activeInHierarchy)
			{
				hideRoofCoroutine = StartCoroutine(HideRoof(1));
			}
			
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
            FootstepManager.roomTilemap = null;
			if (hideRoofCoroutine != null)
			{
				StopCoroutine(hideRoofCoroutine);
				hideRoofCoroutine = null;
			}

			if (showRoofCoroutine == null && gameObject.activeInHierarchy)
			{
				showRoofCoroutine = StartCoroutine(ShowRoof(1));
			}
		}
	}
}
