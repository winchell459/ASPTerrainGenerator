using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseTerrain
{
    public class PlatformChunk
    {
        public int platformID;
        public List<Vector2Int> groundTiles = new List<Vector2Int>();
        public int[,] path;
        public List<int> connectedPlatforms;
        public Vector2Int GetTilePos(Vector2Int tile, RoomChunk roomChunk)
        {
            return new Vector2Int(tile.x + roomChunk.minTile.x, -tile.y + 1 - roomChunk.maxTile.y);
        }
        public void SetPath(int platformID, RoomChunk roomChunk, int jumpHeight)
        {
            this.platformID = platformID;
            roomChunk.PrintPath(new Vector2Int(groundTiles[0].x + roomChunk.minTile.x, -groundTiles[0].y + 1 - roomChunk.maxTile.y), jumpHeight, platformID);
            path = roomChunk.GetPath(new Vector2Int(groundTiles[0].x + roomChunk.minTile.x, -groundTiles[0].y + 1 - roomChunk.maxTile.y), jumpHeight, platformID);
            SetPlatformEdges(platformID, roomChunk);
        }
        public void SetPlatformEdges(int platformID, RoomChunk roomChunk)
        {
            connectedPlatforms = new List<int>();
            for (int x = 0; x < path.GetLength(0); x += 1)
            {
                for (int y = 0; y < path.GetLength(1); y += 1)
                {
                    if (path[x, y] == 0 && y + 1 < roomChunk.height && roomChunk.GetTile(x, y + 1) && roomChunk.GetPlatformID(x, y + 1) != platformID)
                    {
                        int landingID = roomChunk.GetPlatformID(x, y + 1);
                        if (!connectedPlatforms.Contains(landingID)) connectedPlatforms.Add(landingID);
                    }
                }
            }
        }
    }
}