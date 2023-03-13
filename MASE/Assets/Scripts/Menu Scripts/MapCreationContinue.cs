using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapCreationContinue : MonoBehaviour
{
    public GameObject MapGen;
    public void MoveForward()
    {
        SaveData.current.noisedata = MapGen.GetComponent<MapGenerator>().Noisedata;
        SaveData.current.terrainData = MapGen.GetComponent<MapGenerator>().Terraindata;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
