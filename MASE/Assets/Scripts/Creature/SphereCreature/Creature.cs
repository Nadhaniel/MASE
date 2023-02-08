using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Creature : MonoBehaviour
{
    private Brain brain;
    List<Node> InputNodes = new List<Node>();

    private void Start()
    {
        brain = new Brain();
        InputNodes.Add(new Node("InputNode", 1.0f));
        InputNodes.Add(new Node("InputNode", 2.0f));
        InputNodes.Add(new Node("InputNode", 0.5f));
        brain.InitNetwork(InputNodes, brain);
        brain.MutateBrain();
    }
}
