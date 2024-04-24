
using Enums;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public static class Extensions
{
    public static float UnitInterval(this System.Random rng, int precision = 100000)
    {
        return (rng.Next(0, precision + 1)) / ((float)precision);
    }

    public static void SetOrInit<T, V>(this Dictionary<T, V> dict, T key, V value)
    {
        if (dict.ContainsKey(key))
        {
            dict[key] = value;
        }
        else
        {
            dict.Add(key, value);
        }
    }

    public static V GetOrInit<T, V>(this Dictionary<T, V> dict, T key, V defaultValue)
    {
        if (dict.ContainsKey(key))
        {
            return dict[key];
        }
        else
        {
            dict.Add(key, defaultValue);
            return defaultValue;
        }
    }

    public static float RandomRange(this Vector2 vector)
    {
        return Random.Range(vector.x, vector.y);
    }

    public static Vector2Int ToVector2Int(this Vector2 vector)
    {
        return new Vector2Int((int)vector.x, (int)vector.y);
    }

    public static int RandomRange(this Vector2Int vector)
    {
        return Random.Range(vector.x, vector.y + 1);
    }

    public static Vector2Int Swap(this Vector2Int vector)
    {
        return new Vector2Int(vector.y, vector.x);
    }

    public static Vector2Int Rotate(this Vector2Int vector, float angle)
    {
        // Convert angle to radians
        float radians = angle * Mathf.Deg2Rad;

        // Rotate vector
        int newX = Mathf.RoundToInt(vector.x * Mathf.Cos(radians) - vector.y * Mathf.Sin(radians));
        int newY = Mathf.RoundToInt(vector.x * Mathf.Sin(radians) + vector.y * Mathf.Cos(radians));

        return new Vector2Int(newX, newY);
    }

    public static List<Vector2Int> WorldSidesToDirections(this WorldSides worldSides)
    {
        List<Vector2Int> directions = new List<Vector2Int>();
        if (worldSides.HasFlag(WorldSides.North))
            directions.Add(Vector2Int.up);
        if (worldSides.HasFlag(WorldSides.East))
            directions.Add(Vector2Int.right);
        if (worldSides.HasFlag(WorldSides.South))
            directions.Add(Vector2Int.down);
        if (worldSides.HasFlag(WorldSides.West))
            directions.Add(Vector2Int.left);

        return directions;
    }
}
