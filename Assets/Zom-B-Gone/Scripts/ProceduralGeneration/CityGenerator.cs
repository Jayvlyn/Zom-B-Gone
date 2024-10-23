using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{
    public StreetsVisualizer streetsVisualizer;
	public static int chunkSize = 11;

    // Player vars
	public Transform playerT;
    public Vector2Int currentChunk;

    public Dictionary<Vector2Int, ChunkData> loadedChunks;

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
    }

    public void GenerateChunk(Vector2Int chunkPos)
    {
        loadedChunks.Add(chunkPos, new ChunkData());

        streetsVisualizer.DrawModuleToTilemap(new Vector2Int(1,0), streetsVisualizer.tileModule);
        
    }
}

public struct ChunkData
{
    TileModule module;
    ModuleType type;
}

public enum ModuleType
{
    HORIZONTAL,
    VERTICAL,
    NORTH_WEST_TURN,
    NORTH_EAST_TURN,
    SOUTH_WEST_TURN,
    SOUTH_EAST_TURN,
    INTERSECTION_4,
    INTERSECTION_3_NORTH, // no north road
    INTERSECTION_3_SOUTH, // no south road
    INTERSECTION_3_WEST,  // no west road
    INTERSECTION_3_EAST,  // no east road
    GRASS
}
    