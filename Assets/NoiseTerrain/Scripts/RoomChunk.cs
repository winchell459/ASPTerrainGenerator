using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseTerrain
{
    public class RoomChunk
    {
        public int width, height;
        public Vector2Int minTile, maxTile;
        Chunk[,] chunks;

        public List<FilledChunk> filledChunks = new List<FilledChunk>();
        public int[,] filledChunkIDs;

        public RoomChunk(List<Chunk> roomChunks)
        {
            int minYID = int.MaxValue;
            int minXID = int.MaxValue;
            int maxYID = int.MinValue;
            int maxXID = int.MinValue;

            foreach (Chunk chunk in roomChunks)
            {
                minXID = Mathf.Min(chunk.chunkID.x, minXID);
                minYID = Mathf.Min(chunk.chunkID.y, minYID);
                maxXID = Mathf.Max(chunk.chunkID.x, maxXID);
                maxYID = Mathf.Max(chunk.chunkID.y, maxYID);
            }

            Vector2Int roomChunkSize = new Vector2Int(maxXID - minXID + 1, maxYID - minYID + 1);
            chunks = new Chunk[roomChunkSize.x, roomChunkSize.y];
            width = roomChunkSize.x * roomChunks[0].width;
            height = roomChunkSize.y * roomChunks[0].height;

            foreach (Chunk chunk in roomChunks)
            {
                int x = chunk.chunkID.x - minXID;
                int y = chunk.chunkID.y - minYID;
                chunks[x, y] = chunk;
            }

            //calc the min/max tiles maxY and min Y are flipped since positive y is down
            minTile = new Vector2Int(minXID * roomChunks[0].width, maxYID * roomChunks[0].height + roomChunks[0].height - 1);
            maxTile = new Vector2Int(maxXID * roomChunks[0].width + roomChunks[0].width - 1, minYID * roomChunks[0].height);

            PrintBoolMap();
            PrintFilledChunkIDs();
        }
        public void PrintBoolMap()
        {
            bool[,] boolMap = GetBoolMap();
            string map0_1 = "";
            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    map0_1 += boolMap[x, y] ? "1" : "0";
                }
                map0_1 += "\n";
            }
            Debug.Log(map0_1);
        }
        public bool[,] GetBoolMap()
        {
            bool[,] boolMap = new bool[width, height];
            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    boolMap[x, y] = GetTile(x, y);
                }
            }
            return boolMap;
        }

        public bool GetTile(int x, int y)
        {
            int width = chunks[0, 0].width;
            int height = chunks[0, 0].height;
            int xID = x / width;
            int yID = y / height;
            x = x % width;
            y = y % height;

            return chunks[xID, yID].GetTile(x, y);
        }

        public void SetFilledChunkIDs()
        {
            filledChunkIDs = new int[width, height];
            List<Vector2Int> toVisit = new List<Vector2Int>();
            List<Vector2Int> visited = new List<Vector2Int>();
            for (int x = 0; x < width; x += 1)
            {
                for (int y = 0; y < height; y += 1)
                {
                    if (GetTile(x, y)) toVisit.Add(new Vector2Int(x, y));
                }
            }
            int filledChunkID = 0;
            while (toVisit.Count > 0)
            {
                filledChunkID += 1;
                List<Vector2Int> frontier = new List<Vector2Int>();
                frontier.Add(toVisit[0]);
                //toVisit.RemoveAt(0);
                while (frontier.Count > 0)
                {
                    Vector2Int loc = frontier[0];
                    frontier.RemoveAt(0);
                    filledChunkIDs[loc.x, loc.y] = filledChunkID;
                    visited.Add(loc);
                    if (!toVisit.Remove(loc)) Debug.Log(loc + " removed");
                    int x = loc.x, y = loc.y;
                    if (x > 0 && GetTile(x - 1, y) && !visited.Contains(new Vector2Int(x - 1, y)) && !frontier.Contains(new Vector2Int(x - 1, y))) frontier.Add(new Vector2Int(x - 1, y));
                    if (y > 0 && GetTile(x, y - 1) && !visited.Contains(new Vector2Int(x, y - 1)) && !frontier.Contains(new Vector2Int(x, y - 1))) frontier.Add(new Vector2Int(x, y - 1));
                    if (x < width - 1 && GetTile(x + 1, y) && !visited.Contains(new Vector2Int(x + 1, y)) && !frontier.Contains(new Vector2Int(x + 1, y))) frontier.Add(new Vector2Int(x + 1, y));
                    if (y < height - 1 && GetTile(x, y + 1) && !visited.Contains(new Vector2Int(x, y + 1)) && !frontier.Contains(new Vector2Int(x, y + 1))) frontier.Add(new Vector2Int(x, y + 1));
                }
            }
        }
        public void PrintFilledChunkIDs()
        {
            SetFilledChunkIDs();
            string idMap = "";
            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    if (filledChunkIDs[x, y] > 9)
                    {
                        idMap += (char)((int)'A' + filledChunkIDs[x, y] - 10);
                    }
                    else
                        idMap += filledChunkIDs[x, y];
                }
                idMap += "\n";
            }
            Debug.Log(idMap);
        }

        public int GetPlatformID(Vector2Int tile)
        {
            if (tile.x - minTile.x < 0 || tile.x - minTile.x > width-1 || -tile.y - maxTile.y < 0 || -tile.y - maxTile.y > height-1)
                return -1;
            else
                return filledChunkIDs[tile.x - minTile.x, -tile.y - maxTile.y];
        }

        public void PrintPath(Vector2Int start, int jumpHeight)
        {
            int[,] path = GetPath(start, jumpHeight);
            string pathMap = "";
            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    if (GetTile(x, y)) pathMap += "X";
                    else pathMap += path[x, y] + 1;
                }
                pathMap += "\n";
            }
            Debug.Log(pathMap);
        }

        public int[,] GetPath(Vector2Int start, int jumpHeight)
        {
            int exitLoop = 10000;
            int[,] path = new int[width, height];
            for(int i = 0; i < width; i += 1)
            {
                for(int j = 0; j < height; j += 1)
                {
                    path[i, j] = -1;
                }
            }
            List<Vector2Int> frontier = new List<Vector2Int>();
            frontier.Add(new Vector2Int(start.x - minTile.x, -start.y - maxTile.y));
            //path[frontier[0].x, frontier[0].y] = jumpHeight;
            while (frontier.Count > 0)
            {
                exitLoop -= 1;
                if(exitLoop < 0)
                {
                    Debug.LogWarning("exitLoop break");
                    break;
                }
                Vector2Int current = frontier[0];
                frontier.RemoveAt(0);
                int x = current.x;
                int y = current.y;
                Debug.Log($"visiting {current}");
                path[x, y] = Mathf.Max(0, path[x, y]);

                //jumping
                if (y < height - 1 && y > 0 && GetTile(x, y + 1) && !GetTile(x, y - 1))
                {
                    AddMaxToPath(frontier, path, jumpHeight - 1, x, y - 1);
                }
                else if (path[x, y] > 0 && y > 0 && !GetTile(x, y - 1))
                {
                    AddMaxToPath(frontier, path, path[x, y] - 1, x, y - 1);

                    if (x > 0 && !GetTile(x-1, y - 1))
                    {
                        AddMaxToPath(frontier, path, path[x, y] - 1, x - 1, y - 1);
                    }
                    if (x < width - 1 && !GetTile(x + 1, y - 1))
                    {
                        AddMaxToPath(frontier, path, path[x, y] - 1, x + 1, y - 1);
                    }
                }

                //falling
                if ( y < height - 1 && path[x,y] >= 0 && !GetTile(x, y + 1))
                {
                    AddMaxToPath(frontier, path, 0, x, y + 1);

                    if (x > 0 && !GetTile(x - 1, y + 1))
                    {
                        AddMaxToPath(frontier, path, 0, x - 1, y + 1);
                    }
                    if (x < width - 1 && !GetTile(x + 1, y + 1))
                    {
                        AddMaxToPath(frontier, path, 0, x + 1, y+1);
                    }
                }

                //walking
                if(y < height - 1 && GetTile(x, y + 1))
                {
                    if(x > 0 && !GetTile(x - 1, y))
                    {
                        AddMaxToPath(frontier, path, 0, x - 1, y);
                    }
                    if (x < width -1 && !GetTile(x + 1, y))
                    {
                        AddMaxToPath(frontier, path, 0, x + 1, y);
                    }
                }
            }
            return path;
        }
        private void AddMaxToPath(List<Vector2Int> frontier, int[,] path, int newVal, int x, int y)
        {
            int val = Mathf.Max(path[x, y], newVal);
            if(val != path[x, y])
            {
                path[x, y] = val;
                AddToSet(frontier, new Vector2Int(x, y));
            }
        }
        private void AddToSet(List<Vector2Int> list, Vector2Int value)
        {
            if (!list.Contains(value)) list.Add(value);
        }
    }

    public class FilledChunk
    {
        public List<Vector2Int> filledTiles;
        public FilledChunk(List<Vector2Int> filledTiles)
        {
            this.filledTiles = filledTiles;
        }
    }

    public class PlatformChunk
    {

    }
}