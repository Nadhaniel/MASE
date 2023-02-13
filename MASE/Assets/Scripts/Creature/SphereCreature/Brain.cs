using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Brain
{
    private List<Node> network = new List<Node>();
    private int synapseNum = 0;
    private int inputNodes;
    private int hiddenNodes;
    private int outputNodes;

    public List<Node> Network
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
        foreach (Node node in input)
        {
            network.Add(node);
        }
        foreach (Node node in output)
        {
            network.Add(node);
        }
        inputNodes = input.Count;
        outputNodes = output.Count;
        hiddenNodes = 0;
    }

    #region Mutation Algorithm
    public void MutateBrain()
    {
        MutateBrainTypes mutationType;
        if (hiddenNodes == 0)
        {
            mutationType = (MutateBrainTypes)Random.Range(0, 2);
        }
        else
        {
            mutationType = (MutateBrainTypes)Random.Range(0, 6);
        }
        Debug.Log(mutationType.ToString());
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
                    int randInputNode = PickRandomNodeIndex(network, NodeTypes.Input);
                    int randOutputNode = PickRandomNodeIndex(network, NodeTypes.Output);

                    Synapse newSynapse1 = new Synapse(network[randInputNode], newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(newNode, network[randOutputNode], 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.AddLast(newSynapse1);
                    network[randInputNode].SendingSynapses.AddLast(newSynapse1);
                    newNode.SendingSynapses.AddLast(newSynapse2);
                    network[randOutputNode].ReceivingSynapses.AddLast(newSynapse2);
                    network.Add(newNode);
                }
                else if (hiddenNodes > 0) //adding a node between a input node and a hidden node
                {
                    int randInputNode = PickRandomNodeIndex(network, NodeTypes.Input);
                    int randHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden);

                    Synapse newSynapse1 = new Synapse(network[randInputNode], newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(newNode, network[randHiddenNode], 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.AddLast(newSynapse1);
                    network[randInputNode].SendingSynapses.AddLast(newSynapse1);
                    newNode.SendingSynapses.AddLast(newSynapse2);
                    network[randHiddenNode].ReceivingSynapses.AddLast(newSynapse2);
                    network.Add(newNode);
                }
            }
            else
            {
                if (receivingHiddenOutputLayer == 2) //adding a node between a hidden node and output node
                {
                    int randHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                    int randOutputNode = PickRandomNodeIndex(network, NodeTypes.Output);

                    Synapse newSynapse1 = new Synapse(network[randHiddenNode], newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(newNode, network[randOutputNode], 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.AddLast(newSynapse1);
                    network[randHiddenNode].SendingSynapses.AddLast(newSynapse1);
                    newNode.SendingSynapses.AddLast(newSynapse2);
                    network[randOutputNode].ReceivingSynapses.AddLast(newSynapse2);
                    network.Add(newNode);
                }
                else
                {
                    int randHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                    int secondrandHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden);

                    while (randHiddenNode == secondrandHiddenNode)
                    {
                        secondrandHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden); //Could become very inefficient due to luck 
                    }

                    Synapse newSynapse1 = new Synapse(network[randHiddenNode], newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(network[secondrandHiddenNode], newNode, 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.AddLast(newSynapse1);
                    network[randHiddenNode].SendingSynapses.AddLast(newSynapse1);
                    newNode.SendingSynapses.AddLast(newSynapse2);
                    network[secondrandHiddenNode].ReceivingSynapses.AddLast(newSynapse2);
                    network.Add(newNode);
                }
            }
            hiddenNodes += 1;
            Debug.Log(HiddenNodesCount.ToString());
        }
        else if (mutationType == MutateBrainTypes.ADS)
        {
            int sourceInputorHiddenLayer = Random.Range(0, 3); // May be problem here with random.range
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
                    int randInputNode = PickRandomNodeIndex(network, NodeTypes.Input);
                    int randOutputNode = PickRandomNodeIndex(network, NodeTypes.Output);

                    Synapse newSynapse = new Synapse(network[randInputNode], network[randOutputNode], 1);
                    network[randInputNode].SendingSynapses.AddLast(newSynapse);
                    network[randOutputNode].ReceivingSynapses.AddLast(newSynapse);
                    synapseNum += 1;
                }
                else if (hiddenNodes > 0)
                {
                    int randInputNode = PickRandomNodeIndex(network, NodeTypes.Input);
                    int randHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden);

                    Synapse newSynapse = new Synapse(network[randInputNode], network[randHiddenNode], 1);
                    network[randInputNode].SendingSynapses.AddLast(newSynapse);
                    network[randHiddenNode].ReceivingSynapses.AddLast(newSynapse);
                    synapseNum += 1;
                }
            }
            else
            {
                if (receivingHiddenOutputLayer == 2)
                {
                    int randHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                    int randOutputNode = PickRandomNodeIndex(network, NodeTypes.Output);

                    Synapse newSynapse = new Synapse(network[randHiddenNode], network[randOutputNode], 1);
                    network[randHiddenNode].SendingSynapses.AddLast(newSynapse);
                    network[randOutputNode].ReceivingSynapses.AddLast(newSynapse);
                    synapseNum += 1;
                }
                else if(hiddenNodes > 1)
                {
                    int randHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                    int randHiddenNode2 = PickRandomNodeIndex(network, NodeTypes.Hidden);

                    while (randHiddenNode == randHiddenNode2)
                    {
                        randHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden); //Could become very inefficient due to luck 
                    }

                    Synapse newSynapse = new Synapse(network[randHiddenNode], network[randHiddenNode2], 1);
                    network[randHiddenNode].SendingSynapses.AddLast(newSynapse);
                    network[randHiddenNode2].ReceivingSynapses.AddLast(newSynapse);
                    synapseNum += 1;
                }
            }
        }
        else if (mutationType == MutateBrainTypes.RSN && hiddenNodes > 0)
        {
            if (Random.Range(1, 3) == 1) // Delete Node if random == 1 otherwise delete synapse
            {
                int randomNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                foreach (var synapse in network[randomNode].ReceivingSynapses)
                {
                    network.Find(x => x.Equals(synapse.SourceNode)).SendingSynapses.Remove(synapse);
                    synapseNum -= 1;
                }
                foreach (var synapse in network[randomNode].SendingSynapses)
                {
                    network.Find(x => x.Equals(synapse.ReceivingNode)).SendingSynapses.Remove(synapse);
                    synapseNum -= 1;
                }
                network.Remove(network[randomNode]);
                hiddenNodes -= 1;
            }
            else
            {
                int randomNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                if (Random.Range(1, 3) == 1) // delete a sending synapse 
                {
                    Synapse randSynapse = PickRandomSynapse(network[randomNode].SendingSynapses);
                    network.Find(x => x.Equals(randSynapse.ReceivingNode)).ReceivingSynapses.Remove(randSynapse);
                    network.Find(x => x.Equals(network[randomNode])).SendingSynapses.Remove(randSynapse);
                }
                else
                {
                    Synapse randSynapse = PickRandomSynapse(network[randomNode].ReceivingSynapses);
                    network.Find(x => x.Equals(randSynapse.SourceNode)).SendingSynapses.Remove(randSynapse);
                    network.Find(x => x.Equals(randomNode)).ReceivingSynapses.Remove(randSynapse);
                }
                if (network[randomNode].ReceivingSynapses.Count == 0 || network[randomNode].SendingSynapses.Count == 0) //a Final if statement to remove any floating structures in the nerual network
                {
                    foreach (var synapse in network[randomNode].ReceivingSynapses)
                    {
                        network.Find(x => x.Equals(synapse.SourceNode)).SendingSynapses.Remove(synapse);
                        network[randomNode].ReceivingSynapses.Remove(synapse);
                        synapseNum -= 1;
                    }
                    foreach (var synapse in network[randomNode].SendingSynapses)
                    {
                        network.Find(x => x.Equals(synapse.ReceivingNode)).ReceivingSynapses.Remove(synapse);
                        network[randomNode].SendingSynapses.Remove(synapse);
                        synapseNum -= 1;
                    }
                    network.Remove(network[randomNode]);
                    hiddenNodes -= 1;
                }

                synapseNum -= 1;
            }
        }
        else if (mutationType == MutateBrainTypes.CHS && synapseNum > 0)
        {
            if (Random.Range(1, 3) == 1) //Mutate a synapse coming from a input node
            {
                int randomNode = PickRandomNodeIndex(network, NodeTypes.Input);
                if (network[randomNode].SendingSynapses.Count > 0)
                {
                    Synapse randomSyn = PickRandomSynapse(network[randomNode].SendingSynapses);
                    float val = randomSyn.Value;
                    float test = (Random.Range(-10, 10)) / 1000f;
                    val = randomSyn.Value + test;
                    network[randomNode].SendingSynapses.Find(randomSyn).Value.Value = val;
                    network.Find(x => x.Equals(randomSyn.ReceivingNode)).ReceivingSynapses.Find(randomSyn).Value.Value = val;
                }
            }
            else 
            {
                int randomNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                if (network[randomNode].SendingSynapses.Count > 0)
                {
                    Synapse randomSyn = PickRandomSynapse(network[randomNode].SendingSynapses);
                    float val = randomSyn.Value;
                    val = randomSyn.Value + Random.Range(-10, 10) / 1000;
                    network[randomNode].SendingSynapses.Find(randomSyn).Value.Value = val;
                    network.Find(x => x.Equals(randomSyn.ReceivingNode)).ReceivingSynapses.Find(randomSyn).Value.Value = val;
                }
            }
        }
        else if (mutationType == MutateBrainTypes.DIS && synapseNum > 0)
        {
            if (Random.Range(1, 3) == 1) //Disable a synapse coming from a input node
            {
                int randomNode = PickRandomNodeIndex(network, NodeTypes.Input);
                if (network[randomNode].SendingSynapses.Count > 0)
                {
                    Synapse randomSyn = PickRandomSynapse(network[randomNode].SendingSynapses);
                    network[randomNode].SendingSynapses.Find(randomSyn).Value.Disabled = true;
                    network.Find(x => x.Equals(randomSyn.ReceivingNode)).ReceivingSynapses.Find(randomSyn).Value.Disabled = true;
                }
            }
            else
            {
                int randomNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                if (network[randomNode].SendingSynapses.Count > 0)
                {
                    Synapse randomSyn = PickRandomSynapse(network[randomNode].SendingSynapses);
                    network[randomNode].SendingSynapses.Find(randomSyn).Value.Disabled = true;
                    network.Find(x => x.Equals(randomSyn.ReceivingNode)).ReceivingSynapses.Find(randomSyn).Value.Disabled = true;
                }
            }
        }
        else if (mutationType == MutateBrainTypes.ENB && synapseNum > 0)
        {
            if (Random.Range(1, 3) == 1) //Disable a synapse coming from a input node
            {
                int randomNode = PickRandomNodeIndex(network, NodeTypes.Input);
                if (network[randomNode].SendingSynapses.Count > 0)
                {
                    Synapse randomSyn = PickRandomSynapse(network[randomNode].SendingSynapses);
                    network[randomNode].SendingSynapses.Find(randomSyn).Value.Disabled = false;
                    network.Find(x => x.Equals(randomSyn.ReceivingNode)).ReceivingSynapses.Find(randomSyn).Value.Disabled = true;
                }
            }
            else
            {
                int randomNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                if (network[randomNode].SendingSynapses.Count > 0)
                {
                    Synapse randomSyn = PickRandomSynapse(network[randomNode].SendingSynapses);
                    network[randomNode].SendingSynapses.Find(randomSyn).Value.Disabled = false;
                    network.Find(x => x.Equals(randomSyn.ReceivingNode)).ReceivingSynapses.Find(randomSyn).Value.Disabled = true;
                }
            }
        }
        else if (mutationType == MutateBrainTypes.CHN && hiddenNodes > 0)
        {
            int randomNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
            var nodeFunctionType = (NodeTypes)Random.Range(0, 8);
            network[randomNode].HiddenNodeType = nodeFunctionType;
        }
    }
    #endregion

    public void BrainTick()
    {

        for (int i = 0; i < network.Count; i++)
        {
            float updatedNodeValue = 0;
            if (network[i].NodeType == NodeTypes.Hidden)
            {
                foreach (Synapse synapse in network[i].ReceivingSynapses)
                {
                    updatedNodeValue = updatedNodeValue + (network.Find(x => x.Equals(synapse.SourceNode)).NodeValue * synapse.Value);
                }
                
                network[i].NodeFunction(updatedNodeValue);
            }
            else if (network[i].NodeType == NodeTypes.Output)
            {
                foreach (Synapse synapse in network[i].ReceivingSynapses)
                {
                    updatedNodeValue = updatedNodeValue + (network.Find(x => x.Equals(synapse.SourceNode)).NodeValue * synapse.Value);
                }
                network[i].NodeFunction(updatedNodeValue);
            }
        }
    }

    public int PickRandomNodeIndex(List<Node> list, NodeTypes type)
    {
        int randomNodeIndex = 0;
        if (type == NodeTypes.Input)
        {
            randomNodeIndex = Random.Range(0, inputNodes);
        }
        else if (type == NodeTypes.Output)
        {
            randomNodeIndex = Random.Range(inputNodes, inputNodes + outputNodes);
        }
        else
        {
            randomNodeIndex = Random.Range(inputNodes + outputNodes, list.Count);
        }
        return randomNodeIndex;
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
            case NodeTypes.Output:
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
    ADS,
    ADN,//ADS - Add synapse, CHS - Change synapse, DIS - Disable Synapse, ENB - Enable a synapse, ADN - add a hidden Node/Neuron, CHN - Change Neuron function, RSN - Remove Synapse/Node
    CHS,
    DIS,
    ENB,
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