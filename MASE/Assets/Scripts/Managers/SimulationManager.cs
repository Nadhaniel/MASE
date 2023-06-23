using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager instance;
    public static bool spawnfromsave = false;
    public static bool spawncreaturesave = true;
    private int MaxPopulation;
    private int MinPopulation;
    public GameObject creature;
    public GameObject food;
    public List<GameObject> creatures;
    public GameObject[] foods;
    public Brain[] brains;
    public DNA[] dnas;
    public List<Species> speciesInfo;
    public static float elapsedtime = 0;
    public float timeScale = 3;
    public int generation = 0;
    public int inputNodes, outputNodes, hiddenNodes;
    public int currentAlive;
    public int best, worst;
    public int bestTime = 100;
    public int addtoBest = 50;
    public float threshold = 4.0f;
    public int TargetSpecies = 5;
    public int species_count = 0;
    public bool repoping = false;
    int repopcount = 0;

    private float minHeight;
    private float maxHeight;

    private void Start()
    {
        instance = this;
        worst = 280;
        best = 0;
        inputNodes = 10;
        outputNodes = 7;
        hiddenNodes = 13;
        MinPopulation = 100;
        creatures = new List<GameObject>();
        foods = new GameObject[1000];
        brains = new Brain[MinPopulation];
        dnas = new DNA[MinPopulation];
        speciesInfo = new List<Species>();
        if (spawnfromsave == true)
        {
            generation = SaveSimulationData.Current.GenerationNumber;
            for (int i = 0; i < SaveSimulationData.Current.plants.Length; i++)
            {
                foods[i] = Instantiate(food, this.transform);
                foods[i].transform.position = SaveSimulationData.Current.plants[i].position;
                foods[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            for (int i = 0; i < SaveSimulationData.Current.creatures.Length; i++)
            {
                GameObject temp = CreaturePool.instance.GetPooledObject();
                if (temp != null)
                {
                    temp.SetActive(true);
                    creatures.Add(temp);
                    creatures.Last().GetComponent<Creature>().brain = new Brain(NEATUtils.SetGenome(SaveSimulationData.Current.creatures.Last().NNGenomeJson));
                    creatures.Last().GetComponent<Creature>().dna = new DNA();
                    creatures.Last().GetComponent<Creature>().dna.Genes["Speed"] = SaveSimulationData.Current.creatures[i].dnaJSON.genes[0];
                    creatures.Last().GetComponent<Creature>().dna.Genes["Size"] = SaveSimulationData.Current.creatures[i].dnaJSON.genes[1];
                    creatures.Last().GetComponent<Creature>().dna.Genes["Max_Size"] = SaveSimulationData.Current.creatures[i].dnaJSON.genes[2];
                    creatures.Last().GetComponent<Creature>().dna.Genes["Strength"] = SaveSimulationData.Current.creatures[i].dnaJSON.genes[3];
                    creatures.Last().GetComponent<Creature>().dna.Genes["Mutation_Size"] = SaveSimulationData.Current.creatures[i].dnaJSON.genes[4];
                    creatures.Last().GetComponent<Creature>().dna.Genes["Mutation_Chance"] = SaveSimulationData.Current.creatures[i].dnaJSON.genes[5];
                    creatures.Last().GetComponent<Creature>().dna.Genes["View_Distance"] = SaveSimulationData.Current.creatures[i].dnaJSON.genes[6];
                    creatures.Last().GetComponent<Creature>().dna.Genes["Red_Color"] = SaveSimulationData.Current.creatures[i].dnaJSON.genes[7];
                    creatures.Last().GetComponent<Creature>().dna.Genes["Green_Color"] = SaveSimulationData.Current.creatures[i].dnaJSON.genes[8];
                    creatures.Last().GetComponent<Creature>().dna.Genes["Blue_Color"] = SaveSimulationData.Current.creatures[i].dnaJSON.genes[9];
                    creatures.Last().transform.position = SaveSimulationData.Current.creatures[i].position;
                    creatures.Last().GetComponent<Creature>().foodcollected = SaveSimulationData.Current.creatures[i].foodcollected;
                    creatures.Last().GetComponent<Creature>().energy = SaveSimulationData.Current.creatures[i].energy;
                    creatures.Last().GetComponent<Creature>().size = SaveSimulationData.Current.creatures[i].size;
                    creatures.Last().GetComponent<Creature>().hunger = SaveSimulationData.Current.creatures[i].hunger;
                    creatures.Last().GetComponent<Creature>().health = SaveSimulationData.Current.creatures[i].health;
                    creatures.Last().GetComponent<Creature>().maturity = SaveSimulationData.Current.creatures[i].maturity;
                    creatures.Last().GetComponent<Creature>().Time_Alive = SaveSimulationData.Current.creatures[i].Time_Alive;
                    creatures.Last().name = SaveSimulationData.Current.creatures[i].name;

                    creatures.Last().GetComponent<Creature>().InputNodes = inputNodes;
                    creatures.Last().GetComponent<Creature>().Init();
                    brains[i] = new Brain(NEATUtils.SetGenome(SaveSimulationData.Current.creatures[i].NNGenomeJson));
                    brains[i].species_num = 0;
                    brains[i].createNetwork();
                    dnas[i] = new DNA();
                    dnas[i].Genes = creatures.Last().GetComponent<Creature>().dna.Genes;
                }
                
            }
            for (int i = SaveSimulationData.Current.creatures.Length; i < MinPopulation; i++)
            {
                brains[i] = new Brain(inputNodes, outputNodes, hiddenNodes);
                dnas[i] = new DNA();
            }
            generation = 1;
            Speciation();
            spawnfromsave = false;
        }
        else
        {
            this.GetComponent<PlacementGenerator>().Generate();
            StartNetworks();
            MutatePop();
            Generate();
            Speciation();
        }
    }

    private void FixedUpdate()
    {
        currentAlive = CurrentAlive();
        Debug.Log(currentAlive);
        if (repoping == false && currentAlive < MinPopulation)
        {
            repoping = true;
            Repopulate(repopcount);
            repopcount += 1;
            repoping = false;
        }
        if (repopcount > 20)
        {
            repopcount = 0;
        }
        if (CurrentPlants() < 400)
        {
            foods = this.GetComponent<PlacementGenerator>().Generate();
        }
    }


    public int CurrentAlive()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Creature");
        if (objects.Length > 600)
        {
            objects[objects.Length - 1].gameObject.SetActive(false);
        }
        return objects.Length;
    }

    public int CurrentPlants()
    {
        int plants = 0;
        for (int i = 0; i < foods.Length; i++)
        {
            if (foods[i] != null)
            {
                plants += 1;
            }
        }
        return plants;
    }

    private void Repopulate(int repopCount)
    {
        generation += 1;
        SortFitness_Pop();
        if (generation == 1)
        {
            Speciation();
        }
        else if (repopCount == 20)
        {
            Speciation();
        }
        (Brain, DNA) creature_component = OffspringProduction();
        AddOffspring(creature_component.Item1, creature_component.Item2);
    }
    private void SortFitness_Pop()
    {
        for (int i = 0; i < brains.Length; i++)
        {
            for (int y = 0; y < brains.Length; y++)
            {
                if (brains[i].adjFitness > brains[y].adjFitness)
                {
                    Brain tempbrain = brains[i];
                    brains[i] = brains[y];
                    brains[y] = tempbrain;
        
                    DNA tempdna = dnas[i];
                    dnas[i] = dnas[y];
                    dnas[y] = tempdna;
                }
            }
        }
    }

    #region NEAT/RT-NEAT
    private void Speciation()
    {
        int randomindex = 0;
        int id_count = 0;
        if (generation == 1)
        {
            while (brains.Any(x => x.species_num == 0))
            {
                randomindex = UnityEngine.Random.Range(0, brains.Length);
                while (brains[randomindex].species_num == 0)
                {
                    species_count += 1;
                    id_count += 1;
                    Species newSpecies = new Species(species_count);
                    newSpecies.GensSinceImprovement = 0;
                    speciesInfo.Add(newSpecies);
                    brains[randomindex].species_num = species_count;
                    brains[randomindex].NN_genome.species_num = species_count;
                    for (int i = 0; i < brains.Length; i++)
                    {
                        float compare_num = NEATUtils.NetworkComparison(2, 2, 1f, brains[randomindex].NN_genome.connectionGenes, brains[i].NN_genome.connectionGenes);
                        if (brains[i].species_num == 0)
                        {
                            if (compare_num < threshold)
                            {
                                brains[i].species_num = brains[randomindex].species_num;
                                brains[i].NN_genome.species_num = brains[randomindex].species_num;
                            }
                        }
                    }
                }
            }
        }
        else if (generation >= 2)
        {
            speciesInfo.RemoveAll(x => x.Members.Count == 0);
            Brain[] inital_species = new Brain[speciesInfo.Count];
            System.Random rand = new System.Random();
            for (int i = 0; i < speciesInfo.Count; i++)
            {
                speciesInfo[i].Members.Clear();
                speciesInfo[i].fitness_members.Clear();
                var filtered = brains.Where(x => x.species_num == speciesInfo[i].species_num).ToArray();
                int randomind = rand.Next(filtered.Length);
                inital_species[i] = filtered[randomind];
            }
            for (int i = 0; i < brains.Length; i++)
            {
                if (!inital_species.Contains(brains[i]))
                {
                    brains[i].species_num = 0;
                    brains[i].NN_genome.species_num = 0;
                }
            }
            for (int i = 0; i < speciesInfo.Count; i++)
            {
                for (int x = 0; x < brains.Length; x++)
                {
                    if (brains[x].species_num == 0)
                    {
                        float compare_num = NEATUtils.NetworkComparison(2, 2, 1f, inital_species[i].NN_genome.connectionGenes, brains[x].NN_genome.connectionGenes);
                        if (compare_num < threshold)
                        {
                            brains[x].species_num = inital_species[i].species_num;
                            brains[x].NN_genome.species_num = inital_species[i].species_num;
                        }
                    }
                }
            }
            while (brains.Any(x => x.species_num == 0))
            {
                species_count += 1;
                speciesInfo.Add(new Species(species_count));
                var filtered = brains.Where(x => x.species_num == 0).ToArray();
                int index = rand.Next(filtered.Length);
                filtered[index].species_num = species_count;
                filtered[index].NN_genome.species_num = species_count;
                for (int x = 0; x < brains.Length; x++)
                {
                    float compare_num = NEATUtils.NetworkComparison(2, 2, 1f, filtered[index].NN_genome.connectionGenes, brains[x].NN_genome.connectionGenes);
                    if (brains[x].species_num == 0)
                    {
                        if (compare_num < threshold)
                        {
                            brains[x].species_num = filtered[index].species_num;
                            brains[x].NN_genome.species_num = filtered[index].species_num;
                        }
                    }
                }
            }
        }
        AdjustedFitnessCalc(species_count);
        for (int i = 0; i < speciesInfo.Count; i++)
        {
            float initial_fitness = speciesInfo[i].AverageF;
            if (initial_fitness >= AvgFitness_of_species(speciesInfo[i].species_num))
            {
                speciesInfo[i].AverageF = AvgFitness_of_species(speciesInfo[i].species_num);
                speciesInfo[i].GensSinceImprovement += 1;
                speciesInfo[i].Members = new List<NNGenome>();
                speciesInfo[i].fitness_members = new List<float>();
                speciesInfo[i].dna = new DNA();
                bool dnafound = false;
                for (int x = 0; x < brains.Length; x++)
                {
                    if (brains[x].species_num == speciesInfo[i].species_num)
                    {
                        if (dnafound == false)
                        {
                            for (int c = 0; c < creatures.Count; c++)
                            {
                                if (creatures[c] != null)
                                {
                                    if (creatures[c].GetComponent<Creature>().brain.id == brains[x].id)
                                    {
                                        speciesInfo[i].dna.Genes = creatures[c].GetComponent<Creature>().dna.Genes;
                                        break;
                                    }
                                }
                            }
                            dnafound = true;
                        }
                        speciesInfo[i].Members.Add(brains[x].NN_genome);
                        speciesInfo[i].fitness_members.Add(brains[x].adjFitness);
                    }
                }
            }
            else
            {
                speciesInfo[i].AverageF = AvgFitness_of_species(speciesInfo[i].species_num);
                speciesInfo[i].GensSinceImprovement = 0;
                speciesInfo[i].Members = new List<NNGenome>();
                speciesInfo[i].fitness_members = new List<float>();
                speciesInfo[i].dna = new DNA();
                bool dnafound = false;
                for (int x = 0; x < brains.Length; x++)
                {
                    if (brains[x].species_num == speciesInfo[i].species_num)
                    {
                        if (dnafound == false)
                        {
                            for (int c = 0; c < creatures.Count; c++)
                            {
                                if (creatures[c] != null)
                                {
                                    if (creatures[c].GetComponent<Creature>().brain.id == brains[x].id)
                                    {
                                        speciesInfo[i].dna.Genes = creatures[c].GetComponent<Creature>().dna.Genes;
                                        break;
                                    }
                                }
                            }
                            dnafound = true;
                        }
                        speciesInfo[i].Members.Add(brains[x].NN_genome);
                        speciesInfo[i].fitness_members.Add(brains[x].adjFitness);
                    }
                }
            }
        }
        if (species_count < TargetSpecies) //Compatibility threshold adjustment
        {
            threshold -= 0.3f;
        }
        else
        {
            threshold += 0.3f;
        }
    }

    private (Brain, DNA) OffspringProduction()
    {
        Species selected_species = RouletteWheelSelection_Species(speciesInfo);
        NNGenome parent1 = RouletteWheelSelection_Parent(selected_species);
        float parent1_fitness = 0f;
        NNGenome parent2 = RouletteWheelSelection_Parent(selected_species);
        float parent2_fitness = 0f;
        for (int x = 0; x < selected_species.Members.Count; x++)
        {
            bool parent1_found = false;
            bool parent2_found = false;
            if (selected_species.Members[x] == parent1)
            {
                parent1_fitness = selected_species.fitness_members[x];
                parent1_found = true;
            }
            if (selected_species.Members[x] == parent2)
            {
                parent2_fitness = selected_species.fitness_members[x];
                parent2_found = true;
            }
            if (parent1_found == true && parent2_found == true)
            {
                break;
            }
        }
        NNGenome clonedchild = null;
        if (parent1_fitness > parent2_fitness)
        {
            clonedchild = (NNGenome)Crossover(parent1, parent2).Clone();
        }
        else if (parent1_fitness < parent2_fitness)
        {
            clonedchild = (NNGenome)Crossover(parent2, parent1).Clone();
        }
        else if (parent1_fitness == parent2_fitness)
        {
            if (UnityEngine.Random.Range(1, 3) == 1)
            {
                clonedchild = (NNGenome)Crossover(parent1, parent2).Clone();
            }
            else
            {
                clonedchild = (NNGenome)Crossover(parent2, parent1).Clone();
            }
        }
        Brain offspringBrain = new Brain(clonedchild);
        offspringBrain.species_num = selected_species.species_num;
        return (offspringBrain, selected_species.dna);
    }

    private NNGenome Crossover(NNGenome MostFitParent, NNGenome LeastFitParent)
    {
        NNGenome clonedchild;
        List<ConnectionGene> leastfitmatchingconnections = new List<ConnectionGene>();
       
        clonedchild = (NNGenome)MostFitParent.Clone();
        for (int i = 0; i < clonedchild.connectionGenes.Count; i++)
        {
            for (int l = 0; l < LeastFitParent.connectionGenes.Count; l++)
            {
                if (clonedchild.connectionGenes[i].innovation_no == LeastFitParent.connectionGenes[l].innovation_no)
                {
                    leastfitmatchingconnections.Add(LeastFitParent.connectionGenes[l]);
                }
            }
        }
        for (int c = 0; c < clonedchild.connectionGenes.Count; c++)
        {
            clonedchild.connectionGenes[c].weight = 0f;
            if (UnityEngine.Random.Range(1, 3) == 1)
            {
                clonedchild.connectionGenes[c].weight = MostFitParent.connectionGenes.Single(x => x.innovation_no == clonedchild.connectionGenes[c].innovation_no).weight;
            }
            else
            {
                for (int l = 0; l < LeastFitParent.connectionGenes.Count; l++)
                {
                    if (clonedchild.connectionGenes[c].innovation_no == LeastFitParent.connectionGenes[l].innovation_no)
                    {
                        clonedchild.connectionGenes[c].weight = LeastFitParent.connectionGenes[l].weight;
                    }
                }
            }
        }

        return clonedchild;
    }

    private float AvgFitness_of_species(int species_num)
    {
        float AvgFitness_of_species = 0f;
        int count_num_of_species = brains.Count(x => x.species_num == species_num);
        for (int i = 0; i < brains.Length; i++)
        {
            if (brains[i].species_num == species_num)
            {
                AvgFitness_of_species += brains[i].adjFitness;
            }
        }
        AvgFitness_of_species = AvgFitness_of_species / count_num_of_species;

        return AvgFitness_of_species;
    }

    private void AdjustedFitnessCalc(int species_count)
    {
        int species_num = 0;
        for (int i = 0; i < species_count; i++)
        {
            species_num = brains.Count(x => x.species_num == i + 1);
            for (int x = 0; x < brains.Length; x++)
            {
                if (brains[x].species_num == i + 1)
                {
                    brains[x].adjFitness = brains[x].fitness / species_num;
                }
            }
        }
    }

    private float GlobalAvgFitness()
    {
        float global_avg = 0f;
        for (int i = 0; i < brains.Length; i++)
        {
            global_avg += brains[i].adjFitness;
        }

        return (global_avg / brains.Length);
    }

    private static NNGenome RouletteWheelSelection_Parent(Species species)
    {
        System.Random rand = new System.Random();
        float max_fitness = 0f;
        //Fitness proportion selection
        for (int i = 0; i < species.Members.Count; i++)
        {
            max_fitness += species.fitness_members[i];
        }
        float randomFitness = (float)(rand.NextDouble() * (double)max_fitness);
        float cumulativeFitness = 0f;
        for (int f = 0; f < species.fitness_members.Count; f++)
        {
            cumulativeFitness += species.fitness_members[f];
            if (cumulativeFitness >= randomFitness)
            {
                return species.Members[f];
            }
        }

        return null;
    }

    private static Species RouletteWheelSelection_Species(List<Species> species)
    {
        System.Random rand = new System.Random();
        float max_fitness = 0f;
        //Fitness proportion selection
        for (int i = 0; i < species.Count; i++)
        {
            max_fitness += species[i].AverageF;
        }
        float randomFitness = (float)(rand.NextDouble() * (double)max_fitness);
        float cumulativeFitness = 0f;
        for (int f = 0; f < species.Count; f++)
        {
            cumulativeFitness += species[f].AverageF;
            if (cumulativeFitness >= randomFitness)
            {
                return species[f];
            }
        }
        return null;
    }

    #endregion
    private void AddOffspring(Brain newbrain, DNA dna)
    {
        GameObject temp = CreaturePool.instance.GetPooledObject();
        if (temp != null)
        {
            temp.SetActive(true);
            GameObject creatureoffspring = temp;
            creatureoffspring.GetComponent<Creature>().brain = newbrain;
            creatureoffspring.GetComponent<Creature>().InputNodes = inputNodes;
            creatureoffspring.GetComponent<Creature>().brain.createNetwork();
            creatureoffspring.GetComponent<Creature>().dna = dna;
            creatureoffspring.GetComponent<Creature>().name = "Creature of generation: " + generation;
            creatureoffspring.GetComponent<Creature>().brain.MutateNN(1);
            if (UnityEngine.Random.Range(0, creatureoffspring.GetComponent<Creature>().dna.Genes["Mutation_Chance"])  == 1)
            {
                creatureoffspring.GetComponent<Creature>().dna.RandomizeGeneSet(creatureoffspring.GetComponent<Creature>().dna.Genes["Mutation_Size"]);
            }
            creatureoffspring.GetComponent<Creature>().Init();
            creatures.Add(creatureoffspring);
            CreaturePlacement(creatureoffspring);
        }
       
    }

    private void StartNetworks()
    {
        for (int i = 0; i < MinPopulation; i++)
        {
            brains[i] = new Brain(inputNodes, outputNodes, hiddenNodes);
            dnas[i] = new DNA();
        }
    }

    private void MutatePop()
    {
        for (int i = best; i < MinPopulation; i++)
        {
            brains[i].MutateNN(10);
            if (spawncreaturesave == true)
            {
                dnas[i].SetInitialGenome();
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
        while (i < MinPopulation)
        {
            float sampleX = UnityEngine.Random.Range(xRange.x, xRange.y);
            float sampleY = UnityEngine.Random.Range(zRange.x, zRange.y);
            Vector3 rayStart = new Vector3(sampleX, maxHeight, sampleY);
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.point.y > minHeight)
                {
                    GameObject temp = CreaturePool.instance.GetPooledObject();
                    if (temp != null)
                    {
                        temp.SetActive(true);
                        creatures.Add(temp);
                        creatures.Last().transform.position = hit.point;
                        creatures.Last().GetComponent<Creature>().brain = brains[i];
                        creatures.Last().GetComponent<Creature>().dna = dnas[i];
                        creatures.Last().GetComponent<Creature>().InputNodes = inputNodes;
                        creatures.Last().name = "Creature: " + (popCount + 1);
                        creatures.Last().GetComponent<Creature>().Init();
                        popCount += 1;
                        i++;
                    }
                }
            }
        }
    }

    public void CreaturePlacement(GameObject creature)
    {
        Vector2 xRange = new Vector2(-1000, 1000);
        Vector2 zRange = new Vector2(-1000, 1000);
        minHeight = 12;
        maxHeight = 16;
        int i = 0;
        while (i < 1)
        {
            float sampleX = UnityEngine.Random.Range(xRange.x, xRange.y);
            float sampleY = UnityEngine.Random.Range(zRange.x, zRange.y);
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

    #region Time Controls
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
        Time.timeScale = 4;
    }
    #endregion
}

public class Species
{
    public int species_num;
    public List<NNGenome> Members;
    public DNA dna;
    public List<float> fitness_members;
    public float AverageF;
    public int GensSinceImprovement;

    public Species(int species_num)
    {
        this.species_num = species_num;
    }
}
