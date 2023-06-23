using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DNA
{
    private Dictionary<string, float> genes;

    public DNA()
    {
        genes = new Dictionary<string, float>();
        SetInitialGenome();
    }

    public Dictionary<string, float> Genes
    {
        get { return genes; }
        set { genes = value; }
    }

    public void SetInitialGenome()
    {
        genes.Clear();
        genes.Add("Speed", Random.Range(1f, 20f));
        genes.Add("Size", Random.Range(0.1f, 1f));
        genes.Add("Max_Size", Random.Range(genes["Size"], 2f));
        genes.Add("Strength", Random.Range(0f, 2f));
        genes.Add("Mutation_Size", Random.Range(0, 10));
        genes.Add("Mutation_Chance", Random.Range(0, 5));
        genes.Add("View_Distance", Random.Range(1, 10));
        genes.Add("Red_Color", Random.Range(0f, 1f));
        genes.Add("Green_Color", Random.Range(0f, 1f));
        genes.Add("Blue_Color", Random.Range(0f, 1f));
    }

    public void RandomizeGeneSet(float Mutation_Size)
    {
        
        for (int i = 0; i < Mutation_Size; i++)
        {
            var item = genes.ElementAt(Random.Range(0, 10));
            RandomizeSpecificGene(item.Key);
        }
    }

    public void RandomizeSpecificGene(string GeneKey)
    {
        float float_value = 0f;
        int int_value = 0; 
        switch (GeneKey)
        {
            case "Speed":
                float_value = Random.Range(1f, 20f);
                genes[GeneKey] = float_value;
                break;
            case "Size":
                float_value = Random.Range(0.1f, 1f);
                genes[GeneKey] = float_value;
                break;
            case "Max_Size":
                float_value = Random.Range(genes["Size"], 2f);
                genes[GeneKey] = float_value;
                break;
            case "Strength":
                float_value = Random.Range(0f, 2f);
                genes[GeneKey] = float_value;
                break;
            case "Mutation_Size":
                int_value = Random.Range(0, 10);
                genes[GeneKey] = int_value;
                break;
            case "Mutation_Chance":
                int_value = Random.Range(1, 20);
                genes[GeneKey] = int_value;
                break;
            case "View_Distance":
                float_value = Random.Range(1, 10);
                genes[GeneKey] = int_value;
                break;
            case "Red_Color":
                float_value = Random.Range(0f, 1f);
                genes[GeneKey] = int_value;
                break;
            case "Green_Color":
                float_value = Random.Range(0f, 1f);
                genes[GeneKey] = int_value;
                break;
            case "Blue_Color":
                float_value = Random.Range(0f, 1f);
                genes[GeneKey] = int_value;
                break;
        }
    }

    public void Combine(Dictionary<string, float> genes1, Dictionary<string, float> genes2)
    {
        int i = 0;
        Dictionary<string, float> combinedDNA = new Dictionary<string, float>();
        foreach (KeyValuePair<string, float> gene in genes)
        {
            if (i < genes.Count / 2)
            {
                combinedDNA.Add(gene.Key, genes1[gene.Key]);
            }
            else
            {
                combinedDNA.Add(gene.Key, genes2[gene.Key]);
            }
            i++;
        }
        genes = combinedDNA;
    }

    public float getGene(string key)
    {
        return genes[key];
    }
}
