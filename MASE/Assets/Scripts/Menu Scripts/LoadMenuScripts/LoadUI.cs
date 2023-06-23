using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadUI : MonoBehaviour
{
    public GameObject Save_Content;
    public GameObject Save;
    public GameObject[] Saves;
    void Start()
    {
        SaveSimulationData[] data = SavingManager.LoadAllSimData();
        Saves = new GameObject[data.Length];
        if (data.Length > 0)
        {
            for (int i = 0; i < data.Length; i++)
            {
                DateTime timefromjson = JsonUtility.FromJson<JsonDateTime>(data[i].dateTime);
                Saves[i] = Instantiate(Save, Save_Content.transform);
                Saves[i].transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = data[i].savenumber + ": " + data[i].savename + "-" + timefromjson;
                Saves[i].GetComponent<SaveRef>().saveData = data[i];
            }
        }
    }

    public void Quit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 3);
    }
}
