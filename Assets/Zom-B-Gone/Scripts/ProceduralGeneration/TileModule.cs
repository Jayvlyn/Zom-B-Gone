using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Tile Module", menuName = "New Tile Module")]
public class TileModule : ScriptableObject
{
    public int width = 11;
    public int height = 11;
    public TileBase[] tiles; // Flat 1D array to store tile data

    public void Initialize(int w, int h)
    {
        width = w;
        height = h;
        tiles = new TileBase[width * height];
    }

    public void SetTile(int x, int y, TileBase tile)
    {
        tiles[y * width + x] = tile;
    }

    public TileBase GetTile(int x, int y)
    {
        return tiles[y * width + x];
    }
}
