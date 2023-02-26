using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    private Dictionary<string, float> genes;

    public DNA()
    {
        genes = new Dictionary<string, float>();
    }

    public void SetRandom()
    {
        genes.Clear();
        genes.Add("Speed", Random.Range(1f, 20f));
        genes.Add("Size", Random.Range(0.1f, 1f));
        genes.Add("Max_Size", Random.Range(0.5f, 2f));
        genes.Add("Strength", Random.Range(0f, 2f));
        genes.Add("Mutation_Size", Random.Range(0, 6));
        genes.Add("Mutation_Chance", Random.Range(0f, 2f));
        genes.Add("View_Distance", Random.Range(0f, 2f));
    }

    public void Combine(DNA d1, DNA d2)
    {
        int i = 0;
        Dictionary<string, float> combinedDNA = new Dictionary<string, float>();
        foreach (KeyValuePair<string, float> gene in genes)
        {
            if (i < genes.Count / 2)
            {
                combinedDNA.Add(gene.Key, d1.genes[gene.Key]);
            }
            else
            {
                combinedDNA.Add(gene.Key, d2.genes[gene.Key]);
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
