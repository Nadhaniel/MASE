using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class SaveSimulationData
{
    private static SaveSimulationData current;
    public static SaveSimulationData Current 
    {
        get
        {
            if (current == null)
            {
                current = new SaveSimulationData();
            }
            return current;
        }
    }

    public string savename;
    public string dateTime;
    public int savenumber;
    public int GenerationNumber;
    public NoiseDataJSON noisedata = new NoiseDataJSON();
    public TerrainDataJSON terrainData = new TerrainDataJSON();
    public SaveCreature[] creatures;
    public PlantLocation[] plants;
    public List<SpeciesJSON> species;
}

[System.Serializable]
public class BestCreature
{
    public int save_no;
    public float fitness;
    public DNAJson dnaJSON = new DNAJson();
    public NNGenomeJson NNGenomeJson = new NNGenomeJson();
}

[System.Serializable]
public class PlantLocation
{
    public Vector3 position;
}
[System.Serializable]
public class SaveCreature
{
    public Vector3 position;
    public int foodcollected;
    public float energy;
    public float size;
    public float hunger;
    public float health;
    public float maturity;
    public float Time_Alive;
    public string name;
    public DNAJson dnaJSON = new DNAJson();
    public NNGenomeJson NNGenomeJson = new NNGenomeJson();
}

[System.Serializable]
public class DNAJson
{
    public List<float> genes;
}

[System.Serializable]
public class NNGenomeJson
{
    public List<NodeGeneJson> nodeGenes = new List<NodeGeneJson>();
    public List<ConnectionGeneJson> connectionGenes = new List<ConnectionGeneJson>();
    public int species_num;
}

[System.Serializable]
public class NodeGeneJson
{
    public int id;
    public Node_Type type;
}

[System.Serializable]
public class ConnectionGeneJson
{
    public int SourceNode;
    public int ReceivingNode;
    public float weight;
    public bool disabled;
    public int innovation_no;
}

[System.Serializable]
public class TerrainDataJSON
{
    public float uniformScale;
    public bool useFalloff;
    public float meshHeightMultplier;
    public AnimationCurve meshHeightCurve;
}

[System.Serializable]
public class NoiseDataJSON
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
}

[Serializable]
public struct JsonDateTime
{
    public long value;
    public static implicit operator DateTime(JsonDateTime jdt)
    {
        return DateTime.FromFileTimeUtc(jdt.value);
    }
    public static implicit operator JsonDateTime(DateTime dt)
    {
        JsonDateTime jdt = new JsonDateTime();
        jdt.value = dt.ToFileTimeUtc();
        return jdt;
    }
}

[System.Serializable]
public class SpeciesJSON
{
    public int species_num;
    public List<NNGenomeJson> Members;
    public DNAJson dna;
    public List<float> fitness_members;
    public float AverageF;
    public int GensSinceImprovement;
}