using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapToModule : MonoBehaviour
{
    public Tilemap tilemap;           // Reference to the tilemap in the scene
    public TileModule tileModule;     // The TileModule ScriptableObject to save to
    public int moduleSize = 11;       // Fixed size of the module (e.g., 11x11)


    [ContextMenu("Capture Tilemap")]
    public void CaptureTilemap()
    {
        if (tileModule == null || tilemap == null)
        {
            Debug.LogWarning("TileModule or Tilemap not assigned.");
            return;
        }

        tileModule.Initialize(moduleSize, moduleSize);

        // Get the bounds of the tilemap
        BoundsInt bounds = tilemap.cellBounds;

        // Find the center of the bounds so we can extract the center of the tilemap
        Vector3Int center = new Vector3Int(
            bounds.xMin + bounds.size.x / 2,
            bounds.yMin + bounds.size.y / 2,
            0
        );

        // Start from the center and capture an 11x11 area
        int halfSize = moduleSize / 2;

        for (int y = 0; y < moduleSize; y++)
        {
            for (int x = 0; x < moduleSize; x++)
            {
                // Calculate the tile position relative to the center
                Vector3Int tilePosition = new Vector3Int(
                    center.x - halfSize + x,
                    center.y - halfSize + y,
                    0
                );

                // Capture the tile or set to null if out of bounds
                TileBase tile = tilemap.HasTile(tilePosition) ? tilemap.GetTile(tilePosition) : null;
                tileModule.SetTile(x, y, tile); // Save to TileModule
            }
        }

#if UNITY_EDITOR
		EditorUtility.SetDirty(tileModule); // Mark the ScriptableObject as dirty
		AssetDatabase.SaveAssets();         // Save changes to disk
#endif

		Debug.Log("Tilemap captured into module");
    }
}
