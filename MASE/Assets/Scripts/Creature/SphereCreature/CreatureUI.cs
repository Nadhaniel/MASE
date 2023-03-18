using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureUI : MonoBehaviour
{
    private bool isSelected;
    public GameObject panel;
    private void Start()
    {
        panel = GameObject.Find("CreatureInfo");
        isSelected = false;
    }
    private void Update()
    {
        HideRevealPanel();
    }

    public void HideRevealPanel()
    {
        if (isSelected == false)
        {
            panel.SetActive(false);
        }
        else if (isSelected == true)
        {
            panel.SetActive(true);
        }
    }

    private void OnMouseDown()
    {
        isSelected = true;
    }
}
