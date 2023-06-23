using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject popupbox;
    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }

    public void Continue()
    {
        string[] lines = SavingManager.ReadSimLines();
        if (lines.Length > 0)
        {
            SaveSimulationData data = SavingManager.LoadSim(lines.Length - 1);
            SaveSimulationData.Current.noisedata = data.noisedata;
            SaveSimulationData.Current.terrainData = data.terrainData;
            SaveSimulationData.Current.creatures = data.creatures;
            SaveSimulationData.Current.plants = data.plants;
            SaveSimulationData.Current.savenumber = data.savenumber;
            SaveSimulationData.Current.savename = data.savename;
            SaveSimulationData.Current.species = data.species;
            SaveSimulationData.Current.GenerationNumber = data.GenerationNumber;
            MapGen_TerrainScene.spawnfromsave = true;
            SimulationManager.spawnfromsave = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        }
        else
        {
            popupbox.gameObject.SetActive(true);
        }
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player Has Quit");
    }

    public void Popupquit()
    {
        popupbox.gameObject.SetActive(false);
    }
}
