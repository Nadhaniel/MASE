using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Brain
{
    private LinkedList<Node>[] network = new LinkedList<Node>[3];
    //private object[][] network = new object[3][]; // 0 = input, 1 = hidden nodes, 2 = output nodes
    private int synapseNum = 0;
    private int nodeNum = 3;

    public LinkedList<Node>[] Network
    {
        get { return network; }
        set { network = value; }
    }

    public int NodeNum
    {
        get { return nodeNum; }
        set { nodeNum = value; }
    }

    public void InitNetwork(LinkedList<Node> input, Brain brain)
    {
        network[0] = input; // input layer
        network[1] = new LinkedList<Node>(); // hidden layer
        network[2] = new LinkedList<Node>(); // output layer

        network[2].AddLast(new Node("OutputNode", 0f));
        network[2].AddLast(new Node("OutputNode", 0f));
    }

    public void MutateBrain()
    {
        //var mutationType = (MutateBrainTypes)Random.Range(0, 2);
        var mutationType = MutateBrainTypes.RSN;
        if (mutationType == MutateBrainTypes.ADN)
        {
            var nodeFunctionType = (NodeTypes)Random.Range(0, 8);
            Node newNode = new Node(nodeFunctionType.ToString(), 0f);
            int sourceInputorHiddenLayer = Random.Range(0, 2); // May be problem here with random.range
            if (network[1].Count == 0 && sourceInputorHiddenLayer == 1)
            {
                sourceInputorHiddenLayer = 0;
            }
            int receivingHiddenOutputLayer = Random.Range(1, 3); // may be problem here with random.range
            if (network[1].Count == 0 && receivingHiddenOutputLayer == 1)
            {
                receivingHiddenOutputLayer = 2;
            }

            if (sourceInputorHiddenLayer == 0)
            {
                if (receivingHiddenOutputLayer == 2)
                {
                    Node randInputNode = PickRandom(network[0]);
                    Node randOutputNode = PickRandom(network[2]);

                    Synapse newSynapse1 = new Synapse(network[0].Find(randInputNode).Value, newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(newNode, network[2].Find(randInputNode).Value, 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.Add(newSynapse1);
                    network[0].Find(randInputNode).Value.SendingSynapses.Add(newSynapse1);
                    newNode.SendingSynapses.Add(newSynapse2);
                    network[2].Find(randInputNode).Value.ReceivingSynapses.Add(newSynapse2);
                    network[1].AddLast(newNode);
                }
                else if (network[1].Count > 0)
                {
                    Node randInputNode = PickRandom(network[0]);
                    Node randHiddenNode = PickRandom(network[1]);

                    Synapse newSynapse1 = new Synapse(network[0][randInputNodeIndex], newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(newNode, network[1][randHiddenNodeIndex], 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.Add(newSynapse1);
                    network[0][randInputNodeIndex].SendingSynapses.Add(newSynapse1);
                    newNode.SendingSynapses.Add(newSynapse2);
                    network[1][randHiddenNodeIndex].ReceivingSynapses.Add(newSynapse2);
                    network[1].Add(newNode);
                }
            }
            else
            {
                if (receivingHiddenOutputLayer == 2)
                {
                    int randHiddenNodeIndex = Random.Range(0, network[1].Count - 1);
                    int randOutputNode = Random.Range(0, network[2].Count - 1);

                    Synapse newSynapse1 = new Synapse(network[1][randHiddenNodeIndex], newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(newNode, network[2][randOutputNode], 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.Add(newSynapse1);
                    network[1][randHiddenNodeIndex].SendingSynapses.Add(newSynapse1);
                    newNode.SendingSynapses.Add(newSynapse2);
                    network[2][randOutputNode].ReceivingSynapses.Add(newSynapse2);
                    network[1].Add(newNode);
                }
                else
                {
                    int randHiddenNodeIndex = Random.Range(0, network[0].Count - 1);
                    int secondrandHiddenNodeindex = Random.Range(0, network[1].Count - 1);

                    if (randHiddenNodeIndex == secondrandHiddenNodeindex)
                    {
                        secondrandHiddenNodeindex += 1;
                    }

                    Synapse newSynapse1 = new Synapse(network[1][randHiddenNodeIndex], newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(newNode, network[1][secondrandHiddenNodeindex], 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.Add(newSynapse1);
                    network[1][randHiddenNodeIndex].SendingSynapses.Add(newSynapse1);
                    newNode.SendingSynapses.Add(newSynapse2);
                    network[1][secondrandHiddenNodeindex].ReceivingSynapses.Add(newSynapse2);
                    network[1].Add(newNode);
                }
            }

            nodeNum += 1;
        }
        else if (mutationType == MutateBrainTypes.ADS)
        {
            int sourceInputorHiddenLayer = Random.Range(0, 2); // May be problem here with random.range
            if (network[1].Count == 0 && sourceInputorHiddenLayer == 1)
            {
                sourceInputorHiddenLayer = 0;
            }
            int receivingHiddenOutputLayer = Random.Range(1, 3); // may be problem here with random.range
            if (network[1].Count == 0 && receivingHiddenOutputLayer == 1)
            {
                receivingHiddenOutputLayer = 2;
            }

            if (sourceInputorHiddenLayer == 0)
            {
                if (receivingHiddenOutputLayer == 2)
                {
                    int randInputNodeIndex = Random.Range(0, network[0].Count - 1);
                    int randOutputNodeIndex = Random.Range(0, network[2].Count - 1);

                    Synapse newSynapse = new Synapse(network[0][randInputNodeIndex], network[2][randOutputNodeIndex], 1);
                    network[0][randInputNodeIndex].SendingSynapses.Add(newSynapse);
                    network[2][randOutputNodeIndex].ReceivingSynapses.Add(newSynapse);
                    synapseNum += 1;
                }
                else if (network[1].Count > 0)
                {
                    int randInputNodeIndex = Random.Range(0, network[0].Count - 1);
                    int randHiddenNodeIndex = Random.Range(0, network[1].Count - 1);

                    Synapse newSynapse = new Synapse(network[0][randInputNodeIndex], network[1][randHiddenNodeIndex], 1);
                    network[0][randInputNodeIndex].SendingSynapses.Add(newSynapse);
                    network[1][randHiddenNodeIndex].ReceivingSynapses.Add(newSynapse);
                    synapseNum += 1;
                }
            }
            else
            {
                if (receivingHiddenOutputLayer == 2)
                {
                    int randHiddenNodeIndex = Random.Range(0, network[1].Count - 1);
                    int randOutputNode = Random.Range(0, network[2].Count - 1);

                    Synapse newSynapse = new Synapse(network[1][randHiddenNodeIndex], network[1][randOutputNode], 1);
                    network[1][randHiddenNodeIndex].SendingSynapses.Add(newSynapse);
                    network[2][randOutputNode].ReceivingSynapses.Add(newSynapse);
                    synapseNum += 1;
                }
                else
                {
                    int randHiddenNodeIndex = Random.Range(0, network[0].Count - 1);
                    int secondrandHiddenNodeindex = Random.Range(0, network[1].Count - 1);

                    if (randHiddenNodeIndex == secondrandHiddenNodeindex)
                    {
                        secondrandHiddenNodeindex += 1;
                    }

                    Synapse newSynapse = new Synapse(network[1][randHiddenNodeIndex], network[1][secondrandHiddenNodeindex], 1);
                    network[1][randHiddenNodeIndex].SendingSynapses.Add(newSynapse);
                    network[2][secondrandHiddenNodeindex].ReceivingSynapses.Add(newSynapse);
                    synapseNum += 1;
                }
            }
        }
        else if (mutationType == MutateBrainTypes.RSN && network[1].Count > 0)
        {
            if (Random.Range(1, 2) == 1) // Delete Node if random == 1
            {
                int randomNode = Random.Range(0, network[1].Count - 1);
                for (int i = 0; i < network[1][randomNode].ReceivingSynapses.Count; i++)
                {

                }
            }
            else
            {

            }
        }
    }

    public static Node PickRandom(LinkedList<Node> list) //Reservoir Sampling Algorithm to pick random nodes due to linkedlist being inefficiant at index searching
    {
        var node = list.First;
        var n = 1;
        Node result = default(Node);
        while (node != null)
        {
            if (Random.Range(0, n) == 0)
            {
                result = node.Value;
            }
            node = node.Next;
            n++;
        }
        return result;
    }
}

public class Node
{
    private List<Synapse> receivingSynapses = new List<Synapse>();
    private List<Synapse> sendingSynapses = new List<Synapse>();
    private float nodeValue = 0;

    private string nodeType;

    public Node(string nodetype, float value)
    {
        this.nodeType = nodetype;
        this.nodeValue = value;
    }

    public List<Synapse> ReceivingSynapses
    {
        get { return receivingSynapses; }
        set { receivingSynapses = value; }
    }

    public List<Synapse> SendingSynapses
    {
        get { return sendingSynapses; }
        set { sendingSynapses = value; }
    }

    public void NodeDisconnect()
    {
        for (int i = 0; i < receivingSynapses.Count; i++)
        {

        }
    }
}

public class Synapse
{
    private Node sourceNode;
    private Node receivingNode;
    private float value;


    public Synapse(Node Source, Node Receiving, float value)
    {
        this.sourceNode = Source;
        this.receivingNode = Receiving;
        this.value = value;
    }
}

enum MutateBrainTypes
{
    ADS, //ADS - Add synapse, CHS - Change synapse, DIS - Disable Synapse, ADN - add a hidden Node/Neuron, CHN - Change Neuron based on function, RSN - Remove Synapse/Node
    //CHS,
    //DIS,
    ADN,
    //CHN,
    RSN
}
public enum NodeTypes
{
    SIG, //SIG - Sigmoid, LIN - Linear, SQR - Square, SIN - Sinus, ABS - Absolute, REL - Reluctant, GAU - Gaussian, LAT - Latch 
    LIN,
    SQR,
    SIN,
    ABS,
    REL,
    GAU,
    LAT
}