using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public Brain brain; 
    //States
    private float hunger;
    private float health;
    private float speed;
    //Vision
    private float dist_creature;
    private float angle_crature;
    private float dist_food;
    private float angle_food;
    //Clock
    private float Time_Alive;
    
    private Transform creaturetrans;
    LinkedList<Node> InputNodes = new LinkedList<Node>();
    LinkedList<Node> OutputNodes = new LinkedList<Node>();

    private void Start()
    {
        creaturetrans = this.transform;
        InputNodes.AddLast(new Node(NodeTypes.Input, hunger));
        InputNodes.AddLast(new Node(NodeTypes.Input, health));
        InputNodes.AddLast(new Node(NodeTypes.Input, speed));
        InputNodes.AddLast(new Node(NodeTypes.Input, dist_creature));
        InputNodes.AddLast(new Node(NodeTypes.Input, angle_crature));
        InputNodes.AddLast(new Node(NodeTypes.Input, dist_food));
        InputNodes.AddLast(new Node(NodeTypes.Input, angle_food));
        InputNodes.AddLast(new Node(NodeTypes.Input, Time_Alive));

        OutputNodes.AddLast(new Node(NodeTypes.Output, 0)); //Move forward
        OutputNodes.AddLast(new Node(NodeTypes.Output, 0)); //Move left
        OutputNodes.AddLast(new Node(NodeTypes.Output, 0)); //Move right
        OutputNodes.AddLast(new Node(NodeTypes.Output, 0)); //Move backward
        OutputNodes.AddLast(new Node(NodeTypes.Output, 0)); //Want to eat

        brain = new Brain(InputNodes, OutputNodes);
        brain.MutateBrain();
    }

    private void Update()
    {
        if (Physics.Raycast(creaturetrans.position, creaturetrans.TransformDirection(Vector3.forward), out RaycastHit hit, 20f))
        {
            //Debug.Log("Hit something");
            Debug.DrawRay(creaturetrans.position, creaturetrans.TransformDirection(Vector3.forward) * 20f, Color.red);
        }
        else 
        {
            //Debug.Log("Hit Nothing");
            Debug.DrawRay(creaturetrans.position, creaturetrans.TransformDirection(Vector3.forward) * 20f, Color.black);
        }
        brain.BrainTick();
        var test1 = brain.Network[0].NodeValue;
    }

    //Movement Methods
    public void MoveForward(float speedMultiplier)
    {
        Vector3 pos = this.transform.position;
        pos.x += 0.1f * speedMultiplier;
        this.transform.position = pos;
    }

    public void MoveBackward(float speedMultiplier)
    {
        Vector3 pos = this.transform.position;
        pos.x += -(0.1f * speedMultiplier);
        this.transform.position = pos;
    }

    public void MoveLeft(float speedMultiplier)
    {
        Vector3 pos = this.transform.position;
        pos.z += 0.1f * speedMultiplier;
        this.transform.position = pos;
    }

    public void MoveRight(float speedMultiplier)
    {
        Vector3 pos = this.transform.position;
        pos.z += -(0.1f * speedMultiplier);
        this.transform.position = pos;
    }
}
