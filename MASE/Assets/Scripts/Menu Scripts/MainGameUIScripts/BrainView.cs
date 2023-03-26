using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using System.Linq;

public class BrainView : MonoBehaviour
{
    public static BrainView instance;
    public GameObject selectedCreature;
    public GameObject brainNodeImage;
    public GameObject synapseConnection;
    private List<GameObject> NodesImages;
    private List<GameObject> synapseConnections;
    private List<GameObject> Layers;
    private List<Node> InputNodes;
    private List<Node> HiddenNodes;
    private List<Node> OutputNodes;
    private List<Synapse> InputSynapses;

    public GameObject InputPanel;
    public GameObject OutputPanel;
    public GameObject LayerObj;
    public GameObject Layer;
    //public GameObject HiddenPanel;

    private Vector3 FirstNodePos;
    private Vector3 UpdatedNodePos;

    private void Start()
    {
        this.gameObject.SetActive(false);
        instance = this;
    }

    public void Init()
    {
        FirstNodePos = new Vector3(-370, 430);
        NodesImages = new List<GameObject>();
        synapseConnections = new List<GameObject>();
        InputSynapses = new List<Synapse>();
        selectedCreature = InfoGetter.instance.selectedCreature;
        //AssignNodesandSynapses();
        PlaceInputOutputNodes();
        PlaceSynapses();
    }

    public void PlaceInputOutputNodes()
    { 
        //for (int i = 0; i < selectedCreature.GetComponent<Creature>().brain.Network.Count; i++)
        //{
        //    if (selectedCreature.GetComponent<Creature>().brain.Network[i].NodeType == NodeTypes.Input)
        //    {
        //        NodesImages.Add(Instantiate(brainNodeImage, InputPanel.transform));
        //        NodesImages[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.ToString();
        //        NodesImages[i].GetComponent<NodeRef>().attachedNode = selectedCreature.GetComponent<Creature>().brain.Network[i];
        //    }
        //    else if (selectedCreature.GetComponent<Creature>().brain.Network[i].NodeType == NodeTypes.Output)
        //    {
        //        NodesImages.Add(Instantiate(brainNodeImage, OutputPanel.transform));
        //        NodesImages[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.ToString();
        //        NodesImages[i].GetComponent<NodeRef>().attachedNode = selectedCreature.GetComponent<Creature>().brain.Network[i];
        //    }
        //}
        //LayerPlacement();

        Layers = new List<GameObject>();
        foreach (var layer in selectedCreature.GetComponent<Creature>().brain.Network)
        {
            if (layer.Key.Item1 == 0)
            {
                for (int i = 0; i < selectedCreature.GetComponent<Creature>().brain.Network[layer.Key].Count; i++)
                {
                    NodesImages.Add(Instantiate(brainNodeImage, InputPanel.transform));
                    NodesImages.Last().GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.ToString();
                    NodesImages.Last().GetComponent<NodeRef>().attachedNode = selectedCreature.GetComponent<Creature>().brain.Network[layer.Key][i];
                }
            }
            if (layer.Key.Item1 > 0 && layer.Key.Item1 < 6 && layer.Key.Item2 == true)
            {
                Layers.Add(Instantiate(Layer, LayerObj.transform));
                for (int i = 0; i < selectedCreature.GetComponent<Creature>().brain.Network[layer.Key].Count; i++)
                {
                    NodesImages.Add(Instantiate(brainNodeImage, Layers.Last().transform));
                    NodesImages.Last().GetComponentInChildren<TMPro.TextMeshProUGUI>().text = selectedCreature.GetComponent<Creature>().brain.Network[layer.Key][i].NodeName;
                    NodesImages.Last().GetComponent<NodeRef>().attachedNode = selectedCreature.GetComponent<Creature>().brain.Network[layer.Key][i];
                }
            }
            if (layer.Key.Item1 == 6)
            {
                for (int i = 0; i < selectedCreature.GetComponent<Creature>().brain.Network[layer.Key].Count; i++)
                {
                    NodesImages.Add(Instantiate(brainNodeImage, OutputPanel.transform));
                    NodesImages.Last().GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.ToString();
                    NodesImages.Last().GetComponent<NodeRef>().attachedNode = selectedCreature.GetComponent<Creature>().brain.Network[layer.Key][i];
                }
            }
        }
    }

    public void LayerPlacement()
    {
        Layers = new List<GameObject>();
        int currentLayer = 0;
        List<Node> visited = new List<Node>();
        List<List<Node>> layers = new List<List<Node>>();
        if (HiddenNodes.Count >= 1)
        {
            Layers.Add(Instantiate(Layer, LayerObj.transform));
            layers.Add(new List<Node>());
            layers[0] = InputNodes;
            for (int i = 0; i < HiddenNodes.Count; i++)
            {
                List<Synapse> list = HiddenNodes[i].ReceivingSynapses.FindAll(x => x.SourceNode.NodeType == NodeTypes.Hidden);
                if (list.Count == 0)
                {
                    NodesImages.Add(Instantiate(brainNodeImage, Layers[0].transform));
                    NodesImages.Last().GetComponentInChildren<TMPro.TextMeshProUGUI>().text = HiddenNodes[i].NodeName;
                    NodesImages.Last().GetComponent<NodeRef>().attachedNode = HiddenNodes[i];
                    visited.Add(HiddenNodes[i]);
                    layers[0].Add(HiddenNodes[i]);
                }
            }
            while (visited.Count < HiddenNodes.Count)
            {
                layers.Add(new List<Node>());
                Layers.Add(Instantiate(Layer, LayerObj.transform));
                layers[currentLayer + 1].AddRange(layers[currentLayer]);
                List<Node> debug = HiddenNodes.FindAll(x => !visited.Contains(x));
                for (int i = 0; i < HiddenNodes.Count; i++)
                {
                    if (!visited.Contains(HiddenNodes[i]))
                    {
                        if (HiddenNodes[i].ReceivingSynapses.TrueForAll(r => layers[currentLayer].Contains(r.SourceNode)))
                        {
                            int nextLayer = currentLayer + 1;
                            layers[nextLayer].Add(HiddenNodes[i]);
                            NodesImages.Add(Instantiate(brainNodeImage, Layers[nextLayer].transform));
                            NodesImages.Last().GetComponentInChildren<TMPro.TextMeshProUGUI>().text = HiddenNodes[i].NodeName;
                            NodesImages.Last().GetComponent<NodeRef>().attachedNode = HiddenNodes[i];
                            visited.Add(HiddenNodes[i]);
                        }
                    }
                }
                currentLayer += 1;
            }
        }
    }

    public void PlaceSynapses()
    {
        StartCoroutine(CoWaitForPos());
    }

    IEnumerator CoWaitForPos()
    {
        for (int i = 0; i < NodesImages.Count; i++)
        {

            foreach (Synapse synapse in NodesImages[i].GetComponent<NodeRef>().attachedNode.SendingSynapses)
            {
                if (synapse.ReceivingNode.NodeType == NodeTypes.Output)
                {
                    RectTransform rect2 = NodesImages.Find(x => x.GetComponent<NodeRef>().attachedNode.ID == synapse.ReceivingNode.ID).GetComponent<RectTransform>();
                    Vector2[] PointsArr = new Vector2[2];
                    yield return new WaitForEndOfFrame();
                    PointsArr[0] = transform.InverseTransformPoint(NodesImages[i].GetComponent<RectTransform>().position);
                    PointsArr[1] = transform.InverseTransformPoint(rect2.position);
                    synapseConnections.Add(Instantiate(synapseConnection, this.transform));
                    synapseConnections.Last().GetComponent<SynapseRef>().attachedSynapse = synapse;
                    synapseConnections.Last().GetComponent<UILineRenderer>().Points = PointsArr;
                    synapseConnections.Last().transform.SetSiblingIndex(0);
                }
                else if (synapse.ReceivingNode.NodeType == NodeTypes.Hidden)
                {
                    RectTransform rect2 = NodesImages.Find(x => x.GetComponent<NodeRef>().attachedNode.ID == synapse.ReceivingNode.ID).GetComponent<RectTransform>();
                    Vector2[] PointsArr = new Vector2[2];
                    yield return new WaitForEndOfFrame();
                    PointsArr[0] = transform.InverseTransformPoint(NodesImages[i].GetComponent<RectTransform>().position);
                    PointsArr[1] = transform.InverseTransformPoint(rect2.position);
                    synapseConnections.Add(Instantiate(synapseConnection, this.transform));
                    synapseConnections.Last().GetComponent<SynapseRef>().attachedSynapse = synapse;
                    synapseConnections.Last().GetComponent<UILineRenderer>().Points = PointsArr;
                    synapseConnections.Last().transform.SetSiblingIndex(0);
                    foreach (Synapse sendingSynapse in synapse.ReceivingNode.SendingSynapses)
                    {
                        if (sendingSynapse.ReceivingNode.NodeType == NodeTypes.Output)
                        {
                            RectTransform rect = NodesImages.Find(x => x.GetComponent<NodeRef>().attachedNode.ID == sendingSynapse.ReceivingNode.ID).GetComponent<RectTransform>();
                            Vector2[] PointsArr2 = new Vector2[2];
                            PointsArr2[0] = transform.InverseTransformPoint(rect2.position);
                            PointsArr2[1] = transform.InverseTransformPoint(rect.position);
                            synapseConnections.Add(Instantiate(synapseConnection, this.transform));
                            synapseConnections.Last().GetComponent<SynapseRef>().attachedSynapse = sendingSynapse;
                            synapseConnections.Last().GetComponent<UILineRenderer>().Points = PointsArr2;
                            synapseConnections.Last().transform.SetSiblingIndex(0);
                        }
                    }
                }
            }
        }
        StopCoroutine(CoWaitForPos());
    }

    //public void AssignNodesandSynapses()
    //{
    //    InputNodes = new List<Node>();
    //    HiddenNodes = new List<Node>();
    //    OutputNodes = new List<Node>();

    //    for (int i = 0; i < selectedCreature.GetComponent<Creature>().brain.Network.Count; i++)
    //    {
    //        if (selectedCreature.GetComponent<Creature>().brain.Network[i].NodeType == NodeTypes.Input)
    //        {
    //            InputNodes.Add(selectedCreature.GetComponent<Creature>().brain.Network[i]);
    //        }
    //        if (selectedCreature.GetComponent<Creature>().brain.Network[i].NodeType == NodeTypes.Hidden)
    //        {
    //            HiddenNodes.Add(selectedCreature.GetComponent<Creature>().brain.Network[i]);
    //        }
    //        if (selectedCreature.GetComponent<Creature>().brain.Network[i].NodeType == NodeTypes.Output)
    //        {
    //            OutputNodes.Add(selectedCreature.GetComponent<Creature>().brain.Network[i]);
    //        }
    //    }
    //}

    public void HideShowPanel()
    {
        if (this.gameObject.activeSelf == false)
        {
            this.gameObject.SetActive(true);
            Init();
        }
        else
        {
            ClearBrain();
            this.gameObject.SetActive(false);
        }
    }

    public void ClearBrain()
    {
        foreach (Transform input in InputPanel.transform)
        {
            Destroy(input.gameObject);
        }
        foreach (Transform output in OutputPanel.transform)
        {
            Destroy(output.gameObject);
        }
        foreach (Transform child in LayerObj.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform synapse in this.transform)
        {
            if (synapse.gameObject.tag == "SynapseUI")
            {
                Destroy(synapse.gameObject);
            }
        }
    }
}
