using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseTerrain
{
    public class FilledChunk
    {
        public List<Vector2Int> filledTiles = new List<Vector2Int>();
        public List<Vector2Int> groundTiles = new List<Vector2Int>();
        public List<PlatformChunk> platforms = new List<PlatformChunk>();
        int[,] platformIDs;
        int minX, minY, maxX, maxY;
        //public FilledChunk()
        //{
        //    filledTiles = new List<Vector2Int>();
        //    groundTiles = new List<Vector2Int>();
        //}
        //public FilledChunk(List<Vector2Int> filledTiles)
        //{
        //    this.filledTiles = filledTiles;
        //}
        public int GetPlatformID(Vector2Int tile)
        {
            //Debug.Log($"{tile.x - minX} {-tile.y - maxY} minX: {minX} maxY: {maxY}");
            return platformIDs[tile.x - minX, tile.y - minY];
        }
        public void SetPlatforms(RoomChunk roomChunk, int jumpHeight)
        {
            minX = int.MaxValue;
            minY = int.MaxValue;
            maxX = int.MinValue;
            maxY = int.MinValue;
            foreach (Vector2Int tile in filledTiles)
            {
                if (tile.x > maxX) maxX = tile.x;
                if (tile.y > maxY) maxY = tile.y;
                if (tile.x < minX) minX = tile.x;
                if (tile.y < minY) minY = tile.y;
            }
            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            platformIDs = new int[width, height];
            foreach (Vector2Int tile in filledTiles)
            {
                platformIDs[tile.x - minX, tile.y - minY] = -1;
            }

            List<Vector2Int> toVisit = new List<Vector2Int>(groundTiles);
            List<Vector2Int> visited = new List<Vector2Int>();
            int platformID = 0;
            int breakCounter = 1000;
            while (toVisit.Count > 0)
            {
                List<Vector2Int> frontier = new List<Vector2Int>();
                platformID += 1;
                frontier.Add(toVisit[0]);
                //toVisit.RemoveAt(0);
                while (frontier.Count > 0)
                {
                    Vector2Int current = frontier[0];
                    frontier.RemoveAt(0);
                    visited.Add(current);
                    toVisit.Remove(current);

                    platformIDs[current.x - minX, current.y - minY] = platformID;

                    FindPlatformNeighbor(-1, current, jumpHeight, platformID, toVisit, visited, frontier, roomChunk);
                    FindPlatformNeighbor(1, current, jumpHeight, platformID, toVisit, visited, frontier, roomChunk);
                    breakCounter -= 1;
                    if (breakCounter < 0)
                    {
                        Debug.LogWarning("frontier.Count break");
                        break;
                    }
                }
                breakCounter -= 1;
                if (breakCounter < 0)
                {
                    Debug.LogWarning("toVisit.Count break");
                    break;
                }
            }

            for (int i = 0; i < platformID; i += 1) platforms.Add(new PlatformChunk());
            string printMap = "";
            for (int y = 0; y < platformIDs.GetLength(1); y += 1)
            {
                for (int x = 0; x < platformIDs.GetLength(0); x += 1)
                {
                    if (platformIDs[x, y] == -1)
                    {
                        printMap += "X";
                    }
                    else
                    {
                        printMap += platformIDs[x, y].ToString();
                    }
                    if (platformIDs[x, y] > 0) platforms[platformIDs[x, y] - 1].groundTiles.Add(new Vector2Int(x + minX, y + minY));
                }
                printMap += "\n";
            }
            Debug.Log(printMap);
        }
        private void FindPlatformNeighbor(int xOffset, Vector2Int current, int jumpHeight, int platformID, List<Vector2Int> toVisit, List<Vector2Int> visited, List<Vector2Int> frontier, RoomChunk roomChunk)
        {
            int y = current.y;
            int breakCounter = 1000;
            while (Mathf.Abs(y - current.y) <= jumpHeight && y - minY >= 0 && y - minY < platformIDs.GetLength(1) && current.x + xOffset - minX >= 0 && current.x + xOffset - minX < platformIDs.GetLength(0))
            {
                if (platformIDs[current.x + xOffset - minX, y - minY] != 0)
                {
                    //check up

                    if (/*platformIDs[current.x + xOffset - minX, y - 1 - minY] != 0*/ !roomChunk.GetTile(current.x + xOffset, y - 1))
                    {
                        //plaform tile found
                        platformIDs[current.x + xOffset - minX, y - minY] = platformID;
                        Vector2Int neighbor = new Vector2Int(current.x + xOffset, y);
                        if (!visited.Contains(neighbor) && !frontier.Contains(neighbor))
                        {
                            //toVisit.Remove(neighbor);
                            //visited.Add(neighbor);
                            frontier.Add(neighbor);
                        }

                        break;
                    }
                    else
                    {
                        //check up
                        y -= 1;
                    }
                }
                else
                {
                    //check down
                    y += 1;
                    if (y - minY >= platformIDs.GetLength(1) || platformIDs[current.x - minX, y - minY] == 0) break;
                }
                breakCounter -= 1;
                if (breakCounter < 0)
                {
                    Debug.LogWarning("FindPlatformNeighbor break");
                    break;
                }
            }
        }
    }
}