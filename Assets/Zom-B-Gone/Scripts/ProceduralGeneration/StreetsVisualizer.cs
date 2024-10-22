using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

public class StreetsVisualizer : MonoBehaviour
{
    public Tilemap tilemap;
    public TileModule tileModule;

    public void DrawModuleToTilemap()
    {
        if (tilemap == null || tileModule == null)
        {
            Debug.LogWarning("Tilemap or TileModule not assigned.");
            return;
        }

        // Loop through the module's width and height
        for (int y = 0; y < tileModule.height; y++)
        {
            for (int x = 0; x < tileModule.width; x++)
            {
                // Get the tile from the module
                TileBase tile = tileModule.GetTile(x, y);

                // Set the tile on the tilemap at position (x, y)
                if (tile != null)
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }

        //Debug.Log("Module drawn to the tilemap!");
    }
}
