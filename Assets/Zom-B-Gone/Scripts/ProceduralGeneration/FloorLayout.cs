using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
public class FloorLayout : MonoBehaviour
{
	public Vector2 dimensions; // w, h
	public Tilemap tilemap;
    
    public List<Transform> collectibleSpawns = new List<Transform>();

	private void OnTransformChildrenChanged()
    {
        collectibleSpawns.Clear();
        foreach (Transform child in transform)
        {
            if(child.gameObject.CompareTag("CollectibleSpawn"))
            {
                collectibleSpawns.Add(child); 
            }
        }
    }
}
