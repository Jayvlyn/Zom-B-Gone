using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class WindowGenerator : MonoBehaviour
{
    [Header("Right click and select Generate Windows to generate")]
    public GameObject windowPrefab;
	public Tilemap tilemap;
	public TileBase referencedTile;

	[ContextMenu("Generate Windows")]
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

				AdjustWindowPosition(thisWindow, eulerRotation.z);
			}
		}

		#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(gameObject);
		#endif
	}

	private void AdjustWindowPosition(GameObject window, float zRot)
	{
		if (zRot == 0)
		{
			window.transform.localPosition += new Vector3(0.508f, 0.121f, 0);
		}
		else if (zRot == 90)
		{
			window.transform.localPosition += new Vector3(0.879f, 0.508f, 0);
		}
		else if (zRot == -90 || zRot == 270)
		{
			window.transform.localPosition += new Vector3(0.121f, 0.492f, 0);
		}
		else if (zRot == 180)
		{
			window.transform.localPosition += new Vector3(0.492f, 0.879f, 0);
		}
	}


	#region Chat gpt helper methods

	public void CheckTileAtPosition(Vector3 worldPosition)
	{
		// Convert the world position to a cell position on the Tilemap
		Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

		// Get the tile at that cell position
		TileBase tileAtPosition = tilemap.GetTile(cellPosition);

		// Compare the retrieved tile with the referenced tile
		if (tileAtPosition == referencedTile)
		{
			Debug.Log("The tile at " + cellPosition + " matches the referenced tile.");
		}
		else if (tileAtPosition == null)
		{
			Debug.Log("No tile is placed at " + cellPosition);
		}
		else
		{
			Debug.Log("The tile at " + cellPosition + " does not match the referenced tile.");
		}
	}

	public void CheckRotationAtPosition(Vector3 worldPosition)
	{
		// Convert the world position to a cell position on the Tilemap
		Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

		// Get the transform matrix of the tile at the cell position
		Matrix4x4 matrix = tilemap.GetTransformMatrix(cellPosition);

		// Extract the rotation from the matrix
		Quaternion rotation = matrix.rotation;

		// Convert to Euler angles if needed (for easier interpretation)
		Vector3 eulerRotation = rotation.eulerAngles;

		Debug.Log("Rotation at cell " + cellPosition + " is: " + eulerRotation);
	}

	void LogAllTiles()
	{
		BoundsInt bounds = tilemap.cellBounds;
		foreach (Vector3Int pos in bounds.allPositionsWithin)
		{
			TileBase tile = tilemap.GetTile(pos);
			if (tile != null)
			{
				Debug.Log("Tile at " + pos + " is: " + tile.name);
			}
		}
	}
	#endregion
}
