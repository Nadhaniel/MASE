using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureUI : MonoBehaviour
{
    Creature Creature;

    private void Start()
    {
        Creature = this.GetComponent<Creature>();
    }
}
