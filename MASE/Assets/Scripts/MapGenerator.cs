using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh };

    public DrawMode drawMode;

    public TerrainData Terraindata;
    public NoiseData Noisedata;

    public bool autoUpdate;

    public TerrainType[] regions;

    void OnValuesUpdated()
    {
        if (!Application.isPlaying) {
            GenerateMap();
        }
    }
    public void GenerateMap()
    {
        float[,] noisemap = Noise.GenerateNoiseMap(Noisedata.mapWidth, Noisedata.mapHeight, Noisedata.seed, Noisedata.noiseScale, Noisedata.octaves, Noisedata.persistance, Noisedata.lacunarity, Noisedata.offset);
        Color[] colorMap = new Color[Noisedata.mapWidth * Noisedata.mapHeight];
        for (int y = 0; y < Noisedata.mapHeight; y++)
        {
            for (int x = 0; x < Noisedata.mapWidth; x++)
            {
                float currentHeight = noisemap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {

                        colorMap[y * Noisedata.mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.Drawtexture(TextureGen.TexturefromHeightMap(noisemap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.Drawtexture(TextureGen.TexturefromColormap(colorMap, Noisedata.mapWidth, Noisedata.mapHeight));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGen.GenerateTerrainMesh(noisemap, Terraindata.meshHeightMultplier, Terraindata.meshHeightCurve), TextureGen.TexturefromColormap(colorMap, Noisedata.mapWidth, Noisedata.mapHeight));
        }
    }

    private void OnValidate()
    {
        if (Terraindata != null) {
            Terraindata.OnValuesUpdated -= OnValuesUpdated;
            Terraindata.OnValuesUpdated += OnValuesUpdated;
        }
        if (Noisedata != null) {
            Noisedata.OnValuesUpdated -= OnValuesUpdated;
            Noisedata.OnValuesUpdated += OnValuesUpdated;
        }
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string name;
        public float height;
        public Color color;
    }
}
