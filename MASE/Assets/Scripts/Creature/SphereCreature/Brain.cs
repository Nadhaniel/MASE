using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Brain
{
    private LinkedList<Node> network = new LinkedList<Node>();
    private int synapseNum = 0;
    private int inputNodes;
    private int hiddenNodes;
    private int outputNodes;

    public LinkedList<Node> Network
    {
        get { return network; }
        set { network = value; }
    }

    public int InputNodesCount
    {
        get 
        {
            int count = 0;
            foreach (var node in network)
            {
                if (node.NodeType == NodeTypes.Input)
                {
                    count++;
                }
            }
            inputNodes = count;
            return inputNodes;
        }
    }

    public int HiddenNodesCount
    {
        get
        {
            int count = 0;
            foreach (var node in network)
            {
                if (node.NodeType == NodeTypes.Hidden)
                {
                    count++;
                }
            }
            hiddenNodes = count;
            return hiddenNodes;
        }
    }

    public int OutputNodesCount
    {
        get
        {
            int count = 0;
            foreach (var node in network)
            {
                if (node.NodeType == NodeTypes.Output)
                {
                    count++;
                }
            }
            outputNodes = count;
            return inputNodes;
        }
    }

    public Brain(LinkedList<Node> input, Brain brain)
    {
        network.Concat(input); // input layer

        network.AddLast(new Node(NodeTypes.Output, 0f));
        network.AddLast(new Node(NodeTypes.Output, 0f));
        inputNodes = input.Count;
        hiddenNodes = 0;
        outputNodes = 2;
    }

    public void MutateBrain()
    {
        //var mutationType = (MutateBrainTypes)Random.Range(0, 2);
        var mutationType = MutateBrainTypes.RSN;
        if (mutationType == MutateBrainTypes.ADN)
        {
            var nodeFunctionType = (NodeTypes)Random.Range(0, 8);
            Node newNode = new Node(NodeTypes.Hidden, 0f);
            newNode.HiddenNodeType = nodeFunctionType;
            int sourceInputorHiddenLayer = Random.Range(0, 2); // May be problem here with random.range
            if (hiddenNodes == 0 && sourceInputorHiddenLayer == 1)
            {
                sourceInputorHiddenLayer = 0;
            }
            int receivingHiddenOutputLayer = Random.Range(1, 3); // may be problem here with random.range
            if (hiddenNodes == 0 && receivingHiddenOutputLayer == 1)
            {
                receivingHiddenOutputLayer = 2;
            }

            if (sourceInputorHiddenLayer == 0)
            {
                if (receivingHiddenOutputLayer == 2) //adding a node between a input node and a output node
                {
                    Node randInputNode = PickRandom(network, NodeTypes.Input);
                    Node randOutputNode = PickRandom(network, NodeTypes.Output);

                    Synapse newSynapse1 = new Synapse(network.Find(randInputNode).Value, newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(newNode, network.Find(randOutputNode).Value, 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.AddLast(newSynapse1);
                    network.Find(randInputNode).Value.SendingSynapses.AddLast(newSynapse1);
                    newNode.SendingSynapses.AddLast(newSynapse2);
                    network.Find(randOutputNode).Value.ReceivingSynapses.AddLast(newSynapse2);
                    network.AddLast(newNode);
                }
                else if (hiddenNodes > 0) //adding a node between a input node and a hidden node
                {
                    Node randInputNode = PickRandom(network, NodeTypes.Input);
                    Node randHiddenNode = PickRandom(network, NodeTypes.Output);

                    Synapse newSynapse1 = new Synapse(network.Find(randInputNode).Value, newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(newNode, network.Find(randHiddenNode).Value, 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.AddLast(newSynapse1);
                    network.Find(randInputNode).Value.SendingSynapses.AddLast(newSynapse1);
                    newNode.SendingSynapses.AddLast(newSynapse2);
                    network.Find(randHiddenNode).Value.ReceivingSynapses.AddLast(newSynapse2);
                    network.AddLast(newNode);
                }
            }
            else
            {
                if (receivingHiddenOutputLayer == 2) //adding a node between a hidden node and output node
                {
                    Node randHiddenNode = PickRandom(network, NodeTypes.Hidden);
                    Node randOutputNode = PickRandom(network, NodeTypes.Output);

                    Synapse newSynapse1 = new Synapse(network.Find(randHiddenNode).Value, newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(newNode, network.Find(randOutputNode).Value, 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.AddLast(newSynapse1);
                    network.Find(randHiddenNode).Value.ReceivingSynapses.AddLast(newSynapse1);
                    newNode.SendingSynapses.AddLast(newSynapse2);
                    network.Find(randOutputNode).Value.ReceivingSynapses.AddLast(newSynapse2);
                    network.AddLast(newNode);
                }
                else
                {
                    Node randHiddenNode = PickRandom(network, NodeTypes.Hidden);
                    Node secondrandHiddenNode = PickRandom(network, NodeTypes.Hidden);

                    while (randHiddenNode == secondrandHiddenNode)
                    {
                        secondrandHiddenNode = PickRandom(network, NodeTypes.Hidden); //Could become very inefficient due to luck 
                    }

                    Synapse newSynapse1 = new Synapse(network.Find(randHiddenNode).Value, newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(network.Find(secondrandHiddenNode).Value, newNode, 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.AddLast(newSynapse1);
                    network.Find(randHiddenNode).Value.SendingSynapses.AddLast(newSynapse1);
                    newNode.SendingSynapses.AddLast(newSynapse2);
                    network.Find(secondrandHiddenNode).Value.ReceivingSynapses.AddLast(newSynapse2);
                    network.AddLast(newNode);
                }
            }
            Debug.Log(HiddenNodesCount.ToString());
        }
        else if (mutationType == MutateBrainTypes.ADS)
        {
            int sourceInputorHiddenLayer = Random.Range(0, 2); // May be problem here with random.range
            if (hiddenNodes == 0 && sourceInputorHiddenLayer == 1)
            {
                sourceInputorHiddenLayer = 0;
            }
            int receivingHiddenOutputLayer = Random.Range(1, 3); // may be problem here with random.range
            if (hiddenNodes == 0 && receivingHiddenOutputLayer == 1)
            {
                receivingHiddenOutputLayer = 2;
            }

            if (sourceInputorHiddenLayer == 0)
            {
                if (receivingHiddenOutputLayer == 2)
                {
                    Node randInputNode = PickRandom(network, NodeTypes.Input);
                    Node randOutputNode = PickRandom(network, NodeTypes.Output);

                    Synapse newSynapse = new Synapse(randInputNode, randOutputNode, 1);
                    network.Find(randInputNode).Value.SendingSynapses.AddLast(newSynapse);
                    network.Find(randOutputNode).Value.ReceivingSynapses.AddLast(newSynapse);
                    synapseNum += 1;
                }
                else if (hiddenNodes > 0)
                {
                    Node randInputNode = PickRandom(network, NodeTypes.Input);
                    Node randHiddenNode = PickRandom(network, NodeTypes.Hidden);

                    Synapse newSynapse = new Synapse(randInputNode, randHiddenNode, 1);
                    network.Find(randInputNode).Value.SendingSynapses.AddLast(newSynapse);
                    network.Find(randHiddenNode).Value.ReceivingSynapses.AddLast(newSynapse);
                    synapseNum += 1;
                }
            }
            else
            {
                if (receivingHiddenOutputLayer == 2)
                {
                    Node randHiddenNode = PickRandom(network, NodeTypes.Hidden);
                    Node randOutputNode = PickRandom(network, NodeTypes.Output);

                    Synapse newSynapse = new Synapse(randHiddenNode, randOutputNode, 1);
                    network.Find(randHiddenNode).Value.SendingSynapses.AddLast(newSynapse);
                    network.Find(randOutputNode).Value.ReceivingSynapses.AddLast(newSynapse);
                    synapseNum += 1;
                }
                else
                {
                    Node randHiddenNode = PickRandom(network, NodeTypes.Hidden);
                    Node randHiddenNode2 = PickRandom(network, NodeTypes.Hidden);

                    while (randHiddenNode == randHiddenNode2)
                    {
                        randHiddenNode = PickRandom(network, NodeTypes.Hidden); //Could become very inefficient due to luck 
                    }

                    Synapse newSynapse = new Synapse(randHiddenNode, randHiddenNode2, 1);
                    network.Find(randHiddenNode).Value.SendingSynapses.AddLast(newSynapse);
                    network.Find(randHiddenNode2).Value.ReceivingSynapses.AddLast(newSynapse);
                    synapseNum += 1;
                }
            }
        }
        else if (mutationType == MutateBrainTypes.RSN && hiddenNodes > 0)
        {
            if (Random.Range(1, 2) == 1) // Delete Node if random == 1
            {
                Node randomNode = PickRandom(network, NodeTypes.Hidden);
                foreach (var synapse in randomNode.ReceivingSynapses)
                {
                    network.Find(synapse.ReceivingNode).Value.SendingSynapses.Remove(synapse);
                }
            }
            else
            {

            }
        }
    }

    public static Node PickRandom(LinkedList<Node> list, NodeTypes type) //Reservoir Sampling Algorithm to pick random nodes due to linkedlist being inefficiant at index searching
    {
        LinkedList<Node> typelist = new LinkedList<Node>();
        foreach (var node in list)
        {
            if (node.NodeType == type)
            {
                typelist.AddLast(node);
            }
        }
        var n = 1;
        Node result = default(Node);
        var nodefirst = typelist.First;
        while (nodefirst != null)
        {
            if (Random.Range(0, n) == 0)
            {
                result = nodefirst.Value;
            }
            nodefirst = nodefirst.Next;
            n++;
        }
        return result;
    }
}

public class Node
{
    private LinkedList<Synapse> receivingSynapses = new LinkedList<Synapse>();
    private LinkedList<Synapse> sendingSynapses = new LinkedList<Synapse>();
    private float nodeValue = 0;

    private NodeTypes nodeType;
    private NodeTypes hiddenNodeType;

    public Node(NodeTypes nodetype, float value)
    {
        this.nodeType = nodetype;
        this.nodeValue = value;
    }

    public LinkedList<Synapse> ReceivingSynapses
    {
        get { return receivingSynapses; }
        set { receivingSynapses = value; }
    }

    public LinkedList<Synapse> SendingSynapses
    {
        get { return sendingSynapses; }
        set { sendingSynapses = value; }
    }

    public NodeTypes NodeType
    {
        get { return nodeType; }
        set { nodeType = value; }
    }

    public NodeTypes HiddenNodeType
    {
        get { return hiddenNodeType; }
        set { hiddenNodeType = value; }
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

    public Node SourceNode
    {
        get { return sourceNode; }
    }

    public Node ReceivingNode
    {
        get { return receivingNode; }
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
    LAT,
    Input,
    Output,
    Hidden
}