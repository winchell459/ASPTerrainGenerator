using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainChunk
{
    public class SubChunk
    {
        public static int nextID = 0;
        public int subChunkID;
        public bool hasInvalid;
        public int minX;
        public int minY;
        public bool[,] tiles;
        public List<Vector2Int> invalidTiles = new List<Vector2Int>();
        public SubChunk(int minX, int minY, bool[,] tiles, List<Vector2Int> invalidTiles)
        {
            subChunkID = nextID++;
            this.minX = minX;
            this.minY = minY;
            this.tiles = tiles;
            this.invalidTiles = invalidTiles;
        }
        public List<bool> GetTilesList()
        {
            List<bool> tilesList = new List<bool>();
            for (int y = 0; y < tiles.GetLength(1); y += 1)
            {
                for (int x = 0; x < tiles.GetLength(0); x += 1)
                {
                    tilesList.Add(tiles[x, y]);
                }
            }
            return tilesList;
        }

        public bool[] GetTileNeighbors(int x, int y)
        {
            if (tiles[x, y])
            {

                bool[] neighbors = new bool[8];

                neighbors[0] = tiles[x - 1, y - 1];
                neighbors[1] = tiles[x, y - 1];
                neighbors[2] = tiles[x + 1, y - 1];
                neighbors[3] = tiles[x - 1, y];
                neighbors[4] = tiles[x + 1, y];
                neighbors[5] = tiles[x - 1, y + 1];
                neighbors[6] = tiles[x, y + 1];
                neighbors[7] = tiles[x + 1, y + 1];

                //string display = $"{neighbors[0]} {neighbors[1]} {neighbors[2]}\n{neighbors[3]} {boolMap[x, y]} {neighbors[4]}\n{neighbors[5]} {neighbors[6]} {neighbors[7]}";
                //Debug.Log(display);

                return neighbors;
            }
            else
            {
                return null;
            }
        }

        public static bool[] GetTileNeighbors(int index, int width, List<bool> tiles)
        {
            int x = index % width;
            int y = index / width;
            if (tiles[index])
            {

                bool[] neighbors = new bool[8];

                neighbors[0] = tiles[x - 1 + width * (y - 1)];
                neighbors[1] = tiles[x + width * (y - 1)];
                neighbors[2] = tiles[x + 1 + width * (y - 1)];
                neighbors[3] = tiles[x - 1 + width * (y)];
                neighbors[4] = tiles[x + 1 + width * (y)];
                neighbors[5] = tiles[x - 1 + width * (y + 1)];
                neighbors[6] = tiles[x + width * (y + 1)];
                neighbors[7] = tiles[x + 1 + width * (y + 1)];

                //string display = $"{neighbors[0]} {neighbors[1]} {neighbors[2]}\n{neighbors[3]} {boolMap[x, y]} {neighbors[4]}\n{neighbors[5]} {neighbors[6]} {neighbors[7]}";
                //Debug.Log(display);

                return neighbors;
            }
            else
            {
                return null;
            }
        }

        public void PrintTiles()
        {
            string map = "";
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);
            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    map += tiles[x, y] ? 1 : 0;
                }
                map += "\n";
            }
            Debug.Log(map);
        }
    }
}

