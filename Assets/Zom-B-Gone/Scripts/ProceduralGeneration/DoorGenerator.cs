using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class DoorGenerator : MonoBehaviour
{
    [Header("Right click and select 'Generate Door' to generate")]
    public GameObject windowPrefab;
    public Tilemap tilemap;
    public TileBase referencedTile;

    [ContextMenu("Generate Doors")]
    public void GenerateWindows()
    {
        BoundsInt bounds = tilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null && tile == referencedTile)
            {
                Matrix4x4 matrix = tilemap.GetTransformMatrix(pos);
                Quaternion rotation = matrix.rotation;
                Vector3 eulerRotation = rotation.eulerAngles;

                Vector3 localPos = tilemap.CellToLocal(pos);

                GameObject thisWindow = Instantiate(windowPrefab, transform);
                thisWindow.transform.localPosition = localPos;
                thisWindow.transform.localRotation = rotation;

                AdjustDoorPosition(thisWindow, eulerRotation.z);
            }
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    private void AdjustDoorPosition(GameObject window, float zRot)
    {
        if (zRot == 0)
        {
            window.transform.localPosition += new Vector3(0f, 0.1f, 0);
        }
        else if (zRot == 90)
        {
            window.transform.localPosition += new Vector3(0.9f, 0f, 0);
        }
        else if (zRot == -90 || zRot == 270)
        {
            window.transform.localPosition += new Vector3(0.1f, 0f, 0);
        }
        else if (zRot == 180)
        {
            window.transform.localPosition += new Vector3(1f, 0.9f, 0);
        }
    }
}
