using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public GameObject agent;
    public int SpawnAmount;
    public void Spawn()
    {
        for (int i = 0; i < SpawnAmount; i++)
        {
            Instantiate(agent, this.transform.position, this.transform.rotation);
        }
    }
}
