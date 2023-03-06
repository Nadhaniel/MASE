using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class Brain
{
    private List<Node> network;
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
        network = new List<Node>();
        foreach (Node node in input)
        {
            node.Index = network.Count;
            network.Add(node);
        }
        foreach (Node node in output)
        {
            node.Index = network.Count;
            network.Add(node);
        }
        inputNodes = input.Count;
        outputNodes = output.Count;
        hiddenNodes = 0;
    }

    public Brain(Brain brain)
    {
        network = brain.Network;
    }

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
            mutationType = (MutateBrainTypes)Random.Range(0, 7);
        }
        //Debug.Log(mutationType.ToString());
        if (mutationType == MutateBrainTypes.ADN)
        {
            var nodeFunctionType = (NodeTypes)Random.Range(0, 8);
            Node newNode = new Node(NodeTypes.Hidden, 0f);
            newNode.HiddenNodeType = nodeFunctionType;
            newNode.Index = network.Count;
            int sourceInputorHiddenLayer = Random.Range(0, 2); // May be problem here with random.range
            if (HiddenNodesCount == 0 && sourceInputorHiddenLayer == 1)
            {
                sourceInputorHiddenLayer = 0;
            }
            int receivingHiddenOutputLayer = Random.Range(1, 3); // may be problem here with random.range
            if (HiddenNodesCount == 0 && receivingHiddenOutputLayer == 1)
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
                    newNode.ReceivingSynapses.Add(newSynapse1);
                    network[randInputNode].SendingSynapses.Add(newSynapse1);
                    newNode.SendingSynapses.Add(newSynapse2);
                    network[randOutputNode].ReceivingSynapses.Add(newSynapse2);
                    network.Add(newNode);
                }
                else if (receivingHiddenOutputLayer == 1 && hiddenNodes > 0) //adding a node between a input node and a hidden node
                {
                    int randInputNode = PickRandomNodeIndex(network, NodeTypes.Input);
                    int randHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden);

                    Synapse newSynapse1 = new Synapse(network[randInputNode], newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(newNode, network[randHiddenNode], 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.Add(newSynapse1);
                    network[randInputNode].SendingSynapses.Add(newSynapse1);
                    newNode.SendingSynapses.Add(newSynapse2);
                    network[randHiddenNode].ReceivingSynapses.Add(newSynapse2);
                    network.Add(newNode);
                }
            }
            else if(sourceInputorHiddenLayer == 1)
            {
                if (receivingHiddenOutputLayer == 2) //adding a node between a hidden node and output node
                {
                    int randHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                    int randOutputNode = PickRandomNodeIndex(network, NodeTypes.Output);

                    Synapse newSynapse1 = new Synapse(network[randHiddenNode], newNode, 1);
                    synapseNum += 1;
                    Synapse newSynapse2 = new Synapse(newNode, network[randOutputNode], 1);
                    synapseNum += 1;
                    newNode.ReceivingSynapses.Add(newSynapse1);
                    network[randHiddenNode].SendingSynapses.Add(newSynapse1);
                    newNode.SendingSynapses.Add(newSynapse2);
                    network[randOutputNode].ReceivingSynapses.Add(newSynapse2);
                    network.Add(newNode);
                }
                else if(receivingHiddenOutputLayer == 1 && HiddenNodesCount > 1)
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
                    newNode.ReceivingSynapses.Add(newSynapse1);
                    network[randHiddenNode].SendingSynapses.Add(newSynapse1);
                    newNode.SendingSynapses.Add(newSynapse2);
                    network[secondrandHiddenNode].ReceivingSynapses.Add(newSynapse2);
                    network.Add(newNode);
                }
            }
            //Debug.Log(HiddenNodesCount.ToString());
        }
        else if (mutationType == MutateBrainTypes.ADS)
        {
            int sourceInputorHiddenLayer = Random.Range(0, 3); // May be problem here with random.range
            if (HiddenNodesCount == 0 && sourceInputorHiddenLayer == 1)
            {
                sourceInputorHiddenLayer = 0;
            }
            int receivingHiddenOutputLayer = Random.Range(1, 3); // may be problem here with random.range
            if (HiddenNodesCount == 0 && receivingHiddenOutputLayer == 1)
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
                    network[randInputNode].SendingSynapses.Add(newSynapse);
                    network[randOutputNode].ReceivingSynapses.Add(newSynapse);
                    synapseNum += 1;
                }
                else if (receivingHiddenOutputLayer ==  1)
                {
                    int randInputNode = PickRandomNodeIndex(network, NodeTypes.Input);
                    int randHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden);

                    Synapse newSynapse = new Synapse(network[randInputNode], network[randHiddenNode], 1);
                    network[randInputNode].SendingSynapses.Add(newSynapse);
                    network[randHiddenNode].ReceivingSynapses.Add(newSynapse);
                    synapseNum += 1;
                }
            }
            else if(sourceInputorHiddenLayer == 1)
            {
                if (receivingHiddenOutputLayer == 2)
                {
                    int randHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                    int randOutputNode = PickRandomNodeIndex(network, NodeTypes.Output);

                    Synapse newSynapse = new Synapse(network[randHiddenNode], network[randOutputNode], 1);
                    network[randHiddenNode].SendingSynapses.Add(newSynapse);
                    network[randOutputNode].ReceivingSynapses.Add(newSynapse);
                    synapseNum += 1;
                }
                else if(receivingHiddenOutputLayer == 1 && HiddenNodesCount > 1)
                {
                    int randHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                    int randHiddenNode2 = PickRandomNodeIndex(network, NodeTypes.Hidden);

                    while (randHiddenNode == randHiddenNode2)
                    {
                        randHiddenNode = PickRandomNodeIndex(network, NodeTypes.Hidden); //Could become very inefficient due to luck 
                    }

                    Synapse newSynapse = new Synapse(network[randHiddenNode], network[randHiddenNode2], 1);
                    network[randHiddenNode].SendingSynapses.Add(newSynapse);
                    network[randHiddenNode2].ReceivingSynapses.Add(newSynapse);
                    synapseNum += 1;
                }
            }
        }
        //else if (mutationType == MutateBrainTypes.RSN && HiddenNodesCount > 0)
        //{
        //    Dictionary<int, int> indexMap = new Dictionary<int, int>();
        //    if (Random.Range(1, 3) == 1) // Delete Node if random == 1 otherwise delete synapse
        //    {
        //        int randomNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
        //        for (int s = 0; s < network[randomNode].ReceivingSynapses.Count; s++)
        //        {
        //            int SourceNodeIndex = network[randomNode].ReceivingSynapses[s].SourceNode.Index;
        //            for (int i = 0; i < network[SourceNodeIndex].SendingSynapses.Count; i++)
        //            {
        //                if (network[SourceNodeIndex].SendingSynapses[i].ID == network[randomNode].ReceivingSynapses[s].ID)
        //                {
        //                    network[SourceNodeIndex].SendingSynapses.RemoveAt(i);
        //                    synapseNum -= 1;
        //                }
        //            }
        //        }
        //        for (int s = 0; s < network[randomNode].SendingSynapses.Count; s++)
        //        {
        //            int ReceivingNodeIndex = network[randomNode].SendingSynapses[s].ReceivingNode.Index;
        //            for (int i = 0; i < network[ReceivingNodeIndex].ReceivingSynapses.Count; i++)
        //            {
        //                if (network[randomNode].SendingSynapses[s].ID == network[ReceivingNodeIndex].ReceivingSynapses[i].ID)
        //                {
        //                    network[ReceivingNodeIndex].ReceivingSynapses.RemoveAt(i);
        //                    synapseNum -= 1;
        //                }
        //            }
        //        }
        //        for (int i = 0; i < network.Count; i++)
        //        {
        //            indexMap.Add(network[i].Index, i);
        //        }
        //        network.Remove(network[randomNode]);
        //        indexMap.Remove(randomNode);


        //        //Reset hidden node IDs after removal
        //        for (int j = 0; j < network.Count; j++)
        //        {
        //            if (j >= randomNode)
        //            {
        //                indexMap[network[j].Index] = j;
        //                network[j].Index = j;
        //            }
        //            else
        //            {
        //                indexMap[network[j].Index] = j;
        //            }
        //        }
        //    }
        //    else if (synapseNum > 0)
        //    {
        //        int randomNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
        //        if (Random.Range(1, 3) == 1 && network[randomNode].SendingSynapses.Count > 0) // delete a sending synapse 
        //        {
        //            int randSynapse = PickRandomSynapse(network[randomNode].SendingSynapses);
        //            int ReceivingNodeIndex = network[randomNode].SendingSynapses[randSynapse].ReceivingNode.Index;
        //            for (int i = 0; i < network[ReceivingNodeIndex].ReceivingSynapses.Count; i++)
        //            {
        //                if (network[ReceivingNodeIndex].ReceivingSynapses[i].ID == network[randomNode].SendingSynapses[randSynapse].ID)
        //                {
        //                    network[ReceivingNodeIndex].ReceivingSynapses.RemoveAt(i);
        //                    break;
        //                }
        //            }
        //            network[randomNode].SendingSynapses.RemoveAt(randSynapse);
        //        }
        //        else if (network[randomNode].ReceivingSynapses.Count > 0)
        //        {
        //            int randSynapse = PickRandomSynapse(network[randomNode].ReceivingSynapses);
        //            int SourceNodeIndex = network[randomNode].ReceivingSynapses[randSynapse].SourceNode.Index;
        //            for (int i = 0; i < network[SourceNodeIndex].SendingSynapses.Count; i++)
        //            {
        //                if (network[SourceNodeIndex].SendingSynapses[i].ID == network[randomNode].ReceivingSynapses[randSynapse].ID)
        //                {
        //                    network[SourceNodeIndex].SendingSynapses.RemoveAt(i);
        //                    break;
        //                }
        //            }
        //            network[randomNode].ReceivingSynapses.RemoveAt(randSynapse);
        //        }
        //        if (network[randomNode].ReceivingSynapses.Count == 0 || network[randomNode].SendingSynapses.Count == 0) //a Final if statement to remove any floating structures in the nerual network
        //        {
        //            for (int i = 0; i < network[randomNode].ReceivingSynapses.Count; i++)
        //            {
        //                int sourceNodeIndex = network[randomNode].ReceivingSynapses[i].SourceNode.Index;
        //                for (int s = 0; s < network[sourceNodeIndex].SendingSynapses.Count; s++)
        //                {
        //                    if (network[sourceNodeIndex].SendingSynapses[s].ID == network[randomNode].ReceivingSynapses[i].ID)
        //                    {
        //                        network[sourceNodeIndex].SendingSynapses.RemoveAt(s);
        //                        network[randomNode].ReceivingSynapses.RemoveAt(i);
        //                        break;
        //                    }
        //                }
        //            }
        //            for (int i = 0; i < network[randomNode].SendingSynapses.Count; i++)
        //            {
        //                int receivingNodeIndex = network[randomNode].SendingSynapses[i].ReceivingNode.Index;
        //                for (int s = 0; s < network[receivingNodeIndex].ReceivingSynapses.Count; s++)
        //                {
        //                    if (network[receivingNodeIndex].ReceivingSynapses[s].ID == network[randomNode].SendingSynapses[i].ID)
        //                    {
        //                        network[receivingNodeIndex].ReceivingSynapses.RemoveAt(s);
        //                        network[randomNode].SendingSynapses.RemoveAt(i);
        //                        break;
        //                    }
        //                }
        //            }
        //            for (int i = 0; i < network.Count; i++)
        //            {
        //                indexMap.Add(network[i].Index, i);
        //            }
        //            network.Remove(network[randomNode]);
        //            indexMap.Remove(randomNode);

        //            //Reset hidden node IDs after removal
        //            for (int j = 0; j < network.Count; j++)
        //            {
        //                if (j >= randomNode)
        //                {
        //                    indexMap[network[j].Index] = j;
        //                    network[j].Index = j;
        //                }
        //                else
        //                {
        //                    indexMap[network[j].Index] = j;
        //                }
        //            }
        //        }

        //        synapseNum -= 1;
        //    }
        //}
        else if (mutationType == MutateBrainTypes.CHS && synapseNum > 0)
        {
            if (Random.Range(1, 3) == 1) //Mutate a synapse coming from a input node
            {
                int randomNode = PickRandomNodeIndex(network, NodeTypes.Input);
                if (network[randomNode].SendingSynapses.Count > 0)
                {
                    int randomSynIndex = PickRandomSynapse(network[randomNode].SendingSynapses);
                    float offset = (Random.Range(-10, 10)) / 1000f;
                    network[randomNode].SendingSynapses[randomSynIndex].Value += offset;
                    for (int i = 0; i < network.Count; i++)
                    {
                        if (network[i].ID == network[randomNode].SendingSynapses[randomSynIndex].ReceivingNode.ID)
                        {
                            for (int s = 0; s < network[i].ReceivingSynapses.Count; s++)
                            {
                                if (network[i].ReceivingSynapses[s].ID == network[randomNode].SendingSynapses[randomSynIndex].ID)
                                {
                                    network[i].ReceivingSynapses[s].Value += offset;
                                    break;
                                }
                            }
                            //network[i].ReceivingSynapses.Find(randomSyn).Value.Value = val;
                            break;
                        }
                    }
                    //network.Find(x => x.Equals(randomSyn.ReceivingNode)).ReceivingSynapses.Find(randomSyn).Value.Value = val;
                }
            }
            else 
            {
                int randomNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                if (network[randomNode].SendingSynapses.Count > 0)
                {
                    int randomSynIndex = PickRandomSynapse(network[randomNode].SendingSynapses);
                    float offset = (Random.Range(-10, 10)) / 1000f;
                    network[randomNode].SendingSynapses[randomSynIndex].Value += offset;
                    for (int i = 0; i < network.Count; i++)
                    {
                        if (network[i].ID == network[randomNode].SendingSynapses[randomSynIndex].ReceivingNode.ID)
                        {
                            for (int s = 0; s < network[i].ReceivingSynapses.Count; s++)
                            {
                                if (network[i].ReceivingSynapses[s].ID == network[randomNode].SendingSynapses[randomSynIndex].ID)
                                {
                                    network[i].ReceivingSynapses[s].Value += offset;
                                    break;
                                }
                            }
                            //network[i].ReceivingSynapses.Find(randomSyn).Value.Value = val;
                            break;
                        }
                    }
                    //network.Find(x => x.Equals(randomSyn.ReceivingNode)).ReceivingSynapses.Find(randomSyn).Value.Value = val;
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
                    int randomSynIndex = PickRandomSynapse(network[randomNode].SendingSynapses);
                    network[randomNode].SendingSynapses[randomSynIndex].Disabled = true;
                    for (int i = 0; i < network.Count; i++)
                    {
                        if (network[i].ID == network[randomNode].SendingSynapses[randomSynIndex].ReceivingNode.ID)
                        {
                            for (int s = 0; s < network[i].ReceivingSynapses.Count; s++)
                            {
                                if (network[i].ReceivingSynapses[s].ID == network[randomNode].SendingSynapses[randomSynIndex].ID)
                                {
                                    network[i].ReceivingSynapses[s].Disabled = true;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    //network.Find(x => x.Equals(randomSyn.ReceivingNode)).ReceivingSynapses.Find(randomSyn).Value.Disabled = true;
                }
            }
            else
            {
                int randomNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                if (network[randomNode].SendingSynapses.Count > 0)
                {
                    int randomSynIndex = PickRandomSynapse(network[randomNode].SendingSynapses);
                    network[randomNode].SendingSynapses[randomSynIndex].Disabled = true;
                    for (int i = 0; i < network.Count; i++)
                    {
                        if (network[i].ID == network[randomNode].SendingSynapses[randomSynIndex].ReceivingNode.ID)
                        {
                            for (int s = 0; s < network[i].ReceivingSynapses.Count; s++)
                            {
                                if (network[i].ReceivingSynapses[s].ID == network[randomNode].SendingSynapses[randomSynIndex].ID)
                                {
                                    network[i].ReceivingSynapses[s].Disabled = true;
                                    break;
                                }
                            }
                            break;
                        }
                    }
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
                    int randomSynIndex = PickRandomSynapse(network[randomNode].SendingSynapses);
                    network[randomNode].SendingSynapses[randomSynIndex].Disabled = false;
                    for (int i = 0; i < network.Count; i++)
                    {
                        if (network[i].ID == network[randomNode].SendingSynapses[randomSynIndex].ReceivingNode.ID)
                        {
                            for (int s = 0; s < network[i].ReceivingSynapses.Count; s++)
                            {
                                if (network[i].ReceivingSynapses[s].ID == network[randomNode].SendingSynapses[randomSynIndex].ID)
                                {
                                    network[i].ReceivingSynapses[s].Disabled = false;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                int randomNode = PickRandomNodeIndex(network, NodeTypes.Hidden);
                if (network[randomNode].SendingSynapses.Count > 0)
                {
                    int randomSynIndex = PickRandomSynapse(network[randomNode].SendingSynapses);
                    network[randomNode].SendingSynapses[randomSynIndex].Disabled = false;
                    for (int i = 0; i < network.Count; i++)
                    {
                        if (network[i].ID == network[randomNode].SendingSynapses[randomSynIndex].ReceivingNode.ID)
                        {
                            for (int s = 0; s < network[i].ReceivingSynapses.Count; s++)                                                      //Lot of repeition going on could break down into methods
                            {
                                if (network[i].ReceivingSynapses[s].ID == network[randomNode].SendingSynapses[randomSynIndex].ID)
                                {
                                    network[i].ReceivingSynapses[s].Disabled = false;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    //network.Find(x => x.Equals(randomSyn.ReceivingNode)).ReceivingSynapses.Find(randomSyn).Value.Disabled = true;
                }
            }
        }
        else if (mutationType == MutateBrainTypes.CHN && HiddenNodesCount > 0)
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
                    //Debug.Log("Network total: " + network.Count + " Sending Synapse index:" + synapse.SourceNode.Index);
                    if (network[synapse.SourceNode.Index] != null && synapse.Disabled == false)
                    {
                        updatedNodeValue = updatedNodeValue + (network[synapse.SourceNode.Index].NodeValue * synapse.Value);
                    }
                }
                
                network[i].NodeFunction(updatedNodeValue);
            }
            else if (network[i].NodeType == NodeTypes.Output)
            {
                foreach (Synapse synapse in network[i].ReceivingSynapses)
                {
                    //Debug.Log("Network Total: " + network.Count + " Sending Synapse index:" + synapse.SourceNode.Index);
                    if (network[synapse.SourceNode.Index] != null && synapse.Disabled == false)
                    {
                        updatedNodeValue = updatedNodeValue + (network[synapse.SourceNode.Index].NodeValue * synapse.Value);
                    }
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
        //Debug.Log("Random Index: " + randomNodeIndex + " " + type.ToString() + "Network total: " + network.Count);
        return randomNodeIndex;
    }

    public static int PickRandomSynapse(List<Synapse> list) //Reservoir Sampling Algorithm to pick random synapse due to linkedlist being inefficiant at index searching
    {
        int randomIndex = 0;
        randomIndex = Random.Range(0, list.Count);
        return randomIndex;
    }
}

public class Node
{
    private List<Synapse> receivingSynapses = new List<Synapse>();
    private List<Synapse> sendingSynapses = new List<Synapse>();
    private float nodeValue = 0;

    private NodeTypes nodeType;
    private NodeTypes hiddenNodeType;
    private int index;
    private string id;
    public Node(NodeTypes nodetype, float value)
    {
        this.nodeType = nodetype;
        this.nodeValue = value;
        this.id = Guid.NewGuid().ToString();
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

    public int Index
    {
        get { return index; }
        set { index = value; }
    }

    public string ID
    {
        get { return id; }
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