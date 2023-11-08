using NoiseTerrain;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Sebastian
{
    public class MapDisplay : MonoBehaviour
    {
        public Renderer textureRender;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        public Tilemap tilemap;
        public TileBase tileBase;
        public float tileThreshold = 0;
        public TileRules tileRules;

        public void DrawTexture(Texture2D texture)
        {
            textureRender.sharedMaterial.mainTexture = texture;
            textureRender.transform.localScale = new Vector3(texture.width, texture.height, 1);
        }
        
        public void DrawMesh(MeshData meshData, Texture2D texture)
        {
            meshFilter.sharedMesh = meshData.CreateMesh();
            meshRenderer.sharedMaterial.mainTexture = texture;
        }

        public void DrawTiles(float[,] noiseMap)
        {
            if(tileRules != null)
            {
                DrawTiles(noiseMap, tileRules);
            }
            else
            {
                DrawTiles(noiseMap, tileBase);
            }
        }


        private void DrawTiles(float[,] noiseMap, TileBase tileBase)
        {
            int height = noiseMap.GetLength(1);
            int width = noiseMap.GetLength(0);
            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    float noiseHeight = noiseMap[x, y];
                    if (noiseHeight > tileThreshold)
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), tileBase);
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), null);
                    }
                }
            }
        }

        private void DrawTiles(float[,] noiseMap, TileRules tileRules)
        {
            int height = noiseMap.GetLength(1);
            int width = noiseMap.GetLength(0);
            bool[,] boolMap = new bool[width, height];
            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    float noiseHeight = noiseMap[x, y];
                    boolMap[x, y] = noiseHeight > tileThreshold;
                }
            }

            for (int y = 0; y < height; y += 1)
            {
                for (int x = 0; x < width; x += 1)
                {
                    if (boolMap[x, y])
                    {
                        TileBase tileBase = tileRules.GetSprite(boolMap, x, y);
                        if(tileBase != null)
                        {
                            tilemap.SetTile(new Vector3Int(x, y, 0), tileBase);
                        }
                        else
                        {
                            tilemap.SetTile(new Vector3Int(x, y, 0), this.tileBase);
                        }
                        
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), null);
                    }
                    
                }
            }
        }

    }
}