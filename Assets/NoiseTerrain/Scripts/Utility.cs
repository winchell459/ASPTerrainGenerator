using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseTerrain
{
    public static class Utility 
    {
        public static void CheckTileRules(Chunk chunk, TileRules tileRules)
        {

            for (int x = 0; x < chunk.width; x += 1)
            {
                for (int y = 0; y < chunk.height; y += 1)
                {
                    bool[] neighbors = chunk.GetTileNeighbors(x, y);
                    if (neighbors != null)
                    {
                        bool valid = tileRules.GetValidTile(neighbors);
                        chunk.SetInvalidTile(x, y, !valid);

                        if (!valid) chunk.hasInvalidTile = true;
                    }

                }
            }
            //chunk.SetInvalidTile();
        }
        public static bool CheckTileRules(SubChunk subChunk, TileRules tileRules)
        {
            for (int x = 1; x < subChunk.tiles.GetLength(0) - 1; x += 1)
            {
                for (int y = 1; y < subChunk.tiles.GetLength(1) - 1; y += 1)
                {
                    bool[] neighbors = subChunk.GetTileNeighbors(x, y);
                    if (neighbors != null && !tileRules.GetValidTile(neighbors)) return false;

                }
            }
            return true;
        }

        public static bool CheckTileRules(List<bool> tiles, int width, TileRules tileRules)
        {
            int height = tiles.Count / width;
            for (int x = 1; x < width - 1; x += 1)
            {
                for (int y = 1; y < height - 1; y += 1)
                {
                    int index = x + y * width;
                    bool[] neighbors = SubChunk.GetTileNeighbors(index, width, tiles);
                    if (neighbors != null && !tileRules.GetValidTile(neighbors)) return false;

                }
            }
            return true;
        }

        public static void SortToChunkIDs(Vector2Int chunkID, List<Vector2Int> toFixChunkIDs)
        {
            for(int i = 0; i < toFixChunkIDs.Count; i += 1)
            {
                for(int j = 0; j < toFixChunkIDs.Count - 1; j += 1)
                {
                    if(Vector2Int.Distance(chunkID, toFixChunkIDs[j]) > Vector2Int.Distance(chunkID, toFixChunkIDs[j + 1])){
                        Vector2Int temp = toFixChunkIDs[j];
                        toFixChunkIDs[j] = toFixChunkIDs[j + 1];
                        toFixChunkIDs[j + 1] = temp;
                    }
                }
            }

        }
    }
}

