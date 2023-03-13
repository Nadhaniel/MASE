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

    private void Start()
    {
        Terraindata = SaveData.current.terrainData;
        Noisedata = SaveData.current.noisedata;
        falloffmap = FalloffGen.GenerateFalloffMap(Noisedata.mapHeight);
        DrawMap();
        this.GetComponent<PlacementGenerator>().Generate();
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
