using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Creature : MonoBehaviour
{
    private Brain brain;
    private Transform creaturetrans;
    List<Node> InputNodes = new List<Node>();

    private void Start()
    {
        creaturetrans = this.transform;
    }
}
