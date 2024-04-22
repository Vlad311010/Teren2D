using Structs;
using System;
using System.Collections.Generic;
using System.IO;
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

    [SerializeField] Vector2Int riverStart;
    [SerializeField] Vector2Int rivereEnd;


    private Vector3 origin;
    private GridSystem<TerrainCell> grid;
    private float[,] noiseMap;
    private System.Random rng;

    private List<TerrainCell> path;

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
        LayoutGeneration();
        LayoutPostprocessing();
        PathsGeneration();
        FillTilemaps();
    }

    protected void NoiseGeneration()
    {
        noiseMap = PerlinNoise.GetNoiseMap(rng, size.x, size.y, offset, scale, octaves, persistance, lacunarity);
    }

    protected void LayoutGeneration()
    {
        grid = new GridSystem<TerrainCell>(size.x, size.y, cellSize, origin, CreateTerrainCell);
    }


    protected void LayoutPostprocessing()
    {
        /*Debug.Log(Pathfinding.FindPath<TerrainCell>(grid, grid.GetCell(riverStart), grid.GetCell(rivereEnd), out path));
        foreach (TerrainCell cell in path)
        {
            cell.NoiseValue = -1;
        }*/
    }

    protected void PathsGeneration()
    {

    }

    protected void FillTilemaps()
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                float noiseValue = grid.GetCell(x, y).NoiseValue;
                NoiseClampData clampData = CalculateTileType(noiseValue, tiles);

                Tilemap placeOn;
                TileBase tile;
                if (clampData.layer <= 0)
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

    private TerrainCell CreateTerrainCell(GridSystem<TerrainCell> grid, int x, int y)
    {
        return new TerrainCell(grid, x, y, noiseMap[x, y]);
    }

    private NoiseClampData CalculateTileType(float noiseValue, NoiseClampData[] clampData)
    {
        for (int i = 0; i < clampData.Length; i++)
        {
            if (noiseValue <= clampData[i].clampValue)
                return clampData[i];
        }

        return clampData.Last();
    }

    private void OnValidate()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].layer = Math.Clamp(tiles[i].layer, 0, layers.Length - 1);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position;
        Gizmos.DrawWireCube(new Vector3(origin.x + (size.x / 2f), origin.y + (size.y / 2f), origin.z), new Vector3(size.x, size.y));

        if (grid == null) return;

        grid.DebugDraw();

        if (path != null)
        {
            Gizmos.color = Color.yellow;
            foreach (GridCellBase cell in path)
            {
                Gizmos.DrawCube(grid.GetWorldPosition(cell.Coordinates), Vector3.one);
            }

        }
    }
}
