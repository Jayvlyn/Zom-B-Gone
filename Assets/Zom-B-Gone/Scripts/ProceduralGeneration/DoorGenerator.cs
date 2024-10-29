using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class DoorGenerator : MonoBehaviour
{
    [Header("Right click and select 'Generate Doors' to generate")]
    public GameObject windowPrefab;
    public Tilemap tilemap;
    public TileBase referencedTile;
    [SerializeField] private List<GameObject> doors;

    [ContextMenu("Generate Doors")]
    public void GenerateDoors()
    {
        doors = new List<GameObject>();
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

#if UNITY_EDITOR
                GameObject thisWindow = (GameObject)PrefabUtility.InstantiatePrefab(windowPrefab, transform);
#else
                GameObject thisWindow = Instantiate(windowPrefab, transform);
#endif

                thisWindow.transform.localPosition = localPos;
                thisWindow.transform.localRotation = rotation;

                AdjustDoorPosition(thisWindow, eulerRotation.z);

                doors.Add(thisWindow);
                thisWindow.SetActive(false);
            }
        }

#if UNITY_EDITOR
        // Mark this GameObject as dirty so changes are saved to the prefab
        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
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

    public void ActivateDoors()
    {
        foreach (GameObject door in doors)
        {
            door.SetActive(true);
        }
    }
}
