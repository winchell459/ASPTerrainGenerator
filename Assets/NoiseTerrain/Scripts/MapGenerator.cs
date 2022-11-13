using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace NoiseTerrain
{
    public class MapGenerator : MonoBehaviour
    {
        public TileBase fullTile;
        public Tilemap fullTilemap;

        public int width = 100, height = 100;

        public float noiseScale = 1;
        public int octaves = 1;

        [Range(0, 1)]
        public float persistance;
        public float lacunarity;
        public int seed;
        public Vector2 offset;

        public bool autoUpdate = false;

        public virtual void ClearMap()
        {
            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    fullTilemap.SetTile(new Vector3Int(x, -y, 0), null);
                }
            }
        }
        public void ClearMap(int minX, int maxX, int minY, int maxY)
        {
            for (int y = minY; y <= maxY; y += 1)
            {
                for (int x = minX; x <= maxX; x += 1)
                {
                    fullTilemap.SetTile(new Vector3Int(x, -y, 0), null);
                }
            }
        }
        
        public bool[,] GenerateBoolMap(int minX, int maxX, int minY, int maxY)
        {
            //Debug.Log("GenerateMap");
            int chunkSizeX = width;
            int chunkSizeY = height;

            Vector2 scaleOffset = new Vector2(minX / noiseScale, minY / noiseScale);

            float[,] noiseMap = Sebastian.Noise.GenerateNoiseMap(width, height, seed, noiseScale, octaves, persistance, lacunarity, scaleOffset);
            bool[,] boolMap = new bool[width, height];
            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    
                    boolMap[x, y] = noiseMap[x, y] > 0;
                }
            }

            return boolMap;
        }

        public void GenerateMap(int minX, int maxX, int minY, int maxY)
        {
            //Debug.Log("GenerateMap");
            int chunkSizeX = width;
            int chunkSizeY = height;

            Vector2 scaleOffset = new Vector2(minX / noiseScale, minY / noiseScale);
            //Vector2Int chunkID = new Vector2Int(minX / width, minY / height);

            

            float[,] noiseMap = Sebastian.Noise.GenerateNoiseMap(width, height, seed, noiseScale, octaves, persistance, lacunarity, scaleOffset);

            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    float noiseHeight = noiseMap[x, y];
                    if (noiseHeight > 0)
                    {
                        fullTilemap.SetTile(new Vector3Int(x + minX, -y - minY, 0), fullTile);
                    }
                    else
                    {
                        fullTilemap.SetTile(new Vector3Int(x + minX, -y - minY, 0), null);
                    }
                }
            }

            setupComplete = true;
        }

        public virtual void GenerateMap()
        {
            //Debug.Log("GenerateMap");
            float[,] noiseMap = Sebastian.Noise.GenerateNoiseMap(width, height, seed, noiseScale, octaves, persistance, lacunarity, offset);

            for(int y = 0; y < height; y += 1)
            {
                for(int x = 0; x < width; x += 1)
                {
                    float noiseHeight = noiseMap[x, y];
                    if(noiseHeight > 0)
                    {
                        fullTilemap.SetTile(new Vector3Int(x, -y, 0), fullTile);
                    }
                    else
                    {
                        fullTilemap.SetTile(new Vector3Int(x, -y, 0), null);
                    }
                }
            }

            setupComplete = true;
        }

        public bool setupComplete = false;
        public IEnumerator GenerateMap(int seed)
        {
            setupComplete = false;
            this.seed = seed;
            GenerateMap();
            while (!setupComplete)
            {
                yield return null;
            }
        }
    }
}

