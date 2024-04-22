using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapUtils
{
    const float contactPointNormalOffset = 0.3f;

    public static T GetTileFromCollision<T>(Tilemap tilemap, Collision collision) where T : TileBase
    {
        Vector3Int tileCoordinates = tilemap.WorldToCell(collision.contacts[0].point - collision.contacts[0].normal * contactPointNormalOffset);
        return tilemap.GetTile<T>(tileCoordinates);
    }

    public static Vector3Int GetTileCoordinatesFromCollision(Tilemap tilemap, Collision collision)
    {
        return tilemap.WorldToCell(collision.contacts[0].point - collision.contacts[0].normal * contactPointNormalOffset);
    }

    public static T GetTile<T>(Tilemap tilemap, Vector3 coordinates) where T : TileBase
    {
        return GetTile<T>(tilemap, tilemap.WorldToCell(coordinates));
    }

    public static T GetTile<T>(Tilemap tilemap, Vector3Int coordinates) where T : TileBase
    {
        return tilemap.GetTile<T>(coordinates);
    }

    public static void SetTile(Tilemap tilemap, Vector3 coordinates, TileBase tile)
    {
        SetTile(tilemap, tilemap.WorldToCell(coordinates), tile);
    }

    public static void SetTile(Tilemap tilemap, Vector3Int coordinates, TileBase tile)
    {
        tilemap.SetTile(coordinates, tile);
    }

}
