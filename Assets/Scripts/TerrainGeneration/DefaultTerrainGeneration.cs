using Structs;
using System;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DefaultTerrainGeneration : MonoBehaviour // TerrainGenerationBase
{

    [Header("Grid settings")]
    [SerializeField] Vector2Int size;
    [SerializeField] float cellSize;

    [Header("Noise settings")]
    [SerializeField] Vector2 offset;
    [SerializeField] float scale;
    [SerializeField] int octaves;
    [SerializeField] float persistance;
    [SerializeField] float lacunarity;
    [SerializeField] int seed;


    [Header("Tilemaps")]
    [SerializeField] Tilemap[] layers;

    [Header("Tiles")]
    [SerializeField] Tile defaultGroundTile;
    [SerializeField] NoiseClampData[] tiles;


    private Vector3 origin;
    private GridSystem<TerrainCell> grid;
    private float[,] noiseMap;
    private System.Random rng;

    private void Awake()
    {
        origin = transform.position;
        Generate();
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
        rng = new System.Random(seed);
        foreach (Tilemap tilemapLayer in layers) 
        {
            tilemapLayer.ClearAllTiles();
        }
        NoiseGeneration();
        GridGeneration();
        LayoutGeneration();
    }

    public void NoiseGeneration()
    {
        noiseMap = PerlinNoise.GetNoiseMap(rng, size.x, size.y, offset, scale, octaves, persistance, lacunarity);
    }

    public void GridGeneration()
    {
        grid = new GridSystem<TerrainCell>(size.x, size.y, cellSize, origin, (GridSystem<TerrainCell> grid, int x, int y) => new TerrainCell(grid, x, y, noiseMap[x, y]));
    }

    public void LayoutGeneration()
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                float noiseValue = grid.GetCell(x, y).NoiseValue;
                NoiseClampData clampData = CalculateTileType(noiseValue, tiles);

                Tilemap placeOn;
                TileBase tile;
                if (clampData.layer == 0)
                {
                    placeOn = layers[0];
                    tile = clampData.GetRandomTile(rng);
                    TilemapUtils.SetTile(placeOn, grid.GetWorldPosition(x, y), tile);
                }
                else
                {
                    placeOn = layers[clampData.layer];
                    tile = clampData.GetRandomTile(rng);
                    TilemapUtils.SetTile(layers[0], grid.GetWorldPosition(x, y), defaultGroundTile); // set ground tile 
                    TilemapUtils.SetTile(placeOn, grid.GetWorldPosition(x, y), tile); // set tile on proper layer
                }
            }
        }
    }

    public void AdditionalLayoutGeneration()
    {
    }

    public void PathsGeneration()
    {

    }

    private NoiseClampData CalculateTileType(float noiseValue, NoiseClampData[] clampData)
    {
        for (int i = 0; i < clampData.Length; i++)
        {
            if (noiseValue < clampData[i].clampValue)
                return clampData[i];
        }

        return clampData.Last();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position;
        Gizmos.DrawWireCube(new Vector3(origin.x + (size.x / 2f), origin.y + (size.y / 2f), origin.z), new Vector3(size.x, size.y));

        if (grid == null) return;

        grid.DebugDraw();
    }
}
