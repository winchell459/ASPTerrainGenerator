using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sebastian
{
    public class MapGenerator : MonoBehaviour
    {
        public enum DrawMode { NoiseMap, ColorMap, Mesh, FalloffMap}
        public DrawMode drawMode;

        public bool useFlatShading;

        public int mapWidth;
        public int mapHeight;
        public float noiseScale;

        public int octaves;
        [Range(0,1)]
        public float persistance;
        public float lacunarity;

        public int seed;
        public Vector2 offset;

        public bool useFalloff;

        public float meshHeightMultiplier;
        public AnimationCurve meshHeightCurve;

        public bool autoUpdate;

        public TerrainType[] regions;


        float[,] falloffMap;

        private void Awake()
        {
            falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth);
        }

        public void GenerateMap()
        {
            float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale,octaves,persistance,lacunarity, offset);

            Color[] colorMap = new Color[mapWidth * mapHeight];
            for (int y = 0; y < mapHeight; y += 1)
            {
                for (int x = 0; x < mapWidth; x += 1)
                {
                    if (useFalloff)
                    {
                        noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                    }
                    colorMap[y * mapWidth + x] = regions[regions.Length - 1].color;
                    float currentHeight = noiseMap[x, y];
                    for (int i = 0; i < regions.Length; i += 1)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            colorMap[y * mapWidth + x] = regions[i].color;
                            break;
                        }
                    }
                }
            }

            MapDisplay display = FindObjectOfType<MapDisplay>();
            if(drawMode == DrawMode.NoiseMap) 
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
            else if(drawMode == DrawMode.ColorMap)
            {
                
                display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
            }else if(drawMode == DrawMode.Mesh)
            {
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve, useFlatShading), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
            }else if(drawMode == DrawMode.FalloffMap)
            {
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapWidth)));
            }
        }

        private void OnValidate()
        {
            if (mapWidth < 1) mapWidth = 1;
            if (mapHeight < 1) mapHeight = 1;
            if (lacunarity < 1) lacunarity = 1;
            if (octaves < 0) octaves = 0;

            falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth);
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}