using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class EscapeMenu : MonoBehaviour
{
    bool activable;
    bool active;
    public GameObject escapePanel;

    private void Start()
    {
        escapePanel.SetActive(false);
    }

    private void Update()
    {
        activable = !InfoGetter.instance.isSelected;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (active == false && activable == true)
            {
                Time.timeScale = 0;
                active = true;
                escapePanel.SetActive(active);
            }
            else
            {
                Time.timeScale = 1;
                active = false;
                escapePanel.SetActive(active);
            }
        }
    }
    public void Save()
    {
        SaveSimulationData update = new SaveSimulationData();
        update.terrainData = SaveSimulationData.Current.terrainData;
        update.noisedata = SaveSimulationData.Current.noisedata;
        update.creatures = Creature_to_data();
        update.plants = Plant_to_data();
        update.savenumber = SaveSimulationData.Current.savenumber;
        update.savename = SaveSimulationData.Current.savename;
        update.dateTime = JsonUtility.ToJson((JsonDateTime)DateTime.Now);
        update.species = Species_to_data(SimulationManager.instance.speciesInfo);
        update.GenerationNumber = SimulationManager.instance.generation;
        SavingManager.UpdateSim(update, SaveSimulationData.Current.savenumber);
    }

    public void Settings()
    {
        
    }

    public void Quit()
    {
        SaveSimulationData update = new SaveSimulationData();
        update.terrainData = SaveSimulationData.Current.terrainData;
        update.noisedata = SaveSimulationData.Current.noisedata;
        update.creatures = Creature_to_data();
        update.plants = Plant_to_data();
        update.savenumber = SaveSimulationData.Current.savenumber;
        update.savename = SaveSimulationData.Current.savename;
        update.dateTime = JsonUtility.ToJson((JsonDateTime)DateTime.Now);
        update.species = Species_to_data(SimulationManager.instance.speciesInfo);
        update.GenerationNumber = SimulationManager.instance.generation;
        SavingManager.UpdateSim(update, SaveSimulationData.Current.savenumber);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }

    public SaveCreature[] Creature_to_data() //Finds all the creatures currently in the sim and converts them to saveable data
    {
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");
        SaveCreature[] creaturesave = new SaveCreature[creatures.Length];
        for (int i = 0; i < creatures.Length; i++)
        {
            creaturesave[i] = new SaveCreature();
            creaturesave[i].position = creatures[i].transform.position;
            creaturesave[i].foodcollected = creatures[i].GetComponent<Creature>().foodcollected;
            creaturesave[i].energy = creatures[i].GetComponent<Creature>().energy;
            creaturesave[i].size = creatures[i].GetComponent<Creature>().size;
            creaturesave[i].hunger = creatures[i].GetComponent<Creature>().hunger;
            creaturesave[i].health = creatures[i].GetComponent<Creature>().health;
            creaturesave[i].maturity = creatures[i].GetComponent<Creature>().maturity;
            creaturesave[i].Time_Alive = creatures[i].GetComponent<Creature>().Time_Alive;
            creaturesave[i].dnaJSON.genes = creatures[i].GetComponent<Creature>().dna.Genes.Values.ToList();
            creaturesave[i].name = creatures[i].GetComponent<Creature>().name;
            creaturesave[i].NNGenomeJson.nodeGenes = NEATUtils.SetGenomeJSON(creatures[i].GetComponent<Creature>().brain.NN_genome).Item1;
            creaturesave[i].NNGenomeJson.connectionGenes = NEATUtils.SetGenomeJSON(creatures[i].GetComponent<Creature>().brain.NN_genome).Item2;
            creaturesave[i].NNGenomeJson.species_num = creatures[i].GetComponent<Creature>().brain.NN_genome.species_num;
        }

        return creaturesave;
    }

    public PlantLocation[] Plant_to_data() //Finds all the plants positions currently in the sim and converts them to saveable data
    {
        GameObject[] plant = GameObject.FindGameObjectsWithTag("Food");
        PlantLocation[] plantsave = new PlantLocation[plant.Length];
        for (int i = 0; i < plant.Length; i++)
        {
            plantsave[i] = new PlantLocation();
            plantsave[i].position = plant[i].transform.position;
        }

        return plantsave;
    }

    public List<SpeciesJSON> Species_to_data(List<Species> species_to_be_saved)
    {
        List<SpeciesJSON> save = new List<SpeciesJSON>();
        for (int i = 0; i < species_to_be_saved.Count; i++)
        {
            save.Add(new SpeciesJSON());
            save.Last().species_num = species_to_be_saved[i].species_num;
            save.Last().Members = new List<NNGenomeJson>();
            save.Last().fitness_members = new List<float>();
            for (int g = 0; g < species_to_be_saved[i].Members.Count; g++)
            {
                NNGenomeJson member = new NNGenomeJson();
                member.nodeGenes = NEATUtils.SetGenomeJSON(species_to_be_saved[i].Members[g]).Item1;
                member.connectionGenes = NEATUtils.SetGenomeJSON(species_to_be_saved[i].Members[g]).Item2;
                member.species_num = species_to_be_saved[i].Members[g].species_num;
                save.Last().Members.Add(member);
                save.Last().fitness_members.Add(species_to_be_saved[i].fitness_members[g]);
            }
            save.Last().AverageF = species_to_be_saved[i].AverageF;
            DNAJson dna = new DNAJson();
            dna.genes = species_to_be_saved[i].dna.Genes.Values.ToList();
            save.Last().dna = dna;
            save.Last().GensSinceImprovement = species_to_be_saved[i].GensSinceImprovement;
        }

        return save;
    }
}
