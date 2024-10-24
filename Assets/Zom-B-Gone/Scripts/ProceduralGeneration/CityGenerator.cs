using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CityGenerator : MonoBehaviour
{
    public StreetsVisualizer streetsVisualizer;
	public static int chunkSize = 11;

    private int chunkDistance = 10; // how many chunks loaded in each direction from player

    // Player vars
	public Transform playerT;

    private Vector2Int currentChunk;
    public Vector2Int CurrentChunk
    {
        get { return currentChunk; }
        set
        {
            if (value == currentChunk) return;
            else
            {
                currentChunk = value;
                // check for missing chunks in distance from new chunk
            }
        }
    }

    public Dictionary<Vector2Int, ChunkData> loadedChunks;

    private void Update()
    {
        CurrentChunk = new Vector2Int(
            Mathf.FloorToInt(playerT.position.x / chunkSize),
            Mathf.FloorToInt(playerT.position.y / chunkSize)
        );
        //Debug.Log(currentChunk.ToString());
    }

    private void Start()
    {
        loadedChunks = new Dictionary<Vector2Int, ChunkData>();

        // starting chunk
        GenerateChunk(new Vector2Int(0, 0), ModuleType.VERTICAL);

        // surrounding starter chunks
        LoadSurroundingChunks();

    }

    private void LoadSurroundingChunks()
    {
        int radius = 1; // Start with a 1-chunk radius around (0, 0)

        while (radius <= chunkDistance)
        {
            // Step 1: Top and Bottom edges (excluding corners)
            for (int x = -radius + 1; x <= radius - 1; x++)
            {
                GenerateChunk(new Vector2Int(x, radius));  // Top edge (excluding corners)
                //Debug.Log("Generating at " + x + ", " + radius);
                GenerateChunk(new Vector2Int(x, -radius)); // Bottom edge (excluding corners)
                //Debug.Log("Generating at " + x + ", " + -radius);
            }

            // Step 2: Left and Right edges (excluding corners)
            for (int y = -radius + 1; y <= radius - 1; y++)
            {
                GenerateChunk(new Vector2Int(-radius, y)); // Left edge (excluding corners)
                //Debug.Log("Generating at " + -radius + ", " + y);
                GenerateChunk(new Vector2Int(radius, y));  // Right edge (excluding corners)
                //Debug.Log("Generating at " + radius + ", " + y);
            }

            // Step 3: Generate the four corners last
            GenerateChunk(new Vector2Int(-radius, radius));   // Top-left corner
            //Debug.Log("Generating at " + -radius + ", " + radius);
            GenerateChunk(new Vector2Int(radius, radius));    // Top-right corner
            //Debug.Log("Generating at " + radius + ", " + radius);
            GenerateChunk(new Vector2Int(-radius, -radius));  // Bottom-left corner
            //Debug.Log("Generating at " + -radius + ", " + -radius);
            GenerateChunk(new Vector2Int(radius, -radius));   // Bottom-right corner
            //Debug.Log("Generating at " + radius + ", " + -radius);

            radius++; // Increase the radius to load the next layer of chunks
        }
    }

    public void GenerateChunk(Vector2Int chunkPos)
    {
        ModuleType modType = DetermineModule(chunkPos);
        TileModule mod = streetsVisualizer.grassModule;
        switch (modType)
        {
            case ModuleType.HORIZONTAL:
                mod = streetsVisualizer.horizontalModule;
                break;
            case ModuleType.VERTICAL:
                mod = streetsVisualizer.verticalModule;
                break;
            case ModuleType.NORTH_WEST_TURN:
                mod = streetsVisualizer.northWestTurnModule;
                break;
            case ModuleType.NORTH_EAST_TURN:
                mod = streetsVisualizer.northEastTurnModule;
                break;
            case ModuleType.SOUTH_WEST_TURN:
                mod = streetsVisualizer.southWestTurnModule;
                break;
            case ModuleType.SOUTH_EAST_TURN:
                mod = streetsVisualizer.southEastTurnModule;
                break;
            case ModuleType.INTERSECTION_4:
                mod = streetsVisualizer.intersection4Module;
                break;
            case ModuleType.INTERSECTION_3_NORTH:
                mod = streetsVisualizer.intersection3NorthModule;
                break;
            case ModuleType.INTERSECTION_3_SOUTH:
                mod = streetsVisualizer.intersection3SouthModule;
                break;
            case ModuleType.INTERSECTION_3_WEST:
                mod = streetsVisualizer.intersection3WestModule;
                break;
            case ModuleType.INTERSECTION_3_EAST:
                mod = streetsVisualizer.intersection3EastModule;
                break;
        }

        ChunkData chunkData = new ChunkData(mod, modType);

        streetsVisualizer.DrawModuleToTilemap(chunkPos, mod);
        loadedChunks.Add(chunkPos, chunkData);
    }

    public void GenerateChunk(Vector2Int chunkPos, ModuleType modType)
    {
        TileModule mod = streetsVisualizer.grassModule;
        switch (modType)
        {
            case ModuleType.HORIZONTAL:
                mod = streetsVisualizer.horizontalModule;
                break;
            case ModuleType.VERTICAL:
                mod = streetsVisualizer.verticalModule;
                break;
            case ModuleType.NORTH_WEST_TURN:
                mod = streetsVisualizer.northWestTurnModule;
                break;
            case ModuleType.NORTH_EAST_TURN:
                mod = streetsVisualizer.northEastTurnModule;
                break;
            case ModuleType.SOUTH_WEST_TURN:
                mod = streetsVisualizer.southWestTurnModule;
                break;
            case ModuleType.SOUTH_EAST_TURN:
                mod = streetsVisualizer.southEastTurnModule;
                break;
            case ModuleType.INTERSECTION_4:
                mod = streetsVisualizer.intersection4Module;
                break;
            case ModuleType.INTERSECTION_3_NORTH:
                mod = streetsVisualizer.intersection3NorthModule;
                break;
            case ModuleType.INTERSECTION_3_SOUTH:
                mod = streetsVisualizer.intersection3SouthModule;
                break;
            case ModuleType.INTERSECTION_3_WEST:
                mod = streetsVisualizer.intersection3WestModule;
                break;
            case ModuleType.INTERSECTION_3_EAST:
                mod = streetsVisualizer.intersection3EastModule;
                break;
        }

        ChunkData chunkData = new ChunkData(mod, modType);

        streetsVisualizer.DrawModuleToTilemap(chunkPos, mod);
        loadedChunks.Add(chunkPos, chunkData);
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

        int bitmask = 0;
        if (aboveModule != ModuleType.EMPTY) bitmask |= 1;
        if (belowModule != ModuleType.EMPTY) bitmask |= 2;
        if (leftModule != ModuleType.EMPTY) bitmask |= 4;
        if (rightModule != ModuleType.EMPTY) bitmask |= 8;


        // Many of these cases should never occur during generation, at most two generated chunks should be adjacent to this new chunk
        switch (bitmask)
        {
            case 0b0000:  // surrounded by empty chunks
                return ModuleType.GRASS;

            case 0b0001:  // connection above
                if (aboveModule == ModuleType.VERTICAL)
                {
                    // vertical road or intersection that connects up
                    ModuleType[] possibleModules = new ModuleType[7];
                    possibleModules[0] = ModuleType.VERTICAL;
                    possibleModules[1] = ModuleType.NORTH_EAST_TURN;
                    possibleModules[2] = ModuleType.NORTH_WEST_TURN;
                    possibleModules[3] = ModuleType.INTERSECTION_4;
                    possibleModules[4] = ModuleType.INTERSECTION_3_SOUTH;
                    possibleModules[5] = ModuleType.INTERSECTION_3_WEST;
                    possibleModules[6] = ModuleType.INTERSECTION_3_EAST;
                    return PickRandomModule(possibleModules);
                }
                else if (!ModuleLeadsDown(aboveModule))
                { 
                    return ModuleType.GRASS;
                }
                else
                {
                    return ModuleType.VERTICAL;
                }

            case 0b0010:  // connection below
                if (belowModule == ModuleType.VERTICAL)
                {
                    // vertical road or intersection that connects down
                    ModuleType[] possibleModules = new ModuleType[7];
                    possibleModules[0] = ModuleType.VERTICAL;
                    possibleModules[1] = ModuleType.SOUTH_EAST_TURN;
                    possibleModules[2] = ModuleType.SOUTH_WEST_TURN;
                    possibleModules[3] = ModuleType.INTERSECTION_4;
                    possibleModules[4] = ModuleType.INTERSECTION_3_NORTH;
                    possibleModules[5] = ModuleType.INTERSECTION_3_WEST;
                    possibleModules[6] = ModuleType.INTERSECTION_3_EAST;
                    return PickRandomModule(possibleModules);
                }
                else if (!ModuleLeadsUp(belowModule))
                {
                    return ModuleType.GRASS;
                }
                else
                {
                    return ModuleType.VERTICAL;
                }

            case 0b0100:  // connection left
                if (leftModule == ModuleType.HORIZONTAL)
                {
                    // horizontal road or intersection that connects left
                    ModuleType[] possibleModules = new ModuleType[7];
                    possibleModules[0] = ModuleType.HORIZONTAL;
                    possibleModules[1] = ModuleType.SOUTH_WEST_TURN;
                    possibleModules[2] = ModuleType.NORTH_WEST_TURN;
                    possibleModules[3] = ModuleType.INTERSECTION_4;
                    possibleModules[4] = ModuleType.INTERSECTION_3_SOUTH;
                    possibleModules[5] = ModuleType.INTERSECTION_3_NORTH;
                    possibleModules[6] = ModuleType.INTERSECTION_3_EAST;
                    return PickRandomModule(possibleModules);
                }
                else if (!ModuleLeadsRight(leftModule)) // cant lead to road on right
                {
                    return ModuleType.GRASS;
                }
                else 
                {
                    return ModuleType.HORIZONTAL;
                }

            case 0b1000:  // connection right
                if (rightModule == ModuleType.HORIZONTAL)
                {
                    // horizontal road or intersection that connects right
                    ModuleType[] possibleModules = new ModuleType[7];
                    possibleModules[0] = ModuleType.HORIZONTAL;
                    possibleModules[1] = ModuleType.SOUTH_EAST_TURN;
                    possibleModules[2] = ModuleType.NORTH_EAST_TURN;
                    possibleModules[3] = ModuleType.INTERSECTION_4;
                    possibleModules[4] = ModuleType.INTERSECTION_3_SOUTH;
                    possibleModules[5] = ModuleType.INTERSECTION_3_NORTH;
                    possibleModules[6] = ModuleType.INTERSECTION_3_WEST;
                    return PickRandomModule(possibleModules);

                }
                else if (!ModuleLeadsLeft(rightModule)) // cant lead to road on left
                {
                    return ModuleType.GRASS;
                }
                else
                {
                    return ModuleType.HORIZONTAL;
                }

            case 0b0011:  // connection above + below
                Debug.Log("case shouldn't happen during generation");
                break;

            case 0b1100:  // connection left + right
                Debug.Log("case shouldn't happen during generation");
                break;

            case 0b1001:  // connection above + right
                {
                    bool aboveLeadsDown = ModuleLeadsDown(aboveModule);
                    bool rightLeadsLeft = ModuleLeadsLeft(rightModule);
                    if (aboveLeadsDown && rightLeadsLeft)
                    {
                        // intersection at least up and right
                        ModuleType[] possibleModules = new ModuleType[4];
                        possibleModules[0] = ModuleType.NORTH_EAST_TURN;
                        possibleModules[1] = ModuleType.INTERSECTION_4;
                        possibleModules[2] = ModuleType.INTERSECTION_3_SOUTH;
                        possibleModules[3] = ModuleType.INTERSECTION_3_WEST;
                        return PickRandomModule(possibleModules);
                    }
                    else if (aboveLeadsDown)
                    {
                        // at least up and not right
                        ModuleType[] possibleModules = new ModuleType[3];
                        possibleModules[0] = ModuleType.VERTICAL;
                        possibleModules[1] = ModuleType.NORTH_WEST_TURN;
                        possibleModules[2] = ModuleType.INTERSECTION_3_EAST;
                        return PickRandomModule(possibleModules);
                    }
                    else if (rightLeadsLeft)
                    {
                        // at least right and not up
                        ModuleType[] possibleModules = new ModuleType[3];
                        possibleModules[0] = ModuleType.HORIZONTAL;
                        possibleModules[1] = ModuleType.SOUTH_EAST_TURN;
                        possibleModules[2] = ModuleType.INTERSECTION_3_NORTH;
                        return PickRandomModule(possibleModules);
                    }
                    else
                    {
                        return ModuleType.GRASS;
                    }
                }

            case 0b0101:  // connection above + left
                {
                    bool aboveLeadsDown = ModuleLeadsDown(aboveModule);
                    bool leftLeadsRight = ModuleLeadsRight(leftModule);
                    if (aboveLeadsDown && leftLeadsRight)
                    {
                        // intersection at least up and left
                        ModuleType[] possibleModules = new ModuleType[4];
                        possibleModules[0] = ModuleType.NORTH_WEST_TURN;
                        possibleModules[1] = ModuleType.INTERSECTION_4;
                        possibleModules[2] = ModuleType.INTERSECTION_3_SOUTH;
                        possibleModules[3] = ModuleType.INTERSECTION_3_EAST;
                        return PickRandomModule(possibleModules);
                    }
                    else if (aboveLeadsDown)
                    {
                        // at least up and not left
                        ModuleType[] possibleModules = new ModuleType[3];
                        possibleModules[0] = ModuleType.VERTICAL;
                        possibleModules[1] = ModuleType.NORTH_EAST_TURN;
                        possibleModules[2] = ModuleType.INTERSECTION_3_WEST;
                        return PickRandomModule(possibleModules);
                    }
                    else if (leftLeadsRight)
                    {
                        // at least left and not up
                        ModuleType[] possibleModules = new ModuleType[3];
                        possibleModules[0] = ModuleType.HORIZONTAL;
                        possibleModules[1] = ModuleType.SOUTH_WEST_TURN;
                        possibleModules[2] = ModuleType.INTERSECTION_3_NORTH;
                        return PickRandomModule(possibleModules);
                    }
                    else
                    {
                        return ModuleType.GRASS;
                    }
                }

            case 0b1010:  // connection below + right
                {
                    bool belowLeadsUp = ModuleLeadsUp(belowModule);
                    bool rightLeadsLeft = ModuleLeadsLeft(rightModule);
                    if (belowLeadsUp && rightLeadsLeft)
                    {
                        // intersection at least down and right
                        ModuleType[] possibleModules = new ModuleType[4];
                        possibleModules[0] = ModuleType.SOUTH_EAST_TURN;
                        possibleModules[1] = ModuleType.INTERSECTION_4;
                        possibleModules[2] = ModuleType.INTERSECTION_3_NORTH;
                        possibleModules[3] = ModuleType.INTERSECTION_3_WEST;
                        return PickRandomModule(possibleModules);
                    }
                    else if (belowLeadsUp)
                    {
                        // at least down and not right
                        ModuleType[] possibleModules = new ModuleType[3];
                        possibleModules[0] = ModuleType.VERTICAL;
                        possibleModules[1] = ModuleType.SOUTH_WEST_TURN;
                        possibleModules[2] = ModuleType.INTERSECTION_3_EAST;
                        return PickRandomModule(possibleModules);
                    }
                    else if (rightLeadsLeft)
                    {
                        // at least right and not down
                        ModuleType[] possibleModules = new ModuleType[3];
                        possibleModules[0] = ModuleType.HORIZONTAL;
                        possibleModules[1] = ModuleType.NORTH_EAST_TURN;
                        possibleModules[2] = ModuleType.INTERSECTION_3_SOUTH;
                        return PickRandomModule(possibleModules);
                    }
                    else
                    {
                        return ModuleType.GRASS;
                    }
                }


            case 0b0110:  // connection below + left
                {
                    bool belowLeadsUp = ModuleLeadsUp(belowModule);
                    bool leftLeadsRight = ModuleLeadsRight(leftModule);
                    if (belowLeadsUp && leftLeadsRight)
                    {
                        // intersection at least down and left
                        ModuleType[] possibleModules = new ModuleType[4];
                        possibleModules[0] = ModuleType.SOUTH_WEST_TURN;
                        possibleModules[1] = ModuleType.INTERSECTION_4;
                        possibleModules[2] = ModuleType.INTERSECTION_3_NORTH;
                        possibleModules[3] = ModuleType.INTERSECTION_3_EAST;
                        return PickRandomModule(possibleModules);
                    }
                    else if (belowLeadsUp)
                    {
                        // at least down and not left
                        ModuleType[] possibleModules = new ModuleType[3];
                        possibleModules[0] = ModuleType.VERTICAL;
                        possibleModules[1] = ModuleType.SOUTH_EAST_TURN;
                        possibleModules[2] = ModuleType.INTERSECTION_3_WEST;
                        return PickRandomModule(possibleModules);
                    }
                    else if (leftLeadsRight)
                    {
                        // at least left and not down
                        ModuleType[] possibleModules = new ModuleType[3];
                        possibleModules[0] = ModuleType.HORIZONTAL;
                        possibleModules[1] = ModuleType.NORTH_WEST_TURN;
                        possibleModules[2] = ModuleType.INTERSECTION_3_SOUTH;
                        return PickRandomModule(possibleModules);
                    }
                    else
                    {
                        return ModuleType.GRASS;
                    }
                }


            case 0b1111:  // all connected
                Debug.Log("case shouldn't happen during generation");
                break;

            case 0b1110:  // no above connection
                Debug.Log("case shouldn't happen during generation");
                break;

            case 0b1101:  // no below connection
                Debug.Log("case shouldn't happen during generation");
                break;

            case 0b1011:  // no left connection
                Debug.Log("case shouldn't happen during generation");
                break;

            case 0b0111:  // no right connection
                Debug.Log("case shouldn't happen during generation");
                break;

            default:
                Debug.Log("defaulting");
                return ModuleType.GRASS;
        }
        Debug.Log("return out of case");
        return ModuleType.GRASS;
	}

    public ModuleType PickRandomModule(ModuleType[] modules)
    {
        int roll = Random.Range(0, modules.Length);
        return modules[roll];
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

    public bool ModuleLeadsDown(ModuleType module)
    {
        return !(module == ModuleType.GRASS || module == ModuleType.HORIZONTAL || module == ModuleType.NORTH_EAST_TURN || module == ModuleType.NORTH_WEST_TURN || module == ModuleType.INTERSECTION_3_SOUTH);
    }

    public bool ModuleLeadsUp(ModuleType module)
    {
        return !(module == ModuleType.GRASS || module == ModuleType.HORIZONTAL || module == ModuleType.SOUTH_EAST_TURN || module == ModuleType.SOUTH_WEST_TURN || module == ModuleType.INTERSECTION_3_NORTH);
    }

    public bool ModuleLeadsRight(ModuleType module)
    {
        return !(module == ModuleType.GRASS || module == ModuleType.VERTICAL || module == ModuleType.SOUTH_WEST_TURN || module == ModuleType.NORTH_WEST_TURN || module == ModuleType.INTERSECTION_3_EAST);
    }

    public bool ModuleLeadsLeft(ModuleType module)
    {
        return !(module == ModuleType.GRASS || module == ModuleType.VERTICAL || module == ModuleType.SOUTH_EAST_TURN || module == ModuleType.NORTH_EAST_TURN || module == ModuleType.INTERSECTION_3_WEST);
    }
}
public struct ChunkData
{
    public TileModule module;
    public ModuleType type;
    public ChunkData(TileModule module, ModuleType type)
    {
        this.module = module;
        this.type = type;
    }
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
    