
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Structs;

public class Pathfinding
{
    private static List<T> ReconstructPath<T>(Dictionary<T, T> cameFromMap, T current) where T : GridCellBase
    {
        List<T> path = new List<T>() { current };
        while (cameFromMap.ContainsKey(current))
        {
            current = cameFromMap[current];
            path.Insert(0, current);
        }

        return path;
    }

    public static bool FindPath<T>(GridSystem<T> grid, T start, T destination, out List<T> path) where T : GridCellBase
    {
        if (start == null || destination == null) 
        {
            path = null;
            return false;
        }

        path = new List<T>();
        List<T> openSet = new List<T>() { start };
        Dictionary<T, T> cameFrom = new Dictionary<T, T>();
        
        Dictionary<T, float> gScore = new Dictionary<T, float>() 
        {
            { start, 0 }
        };

        Dictionary<T, float> fScore = new Dictionary<T, float>
        {
            { start, Heuristic(grid, start, destination) }
        };


        while (openSet.Count > 0)
        {
            openSet = openSet.OrderBy(e => fScore.GetOrInit(e, int.MaxValue)).ToList();
            T current = openSet[0];

            if (current == destination)
            {
                path = ReconstructPath(cameFrom, current);
                return true;
            }
            openSet.RemoveAt(0);
            foreach (T neighbor in grid.GetNeighboursAll(current.Coordinates))
            {
                float tenativeScore = gScore.GetOrInit(current, int.MaxValue) + 1;
                if (tenativeScore < gScore.GetOrInit(neighbor, int.MaxValue))
                {
                    cameFrom.SetOrInit(neighbor, current);
                    gScore.SetOrInit(neighbor, tenativeScore);
                    fScore.SetOrInit(neighbor, tenativeScore + Heuristic(grid, start, destination));
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return false;
    }

    
    private static float Heuristic<T>(GridSystem<T> grid, T cell, T destination) where T : GridCellBase
    {
        return Vector3.Distance(grid.GetWorldPosition(cell.Coordinates), grid.GetWorldPosition(destination.Coordinates));
    }
}
