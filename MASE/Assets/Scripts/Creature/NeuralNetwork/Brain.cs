using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class Brain
{
    //private List<List<Node>> network;
    public NNGenome NN_genome;
    public List<Node> nodes;
    public List<Node> inputNodes;
    public List<Node> hiddenNodes;
    public List<Node> outputNodes;
    public List<Synapse> synapses;
    public Guid id;
    public float fitness;
    public float adjFitness;
    public int species_num = 0;
    public Brain(int num_input, int num_out, int num_hidden)
    {
        id = Guid.NewGuid();
        NN_genome = Init_Genome(num_input, num_out, num_hidden);
        nodes = new List<Node>();
        inputNodes = new List<Node>();
        outputNodes = new List<Node>();
        hiddenNodes = new List<Node>();
        synapses = new List<Synapse>();
        createNetwork();
    }
    public Brain(NNGenome genome)
    {
        id = Guid.NewGuid();
        NN_genome = genome;
        nodes = new List<Node>();
        inputNodes = new List<Node>();
        outputNodes = new List<Node>();
        hiddenNodes = new List<Node>();
        synapses = new List<Synapse>();
        createNetwork();
    }

    private NNGenome Init_Genome(int num_input, int num_out, int num_hidden)
    {
        List<NodeGene> initNodegenes = new List<NodeGene>();
        List<ConnectionGene> initConnections = new List<ConnectionGene>();
        int nodeIndex = 0;

        for (int i = 0; i < num_input; i++)
        {
            NodeGene nodeGene = new NodeGene(nodeIndex, Node_Type.Input);
            initNodegenes.Add(nodeGene);
            nodeIndex += 1;
        }

        for (int i = 0; i < num_out; i++)
        {
            NodeGene nodeGene = new NodeGene(nodeIndex, Node_Type.Output);
            initNodegenes.Add(nodeGene);
            nodeIndex += 1;
        }

        for (int i = 0; i < num_hidden; i++)
        {
            NodeGene nodeGene = new NodeGene(nodeIndex, Node_Type.Hidden);
            initNodegenes.Add(nodeGene);
            nodeIndex += 1; 

        }

        NNGenome initGenome = new NNGenome(initNodegenes, initConnections);
        return initGenome;
    }

    public void createNetwork()
    {
        clearNetwork();
        //Creates the nodes
        foreach (NodeGene nodegene in NN_genome.nodeGenes)
        {
            string nodeName = nodegene.type.ToString() + " " + nodegene.id.ToString();
            Node node = new Node(nodegene.type,  nodeName, nodegene.id);
            nodes.Add(node);
            if (nodegene.type == Node_Type.Input)
            {
                inputNodes.Add(node);
            }
            else if (nodegene.type == Node_Type.Hidden)
            {
                hiddenNodes.Add(node);
            }
            else if (nodegene.type == Node_Type.Output)
            {
                outputNodes.Add(node);
            }
        }
        //Creates the connections
        foreach (ConnectionGene connectiongene in NN_genome.connectionGenes)
        {
            Synapse synapse = new Synapse(connectiongene.SourceNode, connectiongene.ReceivingNode, connectiongene.weight, connectiongene.disabled);
            synapses.Add(synapse);
        }
        //Sets up node synapses
        foreach (Node node in nodes)
        {
            foreach (Synapse synapse in synapses)
            {
                if (synapse.SourceNode == node.Index)
                {
                    node.SendingSynapses.Add(synapse);
                }
                else if (synapse.ReceivingNode == node.Index)
                {
                    node.ReceivingSynapses.Add(synapse);
                }
            }
        }
    }

    private void clearNetwork()
    {
        nodes.Clear();
        inputNodes.Clear();
        outputNodes.Clear();
        hiddenNodes.Clear();
        synapses.Clear();
    }

    //Mutation related code
    public void MutateNN(int number_of_mutations)
    {
        for (int i = 0; i < number_of_mutations; i++)
        {
            NN_genome.MutateGenome();
        }
        createNetwork();
    }
    //feedforward algorithm
    public float[] BrainTick_FF(float[] input_values) //Feedforward algorithm
    {
        float[] outputs = new float[outputNodes.Count];
        for (int i = 0; i < inputNodes.Count; i++)
        {
            inputNodes[i].SetInputVal(input_values[i]);
            inputNodes[i].FeedForwardVal();
            inputNodes[i].NodeValue = 0;
        }
        for (int i = 0; i < hiddenNodes.Count; i++)
        {
            hiddenNodes[i].setHiddenVal();
            hiddenNodes[i].FeedForwardVal();
            hiddenNodes[i].NodeValue = 0;
        }
        for (int i = 0; i < outputNodes.Count; i++)
        {
            outputNodes[i].setOutputVal();
            outputs[i] = outputNodes[i].NodeValue;
            outputNodes[i].NodeValue = 0;
        }

        return outputs;
    }

    //Random pickers
    public Node PickRandomNode(List<Node> list)
    {
        int randomNodeIndex = 0;
        randomNodeIndex = Random.Range(0, list.Count);
        return list[randomNodeIndex];
    }

    public static Synapse PickRandomSynapse(List<Synapse> list) //Reservoir Sampling Algorithm to pick random synapse due to linkedlist being inefficiant at index searching
    {
        int randomIndex = 0;
        randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }
}

enum MutateBrainTypes
{
    ADN,
    ADS,//ADS - Add synapse, CHS - Change synapse, FLP - flips the synapse active bool, ADN - add a hidden Node/Neuron, CHN - Change Neuron function, RMN - Remove Node, RMS - Remove Synapse
    CHS,
    FLP,
}
public enum Node_Activations
{
    SIG, //SIG - Sigmoid 
    TanH,
    TanHMod1
}