using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapCreationContinue : MonoBehaviour
{
    public GameObject MapGen;
    public GameObject popup;
    public GameObject textinput;
    public void MoveForward()
    {
        string savename = textinput.GetComponent<TMPro.TMP_InputField>().text;
        if (savename != "")
        {
            if (!CheckName(savename))
            {
                SaveSimulationData.Current.savename = savename;
                SaveSimulationData.Current.dateTime = JsonUtility.ToJson((JsonDateTime)DateTime.Now);
                SaveSimulationData.Current.noisedata.mapWidth = MapGen.GetComponent<MapGenerator>().Noisedata.mapWidth;
                SaveSimulationData.Current.noisedata.mapHeight = MapGen.GetComponent<MapGenerator>().Noisedata.mapHeight;
                SaveSimulationData.Current.noisedata.noiseScale = MapGen.GetComponent<MapGenerator>().Noisedata.noiseScale;
                SaveSimulationData.Current.noisedata.persistance = MapGen.GetComponent<MapGenerator>().Noisedata.persistance;
                SaveSimulationData.Current.noisedata.lacunarity = MapGen.GetComponent<MapGenerator>().Noisedata.lacunarity;
                SaveSimulationData.Current.noisedata.seed = MapGen.GetComponent<MapGenerator>().Noisedata.seed;
                SaveSimulationData.Current.noisedata.offset = MapGen.GetComponent<MapGenerator>().Noisedata.offset;
                SaveSimulationData.Current.noisedata.octaves = MapGen.GetComponent<MapGenerator>().Noisedata.octaves;
                SaveSimulationData.Current.terrainData.uniformScale = MapGen.GetComponent<MapGenerator>().Terraindata.uniformScale;
                SaveSimulationData.Current.terrainData.useFalloff = MapGen.GetComponent<MapGenerator>().Terraindata.useFalloff;
                SaveSimulationData.Current.terrainData.meshHeightMultplier = MapGen.GetComponent<MapGenerator>().Terraindata.meshHeightMultplier;
                SaveSimulationData.Current.terrainData.meshHeightCurve = MapGen.GetComponent<MapGenerator>().Terraindata.meshHeightCurve;
                SaveSimulationData.Current.creatures = null;
                SaveSimulationData.Current.plants = null;
                SaveSimulationData.Current.species = null;
                SavingManager.SaveNewSim(SaveSimulationData.Current);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else
            {
                popup.GetComponentInChildren<TextMeshProUGUI>().text = "Save name already taken!";
                Show();
            }
        }
        else
        {
            popup.GetComponentInChildren<TextMeshProUGUI>().text = "No save name given!";
            Show();
        }
    }

    public bool CheckName(string name)
    {
        bool flag = false;
        SaveSimulationData[] data = SavingManager.LoadAllSimData();
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i].savename == name)
            {
                flag = true;
                break;
            }
        }

        return flag;
    }

    public void Show()
    {
        popup.gameObject.SetActive(true);
    }

    public void Hide()
    {
        popup.gameObject.SetActive(false);
    }

    public void Quit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
