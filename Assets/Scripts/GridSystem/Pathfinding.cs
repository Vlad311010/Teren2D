
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Structs;

public class Pathfinding
{ 
}

/*
public class Pathfinding
{
    private static List<GridCell> ReconstructPath(Dictionary<GridCell, GridCell> cameFromMap, GridCell current)
    {
        List<GridCell> path = new List<GridCell>() { current };
        while (cameFromMap.ContainsKey(current))
        {
            current = cameFromMap[current];
            path.Insert(0, current);
        }

        return path;
    }

    public static bool FindPath(GridCell start, GridCell destination, out List<GridCell> path)
    {
        if (start == null || destination == null) 
        {
            path = null;
            return false;
        }

        path = new List<GridCell>();
        List<GridCell> openSet = new List<GridCell>() { start };
        Dictionary<GridCell, GridCell> cameFrom = new Dictionary<GridCell, GridCell>();
        
        Dictionary<GridCell, float> gScore = new Dictionary<GridCell, float>() 
        {
            { start, 0 }
        };

        Dictionary<GridCell, float> fScore = new Dictionary<GridCell, float>
        {
            { start, Heuristic(start, destination) }
        };


        while (openSet.Count > 0)
        {
            openSet = openSet.OrderBy(e => fScore.GetOrInit(e, int.MaxValue)).ToList();
            GridCell current = openSet[0];

            if (current == destination)
            {
                path = ReconstructPath(cameFrom, current);
                return true;
            }
            openSet.RemoveAt(0);
            foreach (GridCell neighbor in current.adjacent) 
            {
                float tenativeScore = gScore.GetOrInit(current, int.MaxValue) + 1;
                if (tenativeScore < gScore.GetOrInit(neighbor, int.MaxValue))
                {
                    cameFrom.SetOrInit(neighbor, current);
                    gScore.SetOrInit(neighbor, tenativeScore);
                    fScore.SetOrInit(neighbor, tenativeScore + Heuristic(start, destination));
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return false;
    }

    

    private static float Heuristic(GridCell cell, GridCell destination)
    {
        return Vector3.Distance(cell.worldPosition, destination.worldPosition);
    }
}*/
