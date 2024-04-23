
using System.Collections.Generic;
using UnityEngine;

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

    public static int RandomRange(this Vector2Int vector)
    {
        return Random.Range(vector.x, vector.y + 1);
    }

    public static Vector2Int Swap(this Vector2Int vector)
    {
        return new Vector2Int(vector.y, vector.x);
    }
}
