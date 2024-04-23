using Structs;
using System;
using System.Collections.Generic;
using System.Linq;
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

    [Header("River settings")]
    [SerializeField] int riverAffectionRange;
    [SerializeField] int riverMinimalWidth;


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

    #region GeneratorFunctions

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
        Debug.Log("Valid path: " + Pathfinding.FindPath<TerrainCell>(grid, grid.GetCell(riverStart), grid.GetCell(rivereEnd), out path));
        FormRiver(path, riverAffectionRange);
        /*foreach (TerrainCell cell in path)
        {
            ChangeNoiseValueWithSpread(cell, -1, riverWidth, 
                (md, d, n) =>
                {
                    return Math.Clamp(n / 2, -1, -0.15f);
                });
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
    #endregion


    #region RiverCreating
    private class RiverAffectedCellData
    {
        public TerrainCell cell;
        public int distanceFromMainPath;

        public RiverAffectedCellData(TerrainCell cell, int distance)
        {
            this.cell = cell;
            distanceFromMainPath = distance;
        }


        public override bool Equals(object obj)
        {
            return cell.Equals(obj);
        }

        public override int GetHashCode()
        {
            return cell.GetHashCode();
        }
    }

    private void FormRiver(List<TerrainCell> riverPath, int riverWidth)
    {
        List<RiverAffectedCellData> riverAffectedCells = CollectRiverAffectedCells(riverPath, riverWidth);
        foreach (RiverAffectedCellData cell in riverAffectedCells)
        {
            
            if (cell.distanceFromMainPath < riverMinimalWidth)
            {
                cell.cell.NoiseValue = -1;
            }
            else
            {
                cell.cell.NoiseValue -= rng.UnitInterval() / 8;
            }
            
        }

        /*List<TerrainCell> affectedCells = new List<TerrainCell>(riverPath);
        List<TerrainCell> closedList = new List<TerrainCell>();

        while (affectedCells.Count > 0)
        {
            TerrainCell cell = affectedCells[0];
            if (!closedList.Contains(cell))
            {
                ChangeNoiseValueWithSpread(cell, -1, riverWidth,
                    (maxDepth, depth, noiseValue) =>
                    {
                        if (depth < maxDepth - 3)
                            return -rng.UnitInterval() / 10;
                        else
                            return noiseValue;
                        return Math.Clamp(noiseValue / 2, -1, -0.01f);
                    });

                closedList.Add(cell);
            }

            affectedCells.Remove(cell);
        }*/
    }

    private List<RiverAffectedCellData> CollectRiverAffectedCells(List<TerrainCell> riverPath, int maxDepth)
    {
        void CollectAffectedCells(List<RiverAffectedCellData> riverAffectedCells, TerrainCell cell, int depth)
        {
            if (depth == 0) return;

            int distance = maxDepth - depth;
            RiverAffectedCellData collectedRiverCellData = riverAffectedCells.FirstOrDefault(c => c.cell.Equals(cell));
            // Debug.Log((collectedRiverCellData == null) + " " + riverAffectedCells.Count);

            if (collectedRiverCellData == null|| (collectedRiverCellData.distanceFromMainPath > distance))
            {
                if (collectedRiverCellData != null)
                    collectedRiverCellData.distanceFromMainPath = distance;
                else
                {
                    riverAffectedCells.Add(new RiverAffectedCellData(cell, distance));
                }
            }

            foreach (TerrainCell neighbour in grid.GetNeighboursCardinal(cell.Coordinates))
            {
                CollectAffectedCells(riverAffectedCells, neighbour, depth - 1);
            }
        }

        List<RiverAffectedCellData> riverAffectedCells = new List<RiverAffectedCellData>();
        foreach (TerrainCell cell in riverPath)
        {
            CollectAffectedCells(riverAffectedCells, cell, maxDepth);
        }
        return riverAffectedCells;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="noiseChange"></param>
    /// <param name="radius"></param>
    /// <param name="influenceChange">Parameters: max depth, current depth, noise value</param>
    private List<TerrainCell> ChangeNoiseValueWithSpread(TerrainCell cell, float noiseChange, int radius, Func<int, int, float, float> influenceChange)
    {
        void ChangeNoiseValue(List<TerrainCell> affectedCells, TerrainCell cell, float newNoiseValue, int depth)
        {
            cell.NoiseValue += influenceChange(radius, depth, newNoiseValue);
            if (depth == 0) return;
            
            affectedCells.Add(cell);

            foreach (TerrainCell neighbour in grid.GetNeighboursCardinal(cell.Coordinates))
            {
                if (affectedCells.Contains(neighbour))
                    continue;

                ChangeNoiseValue(affectedCells, neighbour, newNoiseValue, depth - 1);
            }

            
        }

        List<TerrainCell> affectedCells = new List<TerrainCell>();
        ChangeNoiseValue(affectedCells, cell, noiseChange, radius);

        return affectedCells;
    }

    #endregion

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


    #region Editor
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
    #endregion
}
