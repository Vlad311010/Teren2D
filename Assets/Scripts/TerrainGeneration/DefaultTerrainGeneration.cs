using Enums;
using Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DefaultTerrainGeneration : TerrainGenerationBase
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
    [SerializeField] int riverMinimalWidth;
    [SerializeField] int riverAffectionRange;
    [SerializeField] int riverMidpoints;
    [SerializeField] int riverWaving;

    [Header("Road settings")]
    [SerializeField] Tilemap roadLayer;
    [SerializeField] TileBase regularRoadTile;
    [SerializeField] TileBase waterRoadTile;
    [SerializeField] WorldSides enableRoadTo;

    [Header("Props settings")]
    [SerializeField] Tilemap propsLayer;
    [SerializeField] TileBase groundPropsTile;
    [SerializeField] TileBase waterPropsTile;
    [Range(0, 1)][SerializeField] float propsDensity = 0.5f;



    [Header("Tilemaps")]
    [SerializeField] Tilemap[] layers;

    [Header("Tiles")]
    [SerializeField] TileBase defaultGroundTile;
    [SerializeField] NoiseClampData[] tiles;

    private Vector3 origin;
    private GridSystem<TerrainCell> grid;
    private float[,] noiseMap;
    private System.Random rng;

    private void Awake()
    {
        origin = transform.position;
        Generate(seed);
    }

    #region GeneratorFunctions

    
    [ContextMenu("Generate")]
    public void CallGenerate()
    {
        Generate(seed);
    }


    protected override void Clean()
    {
        foreach (Tilemap tilemapLayer in layers)
        {
            tilemapLayer.ClearAllTiles();
        }
    }

    protected override void Init(int seed)
    {
        rng = new System.Random(seed);
        UnityEngine.Random.InitState(seed);
    }

    protected override void NoiseGeneration()
    {
        noiseMap = PerlinNoise.GetNoiseMap(rng, size.x, size.y, offset, scale, octaves, persistance, lacunarity);
    }

    protected override void LayoutGeneration()
    {
        grid = new GridSystem<TerrainCell>(size.x, size.y, cellSize, origin, CreateTerrainCell);
    }


    protected override void LayoutPostprocessing()
    {
        if (riverMinimalWidth == 0) return;

        float rngValue = UnityEngine.Random.value;
        Vector2Int riverAxis = rngValue < 0.5f ? new Vector2Int(1, 0) : new Vector2Int(0, 1); // determine if river horizontal or vertical

        Vector2Int riverStart;
        Vector2Int riverEnd;
        List<Vector2Int> midpoints;

        if (rngValue < 0.5f)
        {
            riverStart = new Vector2Int(UnityEngine.Random.Range(0, size.x), 0);
            riverEnd = new Vector2Int
                (
                    Math.Clamp(UnityEngine.Random.Range(riverStart.x - riverWaving, riverStart.x + riverWaving), 0, size.x - 1),
                    size.y - 1
                );
            midpoints = new List<Vector2Int>();

            Vector2Int riverCursor = riverStart;
            int riverRemainingLegth = size.y - 1;
            for (int i = 0; i < riverMidpoints; i++)
            {
                int evenStepSize = riverRemainingLegth / riverMidpoints;
                int waving = UnityEngine.Random.Range(-riverWaving, riverWaving);
                int step = UnityEngine.Random.Range(0, evenStepSize * 2);
                riverRemainingLegth -= step;

                riverCursor.x = Math.Clamp(riverCursor.x + waving, 0, size.x - 1);
                riverCursor.y = Math.Clamp(riverCursor.y + evenStepSize, 0, size.y - 1);
                midpoints.Add(riverCursor);
            }
        }
        else
        {
            riverStart = new Vector2Int(0, UnityEngine.Random.Range(0, size.y));
            riverEnd = new Vector2Int
                (
                    size.x - 1,
                    Math.Clamp(UnityEngine.Random.Range(riverStart.y - riverWaving, riverStart.y + riverWaving), 0, size.y - 1)
                );
            midpoints = new List<Vector2Int>();

            Vector2Int riverCursor = riverStart;
            int riverRemainingLegth = size.x - 1;
            for (int i = 0; i < riverMidpoints; i++)
            {
                int evenStepSize = riverRemainingLegth / riverMidpoints;
                int waving = UnityEngine.Random.Range(-riverWaving, riverWaving);
                int step = UnityEngine.Random.Range(0, evenStepSize * 2);
                riverRemainingLegth -= step;

                riverCursor.y = Math.Clamp(riverCursor.y + waving, 0, size.y - 1);
                riverCursor.x = Math.Clamp(riverCursor.x + evenStepSize, 0, size.x - 1);
                midpoints.Add(riverCursor);
            }
        }

        List<TerrainCell> completeRiverPath = new List<TerrainCell>();
        List<TerrainCell> partialPath;

        Pathfinding.FindPath(grid, grid.GetCell(riverStart), grid.GetCell(midpoints[0]), out partialPath);
        completeRiverPath.AddRange(partialPath.SkipLast(1));
        for (int i = 1; i < riverMidpoints; i++)
        {
            Pathfinding.FindPath(grid, grid.GetCell(midpoints[i-1]), grid.GetCell(midpoints[i]), out partialPath);
            completeRiverPath.AddRange(partialPath.SkipLast(1));
        }
        Pathfinding.FindPath(grid, grid.GetCell(midpoints[riverMidpoints - 1]), grid.GetCell(riverEnd), out partialPath);
        
        completeRiverPath.AddRange(partialPath);

        FormRiver(completeRiverPath, riverAffectionRange);
    }

    protected override void FillTilemaps()
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

    protected override void PathsGeneration()
    {
        Vector2Int gridCenter = size / 2;
        int agentLifetime = Math.Max(size.x, size.y);
        RoadBuilderAgent randomWalkAgent = new RoadBuilderAgent(grid, agentLifetime, 0.25f, 0.8f);
        List<Vector2Int> directions = enableRoadTo.WorldSidesToDirections();
        foreach (Vector2Int dir in directions)
        {
            List<Vector2Int> path = randomWalkAgent.Execute(gridCenter, dir);

            foreach (Vector2Int cell in path)
            {
                if (grid.GetCell(cell.x, cell.y).NoiseValue >= 0)
                    TilemapUtils.SetTile(roadLayer, grid.GetWorldPosition(cell), regularRoadTile);
                else
                    TilemapUtils.SetTile(roadLayer, grid.GetWorldPosition(cell), waterRoadTile);
            }
        }
    }

    protected override void PropsPlacing()
    {
        int agentLifetime = Mathf.CeilToInt(Math.Min(size.x, size.y) / (2 - propsDensity));
        Vector2Int objecPlacingInterval = new Vector2Int(2, Mathf.CeilToInt( (Math.Min(size.x, size.y) / 10f) * (1 - propsDensity) ));
        PropsPlacingAgent propsPlacingAgent = new PropsPlacingAgent(size, agentLifetime, 0.3f, objecPlacingInterval);

        Vector2Int startPosition;
        Vector2Int lookDirection;
        int loops = (int)(Mathf.Sqrt(size.x * size.y) * propsDensity);
        for (int i = 0; i < loops; i++)
        {
            startPosition = grid.RandomPoint();
            lookDirection = (UnityEngine.Random.insideUnitCircle + Vector2.up * 0.01f).normalized.ToVector2Int();
            
            propsPlacingAgent.Execute(startPosition, lookDirection);
            foreach (Vector2Int cell in propsPlacingAgent.CellsToPlaceProps)
            {
                if (TilemapUtils.GetTile<TileBase>(propsLayer, new Vector3Int(cell.x, cell.y)) != null)
                    continue;


                if (grid.GetCell(cell).NoiseValue > 0)
                    TilemapUtils.SetTile(propsLayer, grid.GetWorldPosition(cell), groundPropsTile);
                else
                    TilemapUtils.SetTile(propsLayer, grid.GetWorldPosition(cell), waterPropsTile);
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
        List<TerrainCell> riverAffectedCells = CollectRiverAffectedCells(riverPath, riverWidth);
        foreach (TerrainCell cell in riverAffectedCells)
        {
            Vector2Int nearestRiverCellByX = riverPath.Where(c => c.Coordinates.x == cell.Coordinates.x).Select(c => c.Coordinates).FirstOrDefault();
            Vector2Int nearestRiverCellByY = riverPath.Where(c => c.Coordinates.y == cell.Coordinates.y).Select(c => c.Coordinates).FirstOrDefault();

            float distanceFromMainPath;
            if (nearestRiverCellByX == null || nearestRiverCellByY == null)
                distanceFromMainPath = 0;
            else
                distanceFromMainPath = Math.Min(Vector2Int.Distance(nearestRiverCellByX, cell.Coordinates), Vector2Int.Distance(nearestRiverCellByY, cell.Coordinates));

            if (distanceFromMainPath < riverMinimalWidth)
                cell.NoiseValue = -1;
            else
            {
                cell.NoiseValue = UnityEngine.Random.value >  0.8f ? -1 : UnityEngine.Random.Range(0.65f, 1f);
            }
            
        }
    }

    private List<TerrainCell> CollectRiverAffectedCells(List<TerrainCell> riverPath, int maxDepth)
    {
        void CollectAffectedCells(List<TerrainCell> riverAffectedCells, TerrainCell cell, int depth)
        {
            if (depth == 0) return;

            int distance = maxDepth - depth;

            TerrainCell collectedRiverCellData = riverAffectedCells.FirstOrDefault(c => c.Equals(cell));

            if (!riverAffectedCells.Contains(cell))
                riverAffectedCells.Add(cell);

            foreach (TerrainCell neighbour in grid.GetNeighboursCardinal(cell.Coordinates))
            {
                if (riverAffectedCells.Contains(neighbour))
                    continue;

                CollectAffectedCells(riverAffectedCells, neighbour, depth - 1);
            }
        }

        List<TerrainCell> riverAffectedCells = new List<TerrainCell>();
        foreach (TerrainCell cell in riverPath)
        {
            CollectAffectedCells(riverAffectedCells, cell, maxDepth);
        }
        return riverAffectedCells;
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
        size.x = size.x <= 0 ? 1 : size.x;
        size.y = size.y <= 0 ? 1 : size.y;
            
        riverAffectionRange = Math.Clamp(riverAffectionRange, riverMinimalWidth, int.MaxValue);

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
    }
    #endregion
}
