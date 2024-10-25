using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorLayoutDimensionSetter : MonoBehaviour
{
    public FloorLayout floorLayout;

    [ContextMenu("Set Dimensions")]
    public void SetDimensions()
    {
        floorLayout.dimensions = new Vector2(0, 0);

        BoundsInt bounds = floorLayout.tilemap.cellBounds;
        int i = 0;
        while(true)
        {
            TileBase tile = floorLayout.tilemap.GetTile(new Vector3Int(i, 0, 0));
            
            if (tile != null)
            {
                floorLayout.dimensions.x++;
                i++;
            }
            else break;
        }
        i = 0;
        while(true)
        {
            TileBase tile = floorLayout.tilemap.GetTile(new Vector3Int(0, i, 0));
            if (tile != null)
            {
                floorLayout.dimensions.y++;
                i++;
            }
            else break;
        }
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }
}
