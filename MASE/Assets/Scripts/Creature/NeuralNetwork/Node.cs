using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node
{
    private float nodeValue = 0;
    private List<Synapse> receivingSynapses;
    private List<Synapse> sendingSynapses;
    private Node_Type nodeType;
    private Node_Activations activation_type;
    private int index;
    private string id;
    private string nodeName;
    public Node(Node_Type nodetype, string name, int index)
    {
        this.nodeType = nodetype;
        if (nodetype == Node_Type.Input)
        {
            activation_type = Node_Activations.SIG;
        }
        if (nodetype == Node_Type.Output)
        {
            activation_type = Node_Activations.TanH;
        }
        if (nodetype == Node_Type.Hidden)
        {
            activation_type = Node_Activations.TanH;
        }
        this.nodeName = name;
        this.index = index;
        this.id = Guid.NewGuid().ToString();
        receivingSynapses = new List<Synapse>();
        sendingSynapses = new List<Synapse>();
    }

    public void SetInputVal(float value)
    {
         NodeFunction(value);
    }

    public void setHiddenVal()
    {
        float value = 0;
        foreach (Synapse synapse in receivingSynapses)
        {
            value += (synapse.Weight * synapse.TotalNodeValue);
        }
        NodeFunction(value);
    }

    public void setOutputVal()
    {
        float value = 0;
        foreach (Synapse synapse in receivingSynapses)
        {
            value += (synapse.Weight * synapse.TotalNodeValue);
        }
        NodeFunction(value);
    }

    public Node_Type NodeType
    {
        get { return nodeType; }
        set { nodeType = value; }
    }

    public Node_Activations Activation_Type
    {
        get { return activation_type; }
        set { activation_type = value; }
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

    public string NodeName
    {
        get { return nodeName; }
        set { nodeName = value; }
    }

    public List<Synapse> SendingSynapses
    {
        get { return sendingSynapses; }
        set { sendingSynapses = value; }
    }

    public List<Synapse> ReceivingSynapses
    {
        get { return receivingSynapses; }
        set { receivingSynapses = value; }
    }

    public void FeedForwardVal()
    {
        foreach (Synapse synapse in sendingSynapses)
        {
            if (synapse.Disabled == false)
            {
                synapse.TotalNodeValue = nodeValue;
            }
        }
    }

    public void NodeFunction(float value)
    {
        switch (activation_type)
        {
            case Node_Activations.SIG:
                value = (1 / (1 + Mathf.Exp(-value)));
                break;
            case Node_Activations.TanH:
                value = ((2 / (1 + Mathf.Exp(-4*value))) - 1);
                break;
            case Node_Activations.TanHMod1:
                value = ((2 / (1 + Mathf.Exp(-4 * value))) - 1);
                break;
        }
        nodeValue = value;
    }
}

public class Synapse
{
    private int sourceNode;
    private int receivingNode;
    private float weight;
    private string id;
    private bool disabled;
    private int innovationnumber;
    private float totalnodeval;

    public Synapse(int Source, int Receiving, float value, bool disabled)
    {
        this.sourceNode = Source;
        this.receivingNode = Receiving;
        this.weight = value;
        this.disabled = disabled;
        this.id = Guid.NewGuid().ToString();
    }

    public int SourceNode
    {
        get { return sourceNode; }
        set { sourceNode = value; }
    }

    public int ReceivingNode
    {
        get { return receivingNode; }
        set { receivingNode = value; }
    }

    public float Weight
    {
        get { return weight; }
        set { weight = value; }
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
    public float TotalNodeValue
    {
        get { return totalnodeval; }
        set { totalnodeval = value; }
    }
}
