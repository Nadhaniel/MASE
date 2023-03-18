using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager instance;
    private int MaxPopulation;
    public GameObject creature;
    public List<GameObject> creatures;
    public static float elapsedtime = 0;
    public float timeScale = 3;
    int generation = 0;
    public float trialTime = 30;

    private float minHeight;
    private float maxHeight;

    private void Start()
    {
        instance = this;
        creatures = GameObject.FindGameObjectsWithTag("Creature").ToList();
        MaxPopulation = 600;
        Generate();
        generation += 1;
        Time.timeScale = timeScale;
    }

    private void Update()
    {
        creatures = GameObject.FindGameObjectsWithTag("Creature").ToList();
        if (creatures.Count <= 50)
        {
            MaxPopulation = 100;
            Generate();
        }
        MaxPopControl();
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
        while (i < MaxPopulation)
        {
            float sampleX = Random.Range(xRange.x, xRange.y);
            float sampleY = Random.Range(zRange.x, zRange.y);
            Vector3 rayStart = new Vector3(sampleX, maxHeight, sampleY);
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.point.y > minHeight)
                {
                    var newcreature = Instantiate(creature, hit.point, Quaternion.identity);
                    newcreature.name = "Creature: " + (popCount + 1);
                    popCount += 1;
                    i++;
                }
            }
        }
        Debug.Log(creatures.Count);
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
        Time.timeScale = 3;
    }

}
