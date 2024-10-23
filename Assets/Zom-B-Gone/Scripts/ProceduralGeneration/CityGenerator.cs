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

    /// <summary>
    /// Determines what module should be used at the given position, based on the adjacent chunks. Contains random decisionmaking when multiple modules are compatible
    /// Intersections and turns cannot but directly adjacent to eachother, meaning when a surrounding chunk is a turn or intersection, it will become a straight road
    /// </summary>
    /// <param name="chunkPos">Position of the chunk to find a module for</param>
    /// <returns>The decided module suitable for the chunk position</returns>
    public ModuleType DetermineModule(Vector2Int chunkPos)
    {
        ModuleType aboveModule = CheckModuleAbove(chunkPos);
        ModuleType belowModule = CheckModuleBelow(chunkPos);
        ModuleType leftModule = CheckModuleLeft(chunkPos);
        ModuleType rightModule = CheckModuleRight(chunkPos);



        return ModuleType.GRASS;
	}

    public ModuleType CheckModuleAbove(Vector2Int chunkPos)
    {
        Vector2Int aboveCoord = new Vector2Int(chunkPos.x, chunkPos.y + 1);
        if (loadedChunks.ContainsKey(aboveCoord)) return loadedChunks[aboveCoord].type;
        else return ModuleType.EMPTY;
    }

	public ModuleType CheckModuleBelow(Vector2Int chunkPos)
	{
		Vector2Int belowCoord = new Vector2Int(chunkPos.x, chunkPos.y - 1);
		if (loadedChunks.ContainsKey(belowCoord)) return loadedChunks[belowCoord].type;
		else return ModuleType.EMPTY;
	}

	public ModuleType CheckModuleLeft(Vector2Int chunkPos)
	{
		Vector2Int leftCoord = new Vector2Int(chunkPos.x - 1, chunkPos.y);
		if (loadedChunks.ContainsKey(leftCoord)) return loadedChunks[leftCoord].type;
		else return ModuleType.EMPTY;
	}

	public ModuleType CheckModuleRight(Vector2Int chunkPos)
	{
		Vector2Int rightCoord = new Vector2Int(chunkPos.x + 1, chunkPos.y);
		if (loadedChunks.ContainsKey(rightCoord)) return loadedChunks[rightCoord].type;
		else return ModuleType.EMPTY;
	}


}

public struct ChunkData
{
    public TileModule module;
    public ModuleType type;
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
    GRASS,
    EMPTY
}
    