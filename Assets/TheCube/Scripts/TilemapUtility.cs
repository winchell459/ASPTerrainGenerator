using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapUtility 
{
    public static TileBase GetTile(Tilemap tilemap, Vector2 pos)
    {
        Vector3Int tilePos = tilemap.WorldToCell(pos);
        return tilemap.GetTile(tilePos);
    }

    public static void DestroyTile(Tilemap tilemap, Vector2 pos)
    {
        Vector3Int tilePos = tilemap.WorldToCell(pos);
        tilemap.SetTile(tilePos, null);
    }

    public static bool DestroyTile(Tilemap tilemap, Vector2 pos, string tileType)
    {
        TileBase tileBase = GetTile(tilemap, pos);
        if (tileBase && tileBase.name == tileType)
        {
            DestroyTile(tilemap, pos);
            return true;
        }

        else return false;
    }

    //public static bool DestroyTile(Tilemap tilemap, Vector2 pos, string tileType, bool hasKey)
    //{
    //    TileBase tileBase = GetTile(tilemap, pos);
    //    if (tileBase && tileBase.name == tileType && hasKey)
    //    {
    //        DestroyTile(tilemap, pos);
    //        return true;
    //    }

    //    else return false;
    //}

}
