using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//Based on the NEAT algorithm 
public class NNGenome : ICloneable
{
    //properties
    public List<NodeGene> nodeGenes;
    public List<ConnectionGene> connectionGenes;
    public int species_num;

    //Initialization
    public NNGenome()
    {
        nodeGenes = new List<NodeGene>();
        connectionGenes = new List<ConnectionGene>();
    }

    public NNGenome(List<NodeGene> nodeGenes, List<ConnectionGene> connectionGenes)
    {
        this.nodeGenes = nodeGenes;
        this.connectionGenes = connectionGenes;
    }

    //Methods
    public void MutateGenome()
    {
        float connectionChance = 90f;
        float nodeChance = 10f;
        float disablechance = 20f;
        if (UnityEngine.Random.Range(0f, 100) <= nodeChance)
        {
            if (connectionGenes.Count > 0)
            {
                AddRandomNode();
            }
            else
            {
                AddRandomConnection();
            }
        }
        if (UnityEngine.Random.Range(0f, 100f) <= connectionChance)
        {
            AddRandomConnection();
        }
        if (connectionGenes.Count > 0)
        {
            if (Random.Range(0f, 100f) <= disablechance)
            {
                int index = Random.Range(0, connectionGenes.Count);
                connectionGenes[index].disabled = true;
            }
            else
            {
                int index = Random.Range(0, connectionGenes.Count);
                connectionGenes[index].disabled = false;
            }
        }
        MutateWeights();
    }

    private void AddRandomNode()
    {
        int randomconnection = Random.Range(0, connectionGenes.Count);
        ConnectionGene mutatedConnectionGene = connectionGenes[randomconnection];
        int srcNode = mutatedConnectionGene.SourceNode;
        int recNode = mutatedConnectionGene.ReceivingNode;

        mutatedConnectionGene.disabled = true;

        int newIndex = GetNextNodeIndex();

        NodeGene newNode = new NodeGene(newIndex, Node_Type.Hidden);
        nodeGenes.Add(newNode);

        int nextinnovation_num = NextInnovationNumber();
        ConnectionGene newconnection = new ConnectionGene(srcNode, newNode.id, 1f, false, nextinnovation_num);
        connectionGenes.Add(newconnection);
        nextinnovation_num = NextInnovationNumber();
        ConnectionGene secondnewconnection = new ConnectionGene(newNode.id, recNode, mutatedConnectionGene.weight, false, nextinnovation_num);
        connectionGenes.Add(secondnewconnection);
    }
    private int GetNextNodeIndex()
    {
        int nextIndex = 0;
        foreach (NodeGene node in nodeGenes)
        {
            if (nextIndex <= node.id)
            {
                nextIndex = node.id;
            }
        }
        nextIndex += 1;
        return nextIndex;
    }

    private bool AddRandomConnection()
    {
        int firstNodeIndex = Random.Range(0, nodeGenes.Count);
        int secondNodeIndex = Random.Range(0, nodeGenes.Count);
        Node_Type firsttype = nodeGenes[firstNodeIndex].type;
        Node_Type secondtype = nodeGenes[secondNodeIndex].type;

        if (firsttype == secondtype && firsttype != Node_Type.Hidden)
        {
            return AddRandomConnection();
        }

        foreach (ConnectionGene gene in connectionGenes)
        {
            if ((firstNodeIndex == gene.SourceNode && secondNodeIndex == gene.ReceivingNode) || (secondNodeIndex == gene.SourceNode && firstNodeIndex == gene.ReceivingNode))
            {
                return false;
            }
        }

        if (firsttype == Node_Type.Output || (firsttype == Node_Type.Hidden && secondtype == Node_Type.Input))
        {
            int temp = firstNodeIndex;
            firstNodeIndex = secondNodeIndex;
            secondNodeIndex = temp;

            firsttype = nodeGenes[firstNodeIndex].type;
            secondtype = nodeGenes[secondNodeIndex].type;
        }
        int innovation_num = NextInnovationNumber();
        float weight = Random.Range(-1.5f, 1.5f);
        ConnectionGene newConnGene = new ConnectionGene(nodeGenes[firstNodeIndex].id, nodeGenes[secondNodeIndex].id, weight, false, innovation_num);
        connectionGenes.Add(newConnGene);
        return true;
    }

    private int NextInnovationNumber()
    {
        int nextInnovation_num = 0;
        foreach (ConnectionGene gene in connectionGenes)
        {
            if (nextInnovation_num <= gene.innovation_no)
            {
                nextInnovation_num = gene.innovation_no;
            }
        }
        nextInnovation_num += 1;
        return nextInnovation_num;
    }
    private void MutateWeights()
    {
        float mw_chance = 5f; //representating the chance of a single weight getting mutated
        float maw_chance = 95f; //representing the chance of all the weights getting mutated
        if (Random.Range(0f, 100f) <= mw_chance)
        {
            MutateWeight();
        }
        if (Random.Range(0f, 100f) <= maw_chance)
        {
            MutateAllWeights();
        }
    }
    private void MutateWeight()
    {
        if (connectionGenes.Count != 0)
        {
            int index = Random.Range(0, connectionGenes.Count);
            ConnectionGene gene = connectionGenes[index];
            gene.weight += Random.Range(-1.5f, 1.5f);
        }
        else
        {
            Debug.Log("No connection genes or connection gene not found");
        }
    }
    private void MutateAllWeights()
    {
        foreach (ConnectionGene gene in connectionGenes)
        {
            gene.weight += (Random.Range(-0.5f, 0.5f) * 0.5f);
        }
    }

    public object Clone()
    {
        return this;
    }
}

public class NodeGene
{
    public int id;
    public Node_Type type;

    public NodeGene(int id, Node_Type type)
    {
        this.id = id;
        this.type = type;
    }
    
}

public class ConnectionGene
{
    public int SourceNode;
    public int ReceivingNode;
    public float weight;
    public bool disabled;
    public int innovation_no;

    public ConnectionGene(int srcNode, int recNode, float weight, bool disabled, int innovation_num)
    {
        this.SourceNode = srcNode;
        this.ReceivingNode = recNode;
        this.weight = weight;
        this.disabled = disabled;
        this.innovation_no = innovation_num;
    }
}

public enum Node_Type
{
    Input, Output, Hidden
}