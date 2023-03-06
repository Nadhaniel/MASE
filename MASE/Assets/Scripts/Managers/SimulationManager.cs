using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public int MaxPopulation = 150;
    public GameObject creature;
    public GameObject FittestAgent;
    List<GameObject> AgentPopulation = new List<GameObject>();
    List<GameObject> sortedList = new List<GameObject>();
    public static float elapsedtime = 0;
    public float timeScale = 1;
    int generation = 0;
    public float trialTime = 10;
    public GameObject spawner;

    private void Start()
    {
        for (int i = 0; i < MaxPopulation; i++)
        {
            GameObject agent = creature;
            AgentPopulation.Add(Instantiate(agent, spawner.transform.position, spawner.transform.rotation));
        }
        generation += 1;
        Time.timeScale = timeScale;
    }

    private void Update()
    {
        elapsedtime += Time.deltaTime;
        //Debug.Log(elapsedtime);
        //sortedList = AgentPopulation.OrderByDescending(x => x.GetComponent<Creature>().Fitness).ToList();
        if (elapsedtime >= trialTime)
        {
            //FittestAgent = sortedList.First();
            //Debug.Log("Fittest Creature: " + FittestAgent.GetComponent<Creature>().Fitness);
            BreedPopNew();
            elapsedtime = 0;
        }
    }

    private GameObject Breed(GameObject parent1, GameObject parent2) //May move this method to creature class as its more appropriate there
    {
        GameObject offspring = Instantiate(creature, spawner.transform.position, spawner.transform.rotation);
        DNA newDNA = new DNA();
        offspring.GetComponent<Creature>().dna = newDNA;
        offspring.GetComponent<Creature>().brain = parent1.GetComponent<Creature>().brain;
        if (Random.Range(0, 3) == 1)
        {
            offspring.GetComponent<Creature>().brain.MutateBrain();
            offspring.GetComponent<Creature>().dna.Combine(parent1.GetComponent<Creature>().dna.Genes, parent2.GetComponent<Creature>().dna.Genes);
            offspring.GetComponent<Creature>().dna.RandomizeGeneSet(parent1.GetComponent<Creature>().dna.getGene("Mutation_Size")); //Mutates a number of genes dependant on mutation_size
        }
        else
        {
            offspring.GetComponent<Creature>().isChild = true;
            offspring.GetComponent<Creature>().dna.Combine(parent1.GetComponent<Creature>().dna.Genes, parent2.GetComponent<Creature>().dna.Genes);
        }
        return offspring;
    }

    private void BreedPopNew()
    {
        sortedList = AgentPopulation.OrderByDescending(o => o.GetComponent<Creature>().Fitness).ToList();

        string Generation = "Generation: " + this.generation;
        foreach (GameObject g in sortedList)
        {
            Generation += ", " + g.GetComponent<Creature>().Fitness;
        }
        //Debug.Log("Fitness: " + Generation);
        AgentPopulation.Clear();

        while (AgentPopulation.Count < MaxPopulation)
        {
            int bestParentCutoff = sortedList.Count / 4;
            for (int i = 0; i < bestParentCutoff - 1; i++)
            {
                for (int j = 1; j < bestParentCutoff; j++)
                {
                    AgentPopulation.Add(Breed(sortedList[i], sortedList[j]));
                    if (AgentPopulation.Count == MaxPopulation) break;
                    AgentPopulation.Add(Breed(sortedList[j], sortedList[i]));
                    if (AgentPopulation.Count == MaxPopulation) break;
                }
                if (AgentPopulation.Count == MaxPopulation) break;
            }
        }
        for (int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }
        this.generation++;
    }
    
    private void BreedPop()
    {
        if (generation == 1)
        {
            ClearDead(AgentPopulation);
            AgentPopulation.Clear();
            GameObject child = FittestAgent;
            AgentPopulation.Add(Instantiate(child, spawner.transform.position, spawner.transform.rotation));
            for (int i = 0; i < MaxPopulation; i++)
            {
                GameObject agent = Instantiate(creature, spawner.transform.position, spawner.transform.rotation);
                AgentPopulation.Add(agent);
            }
            generation += 1;
        }
        else
        {
            FittestAgent = Breed(FittestAgent, sortedList.First());
            ClearDead(AgentPopulation);
            AgentPopulation.Clear();
            for (int i = 0; i < MaxPopulation; i++)
            {
                GameObject agent = Instantiate(creature, spawner.transform.position, spawner.transform.rotation);
                AgentPopulation.Add(agent);
            }
            generation += 1;
        }
            
    }

    private void ClearDead(List<GameObject> Population)
    {
        foreach (GameObject creature in Population)
        {
            if (creature.GetComponent<Creature>().isdead == true)
            {
                Destroy(creature);
            }
        }
    }                                                                 
}
