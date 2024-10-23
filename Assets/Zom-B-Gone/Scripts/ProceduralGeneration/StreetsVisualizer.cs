using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class StreetsVisualizer : MonoBehaviour
{
    public Tilemap tilemap;
    public TileModule tileModule;

    public void DrawModuleToTilemap(Vector2Int chunkCoords, TileModule module)
    {
        if (tilemap == null)
        {
            Debug.LogWarning("Tilemap not assigned");
            return;
        }

        for (int y = 0; y < module.height; y++)
        {
            for (int x = 0; x < module.width; x++)
            {
                TileBase tile = module.GetTile(x, y);

                if (tile != null)
                {
                    int xOffset = chunkCoords.x * CityGenerator.chunkSize;
                    int yOffset = chunkCoords.y * CityGenerator.chunkSize;
                    tilemap.SetTile(new Vector3Int(xOffset + x, yOffset + y, 0), tile);
                }
            }
        }
    }
}
