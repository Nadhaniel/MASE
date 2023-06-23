using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveRef : MonoBehaviour
{
    public SaveSimulationData saveData;

    public void Play()
    {
        SaveSimulationData.Current.noisedata = saveData.noisedata;
        SaveSimulationData.Current.terrainData = saveData.terrainData;
        SaveSimulationData.Current.creatures = saveData.creatures;
        SaveSimulationData.Current.plants = saveData.plants;
        SaveSimulationData.Current.savenumber = saveData.savenumber;
        SaveSimulationData.Current.savename = saveData.savename;
        SaveSimulationData.Current.species = saveData.species;
        SaveSimulationData.Current.GenerationNumber = saveData.GenerationNumber;
        MapGen_TerrainScene.spawnfromsave = true;
        SimulationManager.spawnfromsave = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Delete()
    {
        SavingManager.DeleteSimSave(saveData.savenumber);
        Destroy(this.gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
