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

    public Brain(LinkedList<Node> input, LinkedList<Node> output)
    {
        network.Concat(input); // input layer
        network.Concat(output);
        network.AddLast(new Node(NodeTypes.Output, 0f));
        network.AddLast(new Node(NodeTypes.Output, 0f));
        inputNodes = input.Count;
        outputNodes = output.Count;
        hiddenNodes = 0;
    }

    #region Mutation Algorithm
    public void MutateBrain()
    {
        var mutationType = (MutateBrainTypes)Random.Range(0, 2);
        //var mutationType = MutateBrainTypes.RSN;
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
                    Node randInputNode = PickRandomNode(network, NodeTypes.Input);
                    Node randOutputNode = PickRandomNode(network, NodeTypes.Output);

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
                    Node randInputNode = PickRandomNode(network, NodeTypes.Input);
                    Node randHiddenNode = PickRandomNode(network, NodeTypes.Output);

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
                    Node randHiddenNode = PickRandomNode(network, NodeTypes.Hidden);
                    Node randOutputNode = PickRandomNode(network, NodeTypes.Output);

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
                    Node randHiddenNode = PickRandomNode(network, NodeTypes.Hidden);
                    Node secondrandHiddenNode = PickRandomNode(network, NodeTypes.Hidden);

                    while (randHiddenNode == secondrandHiddenNode)
                    {
                        secondrandHiddenNode = PickRandomNode(network, NodeTypes.Hidden); //Could become very inefficient due to luck 
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
                    Node randInputNode = PickRandomNode(network, NodeTypes.Input);
                    Node randOutputNode = PickRandomNode(network, NodeTypes.Output);

                    Synapse newSynapse = new Synapse(randInputNode, randOutputNode, 1);
                    network.Find(randInputNode).Value.SendingSynapses.AddLast(newSynapse);
                    network.Find(randOutputNode).Value.ReceivingSynapses.AddLast(newSynapse);
                    synapseNum += 1;
                }
                else if (hiddenNodes > 0)
                {
                    Node randInputNode = PickRandomNode(network, NodeTypes.Input);
                    Node randHiddenNode = PickRandomNode(network, NodeTypes.Hidden);

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
                    Node randHiddenNode = PickRandomNode(network, NodeTypes.Hidden);
                    Node randOutputNode = PickRandomNode(network, NodeTypes.Output);

                    Synapse newSynapse = new Synapse(randHiddenNode, randOutputNode, 1);
                    network.Find(randHiddenNode).Value.SendingSynapses.AddLast(newSynapse);
                    network.Find(randOutputNode).Value.ReceivingSynapses.AddLast(newSynapse);
                    synapseNum += 1;
                }
                else
                {
                    Node randHiddenNode = PickRandomNode(network, NodeTypes.Hidden);
                    Node randHiddenNode2 = PickRandomNode(network, NodeTypes.Hidden);

                    while (randHiddenNode == randHiddenNode2)
                    {
                        randHiddenNode = PickRandomNode(network, NodeTypes.Hidden); //Could become very inefficient due to luck 
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
            if (Random.Range(1, 2) == 1) // Delete Node if random == 1 otherwise delete synapse
            {
                Node randomNode = PickRandomNode(network, NodeTypes.Hidden);
                foreach (var synapse in randomNode.ReceivingSynapses)
                {
                    network.Find(synapse.SourceNode).Value.SendingSynapses.Remove(synapse);
                }
                foreach (var synapse in randomNode.SendingSynapses)
                {
                    network.Find(synapse.ReceivingNode).Value.ReceivingSynapses.Remove(synapse);
                }
                network.Remove(randomNode);
            }
            else
            {
                Node randomNode = PickRandomNode(network, NodeTypes.Hidden);
                if (Random.Range(1, 2) == 1) // delete a sending synapse 
                {
                    Synapse randSynapse = PickRandomSynapse(network.Find(randomNode).Value.SendingSynapses);
                    network.Find(randSynapse.ReceivingNode).Value.ReceivingSynapses.Remove(randSynapse);
                    network.Find(randomNode).Value.SendingSynapses.Remove(randSynapse);
                }
                else
                {
                    Synapse randSynapse = PickRandomSynapse(network.Find(randomNode).Value.ReceivingSynapses);
                    network.Find(randSynapse.SourceNode).Value.SendingSynapses.Remove(randSynapse);
                    network.Find(randomNode).Value.ReceivingSynapses.Remove(randSynapse);
                }
                if (network.Find(randomNode).Value.ReceivingSynapses.Count == 0 || network.Find(randomNode).Value.SendingSynapses.Count == 0) //a Final if statement to remove any floating structures in the nerual network
                {
                    foreach (var synapse in network.Find(randomNode).Value.ReceivingSynapses)
                    {
                        network.Find(synapse.SourceNode).Value.SendingSynapses.Remove(synapse);
                        network.Find(randomNode).Value.ReceivingSynapses.Remove(synapse);
                        synapseNum -= 1;
                    }
                    foreach (var synapse in network.Find(randomNode).Value.SendingSynapses)
                    {
                        network.Find(synapse.ReceivingNode).Value.ReceivingSynapses.Remove(synapse);
                        network.Find(randomNode).Value.SendingSynapses.Remove(synapse);
                        synapseNum -= 1;
                    }
                    network.Remove(randomNode);
                    hiddenNodes -= 1;
                }

                synapseNum -= 1;
            }
        }
        else if (mutationType == MutateBrainTypes.CHS && synapseNum > 0)
        {
            if (Random.Range(1, 2) == 1) //Mutate a synapse coming from a input node
            {
                Node randomNode = PickRandomNode(network, NodeTypes.Input);
                if (randomNode.SendingSynapses.Count > 0)
                {
                    Synapse randomSyn = PickRandomSynapse(network.Find(randomNode).Value.SendingSynapses);
                    float val = randomSyn.Value;
                    val = randomSyn.Value + Random.Range(-10, 10) / 1000;
                    network.Find(randomNode).Value.SendingSynapses.Find(randomSyn).Value.Value = val;
                    network.Find(randomSyn.ReceivingNode).Value.ReceivingSynapses.Find(randomSyn).Value.Value = val;
                }
            }
            else 
            {
                Node randomNode = PickRandomNode(network, NodeTypes.Hidden);
                if (randomNode.SendingSynapses.Count > 0)
                {
                    Synapse randomSyn = PickRandomSynapse(network.Find(randomNode).Value.SendingSynapses);
                    float val = randomSyn.Value;
                    val = randomSyn.Value + Random.Range(-10, 10) / 1000;
                    network.Find(randomNode).Value.SendingSynapses.Find(randomSyn).Value.Value = val;
                    network.Find(randomSyn.ReceivingNode).Value.ReceivingSynapses.Find(randomSyn).Value.Value = val;
                }
            }
        }
        else if (mutationType == MutateBrainTypes.DIS && synapseNum > 0)
        {
            if (Random.Range(1, 2) == 1) //Disable a synapse coming from a input node
            {
                Node randomNode = PickRandomNode(network, NodeTypes.Input);
                if (randomNode.SendingSynapses.Count > 0)
                {
                    Synapse randomSyn = PickRandomSynapse(network.Find(randomNode).Value.SendingSynapses);
                    network.Find(randomNode).Value.SendingSynapses.Find(randomSyn).Value.Disabled = true;
                    network.Find(randomSyn.ReceivingNode).Value.ReceivingSynapses.Find(randomSyn).Value.Disabled = true;
                }
            }
            else
            {
                Node randomNode = PickRandomNode(network, NodeTypes.Hidden);
                if (randomNode.SendingSynapses.Count > 0)
                {
                    Synapse randomSyn = PickRandomSynapse(network.Find(randomNode).Value.SendingSynapses);
                    network.Find(randomNode).Value.SendingSynapses.Find(randomSyn).Value.Disabled = true;
                    network.Find(randomSyn.ReceivingNode).Value.ReceivingSynapses.Find(randomSyn).Value.Disabled = true;
                }
            }
        }
        else if (mutationType == MutateBrainTypes.CHN && hiddenNodes > 0)
        {
            Node randomNode = PickRandomNode(network, NodeTypes.Hidden);
            var nodeFunctionType = (NodeTypes)Random.Range(0, 8);
            network.Find(randomNode).Value.HiddenNodeType = nodeFunctionType;
        }
    }
    #endregion

    public void BrainTick()
    {

        foreach (Node node in network)
        {
            float updatedNodeValue = 0;
            if (node.NodeType == NodeTypes.Hidden)
            {
                foreach (Synapse synapse in node.ReceivingSynapses)
                {
                    updatedNodeValue = updatedNodeValue + (network.Find(synapse.SourceNode).Value.NodeValue * synapse.Value);
                }
                
                network.Find(node).Value.NodeFunction(updatedNodeValue);
            }
            else if (node.NodeType == NodeTypes.Output)
            {
                foreach (Synapse synapse in node.ReceivingSynapses)
                {
                    updatedNodeValue = updatedNodeValue + (network.Find(synapse.SourceNode).Value.NodeValue * synapse.Value);
                }
                network.Find(node).Value.NodeFunction(updatedNodeValue);
            }
        }
    }

    public static Node PickRandomNode(LinkedList<Node> list, NodeTypes type) //Reservoir Sampling Algorithm to pick random nodes due to linkedlist being inefficiant at index searching
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

    public static Synapse PickRandomSynapse(LinkedList<Synapse> list) //Reservoir Sampling Algorithm to pick random synapse due to linkedlist being inefficiant at index searching
    {
        var n = 1;
        Synapse result = default(Synapse);
        var synapsefirst = list.First;
        while (synapsefirst != null)
        {
            if (Random.Range(0, n) == 0)
            {
                result = synapsefirst.Value;
            }
            synapsefirst = synapsefirst.Next;
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

    public float NodeValue
    {
        get { return nodeValue; }
        set { nodeValue = value; }
    }

    public void NodeFunction(float value)
    {
        switch (hiddenNodeType)
        {
            case NodeTypes.SIG:
                value = Mathf.Sign(value);
                break;
            case NodeTypes.LIN:
                value = value;
                break;
            case NodeTypes.SQR:
                value = (value * value);
                break;
            case NodeTypes.SIN:
                value = Mathf.Sin(value);
                break;
            case NodeTypes.ABS:
                value = Mathf.Abs(value);
                break;
            case NodeTypes.REL:
                value = Mathf.Max(0, value); // Possibly wrong function
                break;
            case NodeTypes.GAU:
                value = Mathf.Exp(-Mathf.Pow(value, 2));
                break;
            case NodeTypes.LAT:
                if (Mathf.Abs(value) > 0)
                {
                    value = 0;
                }
                else if (value == 0)
                {
                    value = 0;
                }
                break;
        }
        nodeValue = value;
    }
}

public class Synapse
{
    private Node sourceNode;
    private Node receivingNode;
    private float Synvalue;
    private bool disabled;


    public Synapse(Node Source, Node Receiving, float value)
    {
        this.sourceNode = Source;
        this.receivingNode = Receiving;
        this.Synvalue = value;
        this.disabled = false;
    }

    public Node SourceNode
    {
        get { return sourceNode; }
    }

    public Node ReceivingNode
    {
        get { return receivingNode; }
    }

    public float Value
    {
        get { return Synvalue; }
        set { Synvalue = value; }
    }

    public bool Disabled
    {
        get { return disabled; }
        set { disabled = value; }
    }
}

enum MutateBrainTypes
{
    ADS, //ADS - Add synapse, CHS - Change synapse, DIS - Disable Synapse, ENB - Enable a synapse, ADN - add a hidden Node/Neuron, CHN - Change Neuron function, RSN - Remove Synapse/Node
    CHS,
    DIS,
    ENB,
    ADN,
    CHN,
    RSN
}
public enum NodeTypes
{
    SIG, //SIG - Sigmoid, LIN - Linear, SQR - Square, SIN - Sinus, ABS - Absolute, REL - Rectified Linear Unit, GAU - Gaussian, LAT - Latch 
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