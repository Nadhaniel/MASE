using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, Mesh, FalloffMap };

    public bool useFalloff;
    float[,] falloffmap;

    public DrawMode drawMode;

    public TerrainData Terraindata;
    public NoiseData Noisedata;
    public Toggle toggle;

    public bool autoUpdate;

    public void OnValuesUpdated()
    {
        Noisedata.seed += 1;
        DrawMapInEditor();
    }

    private void Awake()
    {
        falloffmap = FalloffGen.GenerateFalloffMap(Noisedata.mapHeight);
    }

    public void DrawMapInEditor()
    {
        if (toggle.isOn)
        {
            Terraindata.useFalloff = true;
        }
        else
        {
            Terraindata.useFalloff = false;
        }

        MapData mapData = GenerateMap(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap)
        {
            display.Drawtexture(TextureGeneration.TexturefromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, Terraindata.meshHeightMultplier, Terraindata.meshHeightCurve));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.Drawtexture(TextureGeneration.TexturefromHeightMap(FalloffGen.GenerateFalloffMap(Noisedata.mapHeight)));
        }

    }
    MapData GenerateMap(Vector2 centre)
    {
        float[,] noisemap = Noise.GenerateNoiseMap(Noisedata.mapWidth, Noisedata.mapHeight, Noisedata.seed, Noisedata.noiseScale, Noisedata.octaves, Noisedata.persistance, Noisedata.lacunarity, centre + Noisedata.offset);
        if (Terraindata.useFalloff)
        {
            if (falloffmap == null)
            {
                falloffmap = FalloffGen.GenerateFalloffMap(Noisedata.mapHeight);
            }
        }
        for (int y = 0; y < Noisedata.mapHeight; y++)
        {
            for (int x = 0; x < Noisedata.mapWidth; x++)
            {
                if (Terraindata.useFalloff)
                {
                    noisemap[x, y] = Mathf.Clamp01(noisemap[x, y] - falloffmap[x, y]);
                }
            }
        }

        return new MapData(noisemap);
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
        falloffmap = FalloffGen.GenerateFalloffMap(Noisedata.mapHeight);
    }

    public struct MapData {
        public readonly float[,] heightMap;

        public MapData(float[,] heightMap)
        {
            this.heightMap = heightMap;
        }
    }
}
