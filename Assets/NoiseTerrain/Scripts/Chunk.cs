using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoiseTerrain
{
    public class Chunk
    {
        private ProceduralMapGenerator mapGenerator;

        public Vector2Int chunkID;
        private bool[,] boolMap;
        private bool[,] invalidTiles;
        public bool hasInvalidTile;

        public Chunk[] neighborChunks = new Chunk[8]; //0:upleft, 1:up, 2:upright, 3:left, 4:right, 5:downleft, 6:down, 7:downright

        public int width, height;

        public bool valueChanged = true;

        public Chunk(Vector2Int chunkID, bool[,]boolMap, ProceduralMapGenerator mapGenerator)
        {
            this.chunkID = chunkID;
            this.boolMap = boolMap;
            this.mapGenerator = mapGenerator;

            width = boolMap.GetLength(0);
            height = boolMap.GetLength(1);
        }

        public bool initialized;
        
        public void BuildChunk(int seed)
        {
            int minX = chunkID.x * width;
            int minY = chunkID.y * height;
            int maxX = minX + width;
            int maxY = minY + height;

            
            
            initialized = true;
        }

        public void ClearChunk()
        {
            
        }

        public bool GetTile(int x, int y)
        {
            Chunk chunk = GetChunk(x, y);
            if (x < 0) x = width + x;
            else if (x >= width) x -= width;
            if (y < 0) y = height + y;
            else if (y >= height) y -= height;

            if (chunk != this) return chunk.GetTile(x, y);
            return boolMap[x, y];
        }
        public void SetTiles(bool[,] boolMap)
        {
            this.boolMap = boolMap;
            valueChanged = true;
        }
        public void SetTile(int x, int y, bool value)
        {
            Chunk chunk = GetChunk(x, y);
            if (x < 0) x = width + x;
            else if (x >= width) x -= width;
            if (y < 0) y = height + y;
            else if (y >= height) y -= height;

            if (chunk != this) chunk.SetTile(x, y, value);
            else
            {
                boolMap[x, y] = value;
                valueChanged = true;
            }
        }

        public bool GetInvalidTile(int x, int y)
        {
            Chunk chunk = GetChunk(x, y);
            if (x < 0) x = width + x;
            else if (x >= width) x -= width;
            if (y < 0) y = height + y;
            else if (y >= height) y -= height;

            if (chunk != this) return chunk.GetInvalidTile(x, y);
            //Debug.Log(chunkID);
            if(hasInvalidTile) return invalidTiles[x, y];
            //Debug.Log("not invalid");
            return false;
        }

        public void SetValidTile(int x, int y, bool value)
        {
            Chunk chunk = GetChunk(x, y);
            if (x < 0) x = width + x;
            else if (x >= width) x -= width;
            if (y < 0) y = height + y;
            else if (y >= height) y -= height;

            if (chunk != this) chunk.SetValidTile(x, y, value);
            else invalidTiles[x, y] = !value;
        }

        public bool[] GetTileNeighbors(int x, int y)
        {
            if (boolMap[x,y])
            {
                
                bool[] neighbors = new bool[8];

                neighbors[0] = GetTile(x - 1, y - 1);
                neighbors[1] = GetTile(x, y - 1);
                neighbors[2] = GetTile(x + 1, y - 1);
                neighbors[3] = GetTile(x - 1, y);
                neighbors[4] = GetTile(x + 1, y);
                neighbors[5] = GetTile(x - 1, y + 1);
                neighbors[6] = GetTile(x, y + 1);
                neighbors[7] = GetTile(x + 1, y + 1);

                //string display = $"{neighbors[0]} {neighbors[1]} {neighbors[2]}\n{neighbors[3]} {boolMap[x, y]} {neighbors[4]}\n{neighbors[5]} {neighbors[6]} {neighbors[7]}";
                //Debug.Log(display);

                return neighbors;
            }
            else
            {
                return null;
            }
        }

        private Chunk GetChunk(int x, int y)
        {
            if (x < 0 && y < 0) return GetNeighborChunk(0);
            else if (x >= 0 && x < width && y < 0) return GetNeighborChunk(1);
            else if (x >= width && y < 0) return GetNeighborChunk(2);
            else if (x < 0 && y >= 0 && y < height) return GetNeighborChunk(3);
            else if (x >= width && y >= 0 && y < height) return GetNeighborChunk(4);
            else if (x < 0 && y >= height) return GetNeighborChunk(5);
            else if (x >= 0 && x < width && y >= height) return GetNeighborChunk(6);
            else if (x >= width && y >= height) return GetNeighborChunk(7);
            else return this;
        }

        private Chunk GetNeighborChunk(int index)
        {
            if (neighborChunks[index] != null) return neighborChunks[index];
            else
            {
                switch (index)
                {
                    case 0:
                        neighborChunks[0] = mapGenerator.GetChunk(chunkID + new Vector2Int(-1, -1));
                        return neighborChunks[0];
                    case 1:
                        neighborChunks[1] = mapGenerator.GetChunk(chunkID + new Vector2Int(0, -1));
                        return neighborChunks[1];
                    case 2:
                        neighborChunks[2] = mapGenerator.GetChunk(chunkID + new Vector2Int(1, -1));
                        return neighborChunks[2];
                    case 3:
                        neighborChunks[3] = mapGenerator.GetChunk(chunkID + new Vector2Int(-1, 0));
                        return neighborChunks[3];
                    case 4:
                        neighborChunks[4] = mapGenerator.GetChunk(chunkID + new Vector2Int(1, 0));
                        return neighborChunks[4];
                    case 5:
                        neighborChunks[5] = mapGenerator.GetChunk(chunkID + new Vector2Int(-1, 1));
                        return neighborChunks[5];
                    case 6:
                        neighborChunks[6] = mapGenerator.GetChunk(chunkID + new Vector2Int(0, 1));
                        return neighborChunks[6];
                    case 7:
                        neighborChunks[7] = mapGenerator.GetChunk(chunkID + new Vector2Int(1, 1));
                        return neighborChunks[7];
                    default:
                        return null;
                }
            }
        }
        public void SetInvalidTile()
        {
            bool invalid = false;
            for(int x = 0; x < width; x+= 1)
            {
                for(int y = 0; y < height; y += 1)
                {
                    if (invalidTiles[x, y]) invalid = true;
                }
            }
            hasInvalidTile = invalid;
        }

        public void SetInvalidTile(int x, int y, bool invalid)
        {
            if (invalidTiles == null) invalidTiles = new bool[width, height];
            invalidTiles[x, y] = invalid;
        }
        public void SetInvalidTiles(bool[,] invalidTiles)
        {
            this.invalidTiles = invalidTiles;
        }

        public List<SubChunk> GetInvalidSubChunks(int borderWidth)
        {
            List<SubChunk> subChunks = new List<SubChunk>();
            List<Vector2Int> foundInvalidTiles = new List<Vector2Int>();
            for(int x = -borderWidth; x < width + borderWidth; x += 1)
            {
                for(int y = -borderWidth; y < height + borderWidth; y += 1)
                {
                    if (GetInvalidTile(x, y))
                    {
                        Vector2Int invalidTileStart = new Vector2Int(x, y);
                        if (!foundInvalidTiles.Contains(invalidTileStart))
                        {
                            SubChunk subChunk = GetInvalidSubChunk(borderWidth, invalidTileStart);
                            //Debug.Log($"subChunk invalidCount {subChunk.invalidTiles.Count}");
                            foreach(Vector2Int invalidTile in subChunk.invalidTiles)
                            {
                                if (!foundInvalidTiles.Contains(invalidTile))
                                {
                                    foundInvalidTiles.Add(invalidTile);
                                }
                                else
                                {
                                    Debug.LogWarning("Found Invalid Tile duplicate error");
                                }
                            }
                            subChunks.Add(subChunk);
                        }
                    }
                    
                }
            }
            return subChunks;
        }

        SubChunk GetInvalidSubChunk(int borderWidth, Vector2Int invalidTileStart)
        {
            List<Vector2Int> invalidTiles = new List<Vector2Int>();
            invalidTiles.Add(invalidTileStart);
            GetInvalidTiles(borderWidth, invalidTileStart, invalidTiles,chunkID);
            int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
            foreach(Vector2Int invalidPos in invalidTiles)
            {
                if (invalidPos.x > maxX) maxX = invalidPos.x;
                if (invalidPos.y > maxY) maxY = invalidPos.y;
                if (invalidPos.x < minX) minX = invalidPos.x;
                if (invalidPos.y < minY) minY = invalidPos.y;

            }
            minX -= borderWidth;
            minY -= borderWidth;
            maxX += borderWidth;
            maxY += borderWidth;

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            //Debug.Log($"{width} x {height}");
            bool[,] tiles = new bool[width, height];
            for(int x = minX; x <= maxX; x+=1)
            {
                for(int y = minY; y <= maxY; y += 1)
                {
                    tiles[x - minX, y - minY] = GetTile(x, y);
                }
            }
            return new SubChunk(minX, minY, tiles, invalidTiles);
        }
        void GetInvalidTiles(int borderWidth, Vector2Int invalidTileStart, List<Vector2Int> invalidTiles, Vector2Int refChunkID)
        {
            for (int x = -borderWidth; x <= borderWidth; x += 1)
            {
                for (int y = -borderWidth; y <= borderWidth; y += 1)
                {
                    //Debug.Log($"{chunkID} (tileStart = {invalidTileStart} ) GetInvalidTile({x + invalidTileStart.x},{y + invalidTileStart.y}) = {GetInvalidTile(x + invalidTileStart.x, y + invalidTileStart.y)}");
                    if ((x != 0 || y != 0) && GetInvalidTile(x + invalidTileStart.x, y + invalidTileStart.y))
                    {
                        Vector2Int offset = refChunkID - chunkID;
                        Vector2Int invalidTilePos = new Vector2Int(x + invalidTileStart.x + width * offset.x, y + invalidTileStart.y + height * offset.y);
                        //Debug.Log($"added invalidTilePos {invalidTilePos}");
                        if (!invalidTiles.Contains(invalidTilePos))
                        {
                            invalidTiles.Add(invalidTilePos);
                            GetInvalidTiles(borderWidth, invalidTilePos, invalidTiles, refChunkID);
                        }

                    }
                }
            }
        }
        

    }

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

                neighbors[0] = tiles[x - 1 + width*(y - 1)];
                neighbors[1] = tiles[x + width*(y - 1)];
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

