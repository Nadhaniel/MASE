using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoGetter : MonoBehaviour
{
    public static InfoGetter instance;
    public GameObject creaturePanel;
    public GameObject selectedCreature;
    public GameObject BrainView;
    [SerializeField] GameObject Name;
    [SerializeField] GameObject Health;
    [SerializeField] GameObject Speed;
    [SerializeField] GameObject Hunger;
    [SerializeField] GameObject Maturity;
    [SerializeField] GameObject Time_Alive;
    [SerializeField] GameObject Energy;
    public bool isSelected;

    private void Start()
    {
        instance = this;
        isSelected = false;
        HideRevealPanel();
    }

    private void Update()
    {
        if (selectedCreature != null)
        {
            InfoUpdate();
            HideRevealPanel();
        }
    }

    public void HideRevealPanel()
    {
        if (isSelected == false)
        {
            creaturePanel.SetActive(false);
        }
        else if (isSelected == true)
        {
            creaturePanel.SetActive(true);
        }
    }

    public void InfoUpdate()
    {
        Name.GetComponent<TMPro.TextMeshProUGUI>().text = "Name: " + selectedCreature.GetComponent<Creature>().name;
        Health.GetComponent<TMPro.TextMeshProUGUI>().text = "Health: " + selectedCreature.GetComponent<Creature>().health;
        Speed.GetComponent<TMPro.TextMeshProUGUI>().text = "Speed: " + selectedCreature.GetComponent<Creature>().speed;
        Hunger.GetComponent<TMPro.TextMeshProUGUI>().text = "Hunger: " + selectedCreature.GetComponent<Creature>().hunger;
        Maturity.GetComponent<TMPro.TextMeshProUGUI>().text = "Maturity: " + selectedCreature.GetComponent<Creature>().maturity;
        Time_Alive.GetComponent<TMPro.TextMeshProUGUI>().text = "Time Alive: " + selectedCreature.GetComponent<Creature>().Time_Alive;
        Energy.GetComponent<TMPro.TextMeshProUGUI>().text = "Energy: " + selectedCreature.GetComponent<Creature>().energy;
    }
}
