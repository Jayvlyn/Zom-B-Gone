using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapToModule : MonoBehaviour
{
    public Tilemap tilemap;
    public TileModule tileModule;

    [ContextMenu("Capture Tilemap")]
    public void CaptureTilemap()
    {
        if (tileModule == null || tilemap == null)
        {
            Debug.LogWarning("TileModule or Tilemap not assigned.");
            return;
        }

        tileModule.Initialize(CityGenerator.chunkSize, CityGenerator.chunkSize);

        BoundsInt bounds = tilemap.cellBounds;

        Vector3Int center = new Vector3Int(
            bounds.xMin + bounds.size.x / 2,
            bounds.yMin + bounds.size.y / 2,
            0
        );

        int halfSize = CityGenerator.chunkSize / 2;

        for (int y = 0; y < CityGenerator.chunkSize; y++)
        {
            for (int x = 0; x < CityGenerator.chunkSize; x++)
            {
                Vector3Int tilePosition = new Vector3Int(
                    center.x - halfSize + x,
                    center.y - halfSize + y,
                    0
                );


                TileBase tile = tilemap.HasTile(tilePosition) ? tilemap.GetTile(tilePosition) : null;
                tileModule.SetTile(x, y, tile);
            }
        }

#if UNITY_EDITOR
		EditorUtility.SetDirty(tileModule);
		AssetDatabase.SaveAssets();
#endif

		Debug.Log("Tilemap captured into module");
    }
}
