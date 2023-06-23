using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreaturePool : MonoBehaviour
{
    public static CreaturePool instance;

    private List<GameObject> pooledobjects = new List<GameObject>();
    private int maxpool = 600;
    [SerializeField] private GameObject CreaturePrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        for (int i = 0; i < maxpool; i++)
        {
            GameObject creature = Instantiate(CreaturePrefab);
            creature.SetActive(false);
            pooledobjects.Add(creature);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledobjects.Count; i++)
        {
            if (!pooledobjects[i].activeInHierarchy)
            {
                return pooledobjects[i];
            }
        }

        return null;
    }
}
