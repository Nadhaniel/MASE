using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class Brain
{
    //private List<List<Node>> network;
    private Dictionary<(int, bool), List<Node>> network; //each entry represents a layer of neurons
    private int synapseNum = 0;
    private int inputNodes;
    private int hiddenNodes;
    private int outputNodes;

    public Dictionary<(int, bool), List<Node>> Network
    {
        get { return network; }
        set { network = value; }
    }

    public int InputNodesCount
    {
        get 
        {
            inputNodes = network[(0, true)].Count;
            return inputNodes;
        }
    }

    public int HiddenNodesCount
    {
        get
        {
            int count = 0;
            for (int i = 1; i < 6; i++)
            {
                if (network.ContainsKey((i, true)))
                {
                    count += network[(i, true)].Count;
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
            inputNodes = network[(6, true)].Count;
            return inputNodes;
        }
    }

    public Brain(List<Node> input, List<Node> output)
    {
        network = new Dictionary<(int, bool), List<Node>>();
        int numberLayers = Random.Range(1, 7); //Choosing the number of layers a network could have
        network.Add((0, true), input); //Setups the input layer
        network.Add((1, true), new List<Node>()); //Adding one active hidden layer the rest are inactive i.e. considered not to exist
        #region Node Initialization
        var activationFunction = (NodeTypes)Random.Range(0, 8);
        Node initHidden = new Node(NodeTypes.Hidden, 0f, "H" + 0, 0, 1);
        initHidden.HiddenNodeType = activationFunction;
        Node InputSrc = PickRandomNode(network[(0, true)]);
        network[(1, true)].Add(initHidden);
        ADS(network[(0, true)][InputSrc.Index], network[(1, true)][initHidden.Index], 1f);
        for (int i = 2; i < 6; i++)
        {
            if (i < numberLayers)
            {
                network.Add((i, true), new List<Node>());
                var ActivationFunction = (NodeTypes)Random.Range(0, 8);
                Node inithidden = new Node(NodeTypes.Hidden, 0f, "H" + 0, 0, i);
                inithidden.HiddenNodeType = activationFunction;
                Node inputsrc = PickRandomNode(network[(i - 1, true)]);
                network[(i, true)].Add(inithidden);
                ADS(network[(i - 1, true)][0], network[(i, true)][0], 1f);
            }
            else
            {
                network.Add((i, false), new List<Node>());
            }
        }
        int layerbefore = 0;
        if (numberLayers == 1)
        {
            layerbefore = numberLayers;
        }
        else
        {
            layerbefore = numberLayers - 1;
        }
        network.Add((6, true), output);
        Node randomOutputNode = PickRandomNode(network[(6, true)]);
        ADS(network[(layerbefore, true)][0], network[(6, true)][randomOutputNode.Index], 1f);
        #endregion  //Creates the first basic nodes and adds them to the network
        inputNodes = input.Count;
        outputNodes = output.Count;
    }

    public Brain(Brain brain)
    {
        network = brain.Network;
    }

    //Mutation related code
    #region Mutation Methods
    public void ADN(NodeTypes type, NodeTypes ActivationFunction, float value, string name, int index, int layer, Node SourceNode, Node ReceivingNode)
    {
        Node newNode = new Node(type, value, name, index, layer);
        newNode.HiddenNodeType = ActivationFunction;
        int layerBefore = SourceNode.Layer;
        int layerAfter = ReceivingNode.Layer;

        Synapse newSynapse1 = new Synapse(network[(layerBefore, true)][SourceNode.Index], newNode, 1);
        synapseNum += 1;
        Synapse newSynapse2 = new Synapse(newNode, network[(layerAfter, true)][ReceivingNode.Index], 1);
        synapseNum += 1;
        newNode.ReceivingSynapses.Add(newSynapse1);
        network[(layerBefore, true)][SourceNode.Index].SendingSynapses.Add(newSynapse1);
        newNode.SendingSynapses.Add(newSynapse2);
        network[(layerAfter, true)][ReceivingNode.Index].ReceivingSynapses.Add(newSynapse2);
        network[(layer, true)].Add(newNode);
    }

    public void ADS(Node SourceNode, Node ReceivingNode, float SynapseValue)
    {
        Synapse newSynapse = new Synapse(SourceNode, ReceivingNode, SynapseValue);
        synapseNum += 1;
        network[(SourceNode.Layer, true)][SourceNode.Index].SendingSynapses.Add(newSynapse);
        network[(ReceivingNode.Layer, true)][ReceivingNode.Index].ReceivingSynapses.Add(newSynapse);
    }
    public void CHS(Synapse receivingsynapse, float value)
    {
        receivingsynapse.SourceNode.SendingSynapses.Find(x => x.ID == receivingsynapse.ID).Value = value;
    }

    public void FLP(Synapse synapse)
    {
        if (synapse.Disabled == false)
        {
            synapse.Disabled = true;
        }
        else
        {
            synapse.Disabled = false;
        }
    }

    public void CHN(Node node, NodeTypes activationfunciton)
    {
        if (node.NodeType == NodeTypes.Hidden)
        {
            node.HiddenNodeType = activationfunciton;
        }
        else
        {
            //throw error
        }
    }

    public void ADL()
    {
        Dictionary<(int, bool), List<Node>> new_network = new Dictionary<(int, bool), List<Node>>();
        int count = 0;
        for (int i = 0; i < network.Count; i++)
        {
            if (network.ContainsKey((i, false)))
            {
                count = i;
                break;
            }
        }
        for (int i = 0; i < network.Count; i++)
        {

            if (count == i)
            {
                new_network.Add((i, true), network[(i, false)]);
            }
            else
            {
                if (network.ContainsKey((i, false)))
                {
                    new_network.Add((i, false), network[(i, false)]);
                }
                else
                {
                    new_network.Add((i, true), network[(i, true)]);
                }
            }
        }
        network.Clear();
        network = new_network;
    }

    public void RMN(Node node)
    {
        int layerKey = node.Layer;
        if (network[(layerKey, true)].Count > 1)
        {
            for (int r = 0; r < node.ReceivingSynapses.Count; r++)
            {
                node.ReceivingSynapses[r].SourceNode.SendingSynapses.Remove(node.ReceivingSynapses[r]);
                node.ReceivingSynapses.RemoveAt(r);
                synapseNum -= 1;
            }
            for (int s = 0; s < node.SendingSynapses.Count; s++)
            {
                node.SendingSynapses[s].ReceivingNode.ReceivingSynapses.Remove(node.SendingSynapses[s]);
                node.SendingSynapses.RemoveAt(s);
                synapseNum -= 1;
            }
            network[(layerKey, true)].Remove(node);
            for (int i = 0; i < network[(layerKey, true)].Count; i++) //Fixing the node indexes affter removal of node
            {
                network[(layerKey, true)][i].Index = i;
                network[(layerKey, true)][i].NodeName = "H" + i;
            }
        }
    }

    public void RMS(Synapse synapse)
    {
        if (synapse.SourceNode.SendingSynapses.Count > 1)
        {
            synapse.SourceNode.SendingSynapses.Remove(synapse);
            synapse.ReceivingNode.ReceivingSynapses.Remove(synapse);
            synapseNum -= 1;
        }
    }

    public void ClearFloatingStructures()
    {
        for (int l = 1; l < 6; l++)
        {
            if (network.ContainsKey((l, true)))
            {
                for (int n = 0; n < network[(l, true)].Count; n++)
                {
                    if (network[(l, true)][n].ReceivingSynapses.Count == 0 || network[(l, true)][n].SendingSynapses.Count == 0)
                    {
                        RMN(network[(l, true)][n]);
                    }
                }
            }
        }
    }
    #endregion
    #region Mutation Algorithm
    public void MutateBrain()
    {
        MutateBrainTypes mutationType;
        if (HiddenNodesCount == 0)
        {
            mutationType = (MutateBrainTypes)Random.Range(0, 2);
        }
        else
        {
            mutationType = (MutateBrainTypes)Random.Range(0, 5);
        }
        if (mutationType == MutateBrainTypes.ADN)
        {
            var activationFunction = (NodeTypes)Random.Range(0, 8);
            int layerKey = PickRandomLayer();
            int layerafter = layerKey + 1;
            while (!network.ContainsKey((layerafter, true)))
            {
                layerafter += 1;
            }
            Node receiving_from_node = PickRandomNode(network[(layerKey - 1, true)]);
            Node sending_to_node = PickRandomNode(network[(layerafter, true)]);
            ADN(NodeTypes.Hidden, activationFunction, 0f, "H" + network[(layerKey, true)].Count, network[(layerKey, true)].Count, layerKey, receiving_from_node, sending_to_node);
        }
        if (mutationType == MutateBrainTypes.ADS)
        {
            if (Random.Range(0, 2) == 0) //Picking between direct connection from input to output or any connection
            {
                int layerKey = PickRandomLayer();
                int layerafter = layerKey + 1;
                while (!network.ContainsKey((layerafter, true)))
                {
                    layerafter += 1;
                }

                List<Node> possiblesourcenodes = network[(layerKey, true)].FindAll(x => x.SendingSynapses.Count < network[(layerafter, true)].Count);
                if (possiblesourcenodes.Count != 0)
                {
                    Node sourcenode = PickRandomNode(possiblesourcenodes);
                    List<Node> possiblereceivers = new List<Node>();
                    for (int i = 0; i < network[(layerafter, true)].Count; i++)
                    {
                        if (network[(layerafter, true)][i].ReceivingSynapses.Any(r => r.SourceNode == sourcenode))
                        {

                        }
                        else
                        {
                            possiblereceivers.Add(network[(layerafter, true)][i]);
                        }
                    }
                    if (possiblereceivers.Count != 0)
                    {
                        Node receivingnode = PickRandomNode(possiblereceivers);
                        ADS(sourcenode, receivingnode, 1f);
                    }
                }
            }
            else
            {
                List<Node> sourcenodes = network[(0, true)].FindAll(x => x.SendingSynapses.Count < network[(6, true)].Count);
                Node sourcenode = PickRandomNode(sourcenodes);
                List<Node> possiblereceivers = new List<Node>();
                for (int i = 0; i < network[(6, true)].Count; i++)
                {
                    if (network[(6, true)][i].ReceivingSynapses.Any(r => r.SourceNode == sourcenode))
                    {

                    }
                    else
                    {
                        possiblereceivers.Add(network[(6, true)][i]);
                    }
                }
                if (possiblereceivers.Count != 0)
                {
                    Node receivingnode = PickRandomNode(possiblereceivers);
                    ADS(sourcenode, receivingnode, 1f);
                }
            }
        }
        if (mutationType == MutateBrainTypes.CHS)
        {
            int layerKey = PickRandomLayer();
            Node affectednode = PickRandomNode(network[(layerKey, true)]);
            if (affectednode.ReceivingSynapses.Count > 0)
            {
                Synapse affectedsyn = PickRandomSynapse(affectednode.ReceivingSynapses);
                float value = affectedsyn.Value + Random.Range(-1.5f, 1.5f);
                CHS(affectedsyn, value);
            }
        }
        if (mutationType == MutateBrainTypes.FLP)
        {
            int layerKey = PickRandomLayer();
            Node affectednode = PickRandomNode(network[(layerKey, true)]);
            if (affectednode.ReceivingSynapses.Count > 0)
            {
                Synapse affectedsyn = PickRandomSynapse(affectednode.ReceivingSynapses);
                FLP(affectedsyn);
            }
        }
        if (mutationType == MutateBrainTypes.CHN)
        {
            int layerKey = PickRandomLayer();
            Node affectednode = PickRandomNode(network[(layerKey, true)]);
            NodeTypes activation = (NodeTypes)Random.Range(0, 8);
            CHN(affectednode, activation);
        }
        if (mutationType == MutateBrainTypes.RMN)
        {
            int layerKey = PickRandomLayer();
            Node node = PickRandomNode(network[(layerKey, true)]);
            RMN(node);
        }
        if (mutationType == MutateBrainTypes.RMS)
        {
            int layerKey;
            if (Random.Range(0, 2) == 0)
            {
                layerKey = PickRandomLayer();
            }
            else
            {
                layerKey = 0;
            }
            if (layerKey != 0)
            {
                Node node = PickRandomNode(network[(layerKey, true)]);
                if (Random.Range(0, 2) == 0)
                {
                    Synapse synapse = PickRandomSynapse(node.SendingSynapses);
                    RMS(synapse);
                }
                else
                {
                    Synapse synapse = PickRandomSynapse(node.ReceivingSynapses);
                    RMS(synapse);
                }
            }
            else
            {
                Node node = PickRandomNode(network[(layerKey, true)]);
                if (node.SendingSynapses.Count > 0)
                {
                    Synapse synapse = PickRandomSynapse(node.SendingSynapses);
                    RMS(synapse);
                }
            }
        }
        Debug.Log(mutationType);
    }
    #endregion

    //feedforward algorithm
    public void BrainTick()
    {
        float updatedNodeValue = 0;
        for (int i = 1; i < network.Count; i++)
        {
            if (network.ContainsKey((i, true)))
            {
                foreach (Node node in network[(i, true)])
                {
                    bool containssending = false;
                    foreach (Synapse synapse in node.ReceivingSynapses)
                    {
                        if (synapse.Disabled == false)
                        {
                            containssending = true;
                            updatedNodeValue = updatedNodeValue + (network[(synapse.SourceNode.Layer, true)][synapse.SourceNode.Index].NodeValue * synapse.Value);
                        }
                    }
                    if (containssending)
                    {
                        network[(i, true)][node.Index].NodeFunction(updatedNodeValue);
                    }
                    updatedNodeValue = 0;
                }
            }
        }
    }

    //Random pickers
    public Node PickRandomNode(List<Node> list)
    {
        int randomNodeIndex = 0;
        randomNodeIndex = Random.Range(0, list.Count);
        return list[randomNodeIndex];
    }

    public int PickRandomLayer()
    {
        List<(int, bool)> test = new List<(int, bool)>();
        foreach ((int, bool) key in network.Keys)
        {
            if (key.Item1 > 0 && key.Item1 < 6 && key.Item2 == true)
            {
                test.Add(key);
            }
        }
        (int, bool) layerkey = test[Random.Range(0, test.Count)];
        return layerkey.Item1;
    }
    public static Synapse PickRandomSynapse(List<Synapse> list) //Reservoir Sampling Algorithm to pick random synapse due to linkedlist being inefficiant at index searching
    {
        int randomIndex = 0;
        randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }
}

public class Synapse
{
    private Node sourceNode;
    private Node receivingNode;
    private float Synvalue;
    private string id;
    private bool disabled;

    public Synapse(Node Source, Node Receiving, float value)
    {
        this.sourceNode = Source;
        this.receivingNode = Receiving;
        this.Synvalue = value;
        this.disabled = false;
        this.id = Guid.NewGuid().ToString();
    }

    public Node SourceNode
    {
        get { return sourceNode; }
        set { sourceNode = value; }
    }

    public Node ReceivingNode
    {
        get { return receivingNode; }
        set { receivingNode = value; }
    }

    public float Value
    {
        get { return Synvalue; }
        set { Synvalue = value; }
    }

    public string ID
    {
        get { return id; }
        set { id = value; }
    }

    public bool Disabled
    {
        get { return disabled; }
        set { disabled = value; }
    }
}

enum MutateBrainTypes
{
    ADN,
    ADS,//ADS - Add synapse, CHS - Change synapse, FLP - flips the synapse active bool, ADN - add a hidden Node/Neuron, CHN - Change Neuron function, RMN - Remove Node, RMS - Remove Synapse
    CHS,
    FLP,
    CHN,
    RMN,
    RMS
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