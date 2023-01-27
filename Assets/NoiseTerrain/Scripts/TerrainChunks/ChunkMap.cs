using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainChunk
{
    public class ChunkMap : MonoBehaviour
    {
        List<Chunk> chunks = new List<Chunk>();
        public int width = 12, height = 9;

        public void AddChunk(Chunk chunk)
        {
            chunks.Add(chunk);
        }
        public Chunk GetChunk(Vector2Int chunkID)
        {
            foreach (Chunk chunk in chunks)
            {
                if (chunk.chunkID == chunkID) return chunk;
            }
            return null;
        }

        public Vector2Int GetChunkID(Vector2 pos)
        {
            int xOffset = pos.x < 0 ? -1 : 0;
            int yOffset = pos.y > 0 ? 1 : 0;

            return new Vector2Int(xOffset + ((int)pos.x - xOffset) / width, -yOffset - ((int)pos.y - yOffset) / height);
        }

        public void SetTile(Vector2Int pos, bool value)
        {
            int x = ((pos.x % width) + width) % width;
            int y = ((-pos.y % height) + height) % height;
            Debug.Log($"{x} {y}");
            GetChunk(GetChunkID(pos)).SetTile(x, y, value);
        }

        public bool GetTile(Vector2Int pos)
        {
            int x = ((pos.x % width) + width) % width;
            int y = ((-pos.y % height) + height) % height;
            Debug.Log($"{x} {y}");
            return GetChunk(GetChunkID(pos)).GetTile(x, y);
        }
    }
}

