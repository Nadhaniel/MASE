using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager instance;
    private int MaxPopulation;
    private int minpop;
    public GameObject creature;
    public List<GameObject> creatures;
    public List<GameObject> fittestcreatures;
    public static float elapsedtime = 0;
    public float timeScale = 3;
    int generation = 0;
    public float trialTime = 30;

    private float minHeight;
    private float maxHeight;

    private void Start()
    {
        instance = this;
        creatures = GameObject.FindGameObjectsWithTag("Creature").ToList();
        MaxPopulation = 100;
        Generate();
        creatures = GameObject.FindGameObjectsWithTag("Creature").ToList();
        minpop = 50;
        generation += 1;
        Time.timeScale = timeScale;
    }

    private void Update()
    {
        creatures = GameObject.FindGameObjectsWithTag("Creature").ToList();
        if (creatures.Count < minpop)
        {
            fittestcreatures = creatures.OrderByDescending(x => x.GetComponent<Creature>().Fitness).ToList();
            for (int i = 0; i < 50; i++)
            {
                Reproduce(fittestcreatures[0], fittestcreatures[1]);
            }
        }
    }

    public void Generate()
    {
        //Clear();
        Vector2 xRange = new Vector2(-1000, 1000);
        Vector2 zRange = new Vector2(-1000, 1000);
        int popCount = creatures.Count;
        minHeight = 10;
        maxHeight = 16;
        int i = 0;
        while (i < MaxPopulation)
        {
            float sampleX = Random.Range(xRange.x, xRange.y);
            float sampleY = Random.Range(zRange.x, zRange.y);
            Vector3 rayStart = new Vector3(sampleX, maxHeight, sampleY);
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.point.y > minHeight)
                {
                    var newcreature = Instantiate(creature, hit.point, Quaternion.identity);
                    newcreature.name = "Creature: " + (popCount + 1);
                    popCount += 1;
                    i++;
                }
            }
        }
    }
    public void CreaturePlacement(GameObject creature)
    {
        Vector2 xRange = new Vector2(-1000, 1000);
        Vector2 zRange = new Vector2(-1000, 1000);
        minHeight = 10;
        maxHeight = 16;
        int i = 0;
        while (i < 1)
        {
            float sampleX = Random.Range(xRange.x, xRange.y);
            float sampleY = Random.Range(zRange.x, zRange.y);
            Vector3 rayStart = new Vector3(sampleX, maxHeight, sampleY);
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.point.y > minHeight)
                {
                    creature.transform.position = hit.point;
                    i++;
                }
            }
        }
    }

    //public void Clear()
    //{
    //    for (int i = 0; i < AgentPopulation.Count; i++)
    //    {
    //        DestroyImmediate(AgentPopulation[i]);
    //    }
    //    AgentPopulation.Clear();
    //}

    public void MaxPopControl()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Creature");
        if (objects.Length > 600)
        {
            Destroy(objects[objects.Length - 1].gameObject);
        }
    }

    public void Reproduce(GameObject parent1, GameObject parent2)
    {
        Transform pos = parent1.transform;
        GameObject offspring = Instantiate(creature, parent1.transform.position, Quaternion.identity);
        CreaturePlacement(offspring);
        DNA newDNA = new DNA();
        offspring.GetComponent<Creature>().dna = newDNA;
        offspring.GetComponent<Creature>().brain = parent1.GetComponent<Creature>().brain;
        offspring.GetComponent<Creature>().isChild = true;
        int popcount = creatures.Count;
        offspring.name = "Creature: " + (popcount + 1);
        if (UnityEngine.Random.Range(0, parent1.GetComponent<Creature>().dna.getGene("Mutation_Chance")) == 1)
        {
            offspring.GetComponent<Creature>().brain.MutateBrain();
            offspring.GetComponent<Creature>().dna.Combine(parent1.GetComponent<Creature>().dna.Genes, parent1.GetComponent<Creature>().dna.Genes);
            offspring.GetComponent<Creature>().dna.RandomizeGeneSet(parent2.transform.GetComponent<Creature>().dna.getGene("Mutation_Size"));
        }
        else
        {
            offspring.GetComponent<Creature>().dna.Combine(parent1.GetComponent<Creature>().dna.Genes, parent2.GetComponent<Creature>().dna.Genes);
        }
    }

    //Time controls
    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Play()
    {
        Time.timeScale = 1;
    }
    public void FastForward()
    {
        Time.timeScale = 2;
    }
    public void FastForwardx2()
    {
        Time.timeScale = 6;
    }

}
