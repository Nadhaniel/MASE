using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    private int MaxPopulation;
    public GameObject creature;
    //List<GameObject> AgentPopulation = new List<GameObject>();
    public static float elapsedtime = 0;
    public float timeScale = 3;
    int generation = 0;
    public float trialTime = 30;

    //New Generation system

    private float minHeight;
    private float maxHeight;

    private void Start()
    {
        MaxPopulation = 600;
        Generate();
        generation += 1;
        Time.timeScale = timeScale;
    }

    private void Update()
    {
        GameObject[] creatures = GameObject.FindGameObjectsWithTag("Creature");
        if (creatures.Length <= 50)
        {
            MaxPopulation = 300;
            Generate();
        }
        MaxPopControl();
    }

    public void Generate()
    {
        //Clear();
        List<GameObject> AgentPopulation = new List<GameObject>();
        Vector2 xRange = new Vector2(-1000, 1000);
        Vector2 zRange = new Vector2(-1000, 1000);
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
                    AgentPopulation.Add(Instantiate(creature, hit.point, Quaternion.identity));
                    AgentPopulation[i].gameObject.name = "Creature " + i;
                    i++;
                }
            }
        }
        Debug.Log(AgentPopulation.Count);
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



}
