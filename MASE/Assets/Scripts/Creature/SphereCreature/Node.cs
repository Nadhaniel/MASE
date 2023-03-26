using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node
{
    private List<Synapse> receivingSynapses = new List<Synapse>();
    private List<Synapse> sendingSynapses = new List<Synapse>();
    private float nodeValue = 0;

    private NodeTypes nodeType;
    private NodeTypes hiddenNodeType;
    private int index;
    private int layer;
    private string id;
    private string nodeName;
    public Node(NodeTypes nodetype, float value, string name, int index, int layer)
    {
        this.nodeType = nodetype;
        this.nodeValue = value;
        this.nodeName = name;
        this.index = index;
        this.id = Guid.NewGuid().ToString();
        this.layer = layer;
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

    public string NodeName
    {
        get { return nodeName; }
        set { nodeName = value; }
    }

    public int Layer
    {
        get { return layer; }
        set { layer = value; }
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
