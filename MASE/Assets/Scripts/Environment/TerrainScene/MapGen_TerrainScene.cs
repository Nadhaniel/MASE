using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen_TerrainScene : MonoBehaviour
{

    public bool useFalloff;
    float[,] falloffmap;

    public TerrainData Terraindata;
    public NoiseData Noisedata;

    public bool autoUpdate;
    public static bool spawnfromsave = false;

    private void Start()
    {
        Terraindata = new TerrainData();
        Noisedata = new NoiseData();
        Terraindata.uniformScale = SaveSimulationData.Current.terrainData.uniformScale;
        Terraindata.useFalloff = SaveSimulationData.Current.terrainData.useFalloff;
        Terraindata.meshHeightMultplier = SaveSimulationData.Current.terrainData.meshHeightMultplier;
        Terraindata.meshHeightCurve = SaveSimulationData.Current.terrainData.meshHeightCurve;
        Noisedata.mapWidth = SaveSimulationData.Current.noisedata.mapWidth;
        Noisedata.mapHeight = SaveSimulationData.Current.noisedata.mapHeight;
        Noisedata.noiseScale = SaveSimulationData.Current.noisedata.noiseScale;
        Noisedata.persistance = SaveSimulationData.Current.noisedata.persistance;
        Noisedata.lacunarity = SaveSimulationData.Current.noisedata.lacunarity;
        Noisedata.seed = SaveSimulationData.Current.noisedata.seed;
        Noisedata.offset = SaveSimulationData.Current.noisedata.offset;
        Noisedata.octaves = SaveSimulationData.Current.noisedata.octaves;
        falloffmap = FalloffGen.GenerateFalloffMap(Noisedata.mapHeight);
        DrawMap();
    }

    public void DrawMap()
    {
        MapData mapData = GenerateMap(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, Terraindata.meshHeightMultplier, Terraindata.meshHeightCurve));
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

    public struct MapData
    {
        public readonly float[,] heightMap;

        public MapData(float[,] heightMap)
        {
            this.heightMap = heightMap;
        }
    }
}
