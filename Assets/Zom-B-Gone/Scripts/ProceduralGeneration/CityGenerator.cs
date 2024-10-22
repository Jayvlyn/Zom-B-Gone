using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
	public Transform playerT;
    public StreetsVisualizer streetsVisualizer;

	public int chunkSize = 11;

    Vector2Int currentChunk;

    private void Update()
    {
        currentChunk = new Vector2Int(
            Mathf.FloorToInt(playerT.position.x / chunkSize),
            Mathf.FloorToInt(playerT.position.y / chunkSize)
        );
        //Debug.Log(currentChunk.ToString());
    }

    private void Start()
    {
        streetsVisualizer.DrawModuleToTilemap();
    }
}
