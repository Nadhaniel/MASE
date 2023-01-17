using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, Mesh, FalloffMap };

    public bool useFalloff;
    float[,] falloffmap;

    public DrawMode drawMode;

    public TerrainData Terraindata;
    public NoiseData Noisedata;
    public TextureData Texturedata;

    public Material terrainMaterial;

    public bool autoUpdate;


    void OnValuesUpdated()
    {
        if (!Application.isPlaying) {
            DrawMapInEditor();
        }
    }

    void OnTextureValuesUpdated()
    {
        Texturedata.ApplyToMaterial(terrainMaterial);
    }

    private void Awake()
    {
        falloffmap = FalloffGen.GenerateFalloffMap(Noisedata.mapHeight);
    }

    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMap(Vector2.zero);

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap)
        {
            display.Drawtexture(TextureGen.TexturefromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGen.GenerateTerrainMesh(mapData.heightMap, Terraindata.meshHeightMultplier, Terraindata.meshHeightCurve));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.Drawtexture(TextureGen.TexturefromHeightMap(FalloffGen.GenerateFalloffMap(Noisedata.mapHeight)));
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
        Texturedata.UpdateMeshHeights(terrainMaterial, Terraindata.minHeight, Terraindata.maxHeight);

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
        if (Texturedata != null)
        {
            Texturedata.OnValuesUpdated -= OnTextureValuesUpdated;
            Texturedata.OnValuesUpdated += OnTextureValuesUpdated;
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
