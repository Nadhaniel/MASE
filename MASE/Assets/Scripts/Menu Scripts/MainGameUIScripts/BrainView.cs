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
    private NNGenome Genome;

    public GameObject InputPanel;
    public GameObject OutputPanel;
    public GameObject HiddenPanel;

    private void Start()
    {
        this.gameObject.SetActive(false);
        instance = this;
    }

    public void Init()
    {
        NodesImages = new List<GameObject>();
        synapseConnections = new List<GameObject>();
        selectedCreature = InfoGetter.instance.selectedCreature;
        Genome = selectedCreature.GetComponent<Creature>().brain.NN_genome;
        PlaceInputOutputNodes();
        PlaceSynapses();
    }

    public void PlaceInputOutputNodes()
    { 
        for (int i = 0; i < selectedCreature.GetComponent<Creature>().brain.inputNodes.Count; i++)
        {
            NodesImages.Add(Instantiate(brainNodeImage, InputPanel.transform));
            NodesImages.Last().GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.ToString();
            NodesImages.Last().GetComponent<NodeRef>().attachedNode = selectedCreature.GetComponent<Creature>().brain.inputNodes[i];
        }
        for (int i = 0; i < selectedCreature.GetComponent<Creature>().brain.outputNodes.Count; i++)
        {
            NodesImages.Add(Instantiate(brainNodeImage, OutputPanel.transform));
            NodesImages.Last().GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.ToString();
            NodesImages.Last().GetComponent<NodeRef>().attachedNode = selectedCreature.GetComponent<Creature>().brain.outputNodes[i];
        }
    }


    public void PlaceSynapses()
    {
        //StartCoroutine(CoWaitForPos());
    }
    //IEnumerator CoWaitForPos()
    //{
    //    for (int i = 0; i < NodesImages.Count; i++)
    //    {
    //        foreach (ConnectionGene gene in Genome.connectionGenes)
    //        {
    //            if (NodesImages[i].GetComponent<NodeRef>().attachedNode.Index == gene.SourceNode)
    //            {
    //                RectTransform rect2 = NodesImages.Find(x => x.GetComponent<NodeRef>().attachedNode.Index == gene.ReceivingNode).GetComponent<RectTransform>();
    //                yield return new WaitForEndOfFrame();
    //            }
    //        }
    //    }

    //    //for (int i = 0; i < Genome.connectionGenes.Count; i++)
    //    //{
            
    //    //    foreach (ConnectionGene conngene in ConnectionGenes)
    //    //    {
    //    //        if (synapse.ReceivingNode. == Node_Activations.Output)
    //    //        {
    //    //            RectTransform rect2 = NodesImages.Find(x => x.GetComponent<NodeRef>().attachedNode.ID == synapse.ReceivingNode.ID).GetComponent<RectTransform>();
    //    //            Vector2[] PointsArr = new Vector2[2];
    //    //            yield return new WaitForEndOfFrame();
    //    //            PointsArr[0] = transform.InverseTransformPoint(NodesImages[i].GetComponent<RectTransform>().position);
    //    //            PointsArr[1] = transform.InverseTransformPoint(rect2.position);
    //    //            synapseConnections.Add(Instantiate(synapseConnection, this.transform));
    //    //            synapseConnections.Last().GetComponent<SynapseRef>().attachedSynapse = synapse;
    //    //            synapseConnections.Last().GetComponent<UILineRenderer>().Points = PointsArr;
    //    //            synapseConnections.Last().transform.SetSiblingIndex(0);
    //    //        }
    //    //        else if (synapse.ReceivingNode.NodeType == Node_Activations.Hidden)
    //    //        {
    //    //            RectTransform rect2 = NodesImages.Find(x => x.GetComponent<NodeRef>().attachedNode.ID == synapse.ReceivingNode.ID).GetComponent<RectTransform>();
    //    //            Vector2[] PointsArr = new Vector2[2];
    //    //            yield return new WaitForEndOfFrame();
    //    //            PointsArr[0] = transform.InverseTransformPoint(NodesImages[i].GetComponent<RectTransform>().position);
    //    //            PointsArr[1] = transform.InverseTransformPoint(rect2.position);
    //    //            synapseConnections.Add(Instantiate(synapseConnection, this.transform));
    //    //            synapseConnections.Last().GetComponent<SynapseRef>().attachedSynapse = synapse;
    //    //            synapseConnections.Last().GetComponent<UILineRenderer>().Points = PointsArr;
    //    //            synapseConnections.Last().transform.SetSiblingIndex(0);
    //    //            foreach (Synapse sendingSynapse in synapse.ReceivingNode.SendingSynapses)
    //    //            {
    //    //                if (sendingSynapse.ReceivingNode.NodeType == Node_Activations.Output)
    //    //                {
    //    //                    RectTransform rect = NodesImages.Find(x => x.GetComponent<NodeRef>().attachedNode.ID == sendingSynapse.ReceivingNode.ID).GetComponent<RectTransform>();
    //    //                    Vector2[] PointsArr2 = new Vector2[2];
    //    //                    PointsArr2[0] = transform.InverseTransformPoint(rect2.position);
    //    //                    PointsArr2[1] = transform.InverseTransformPoint(rect.position);
    //    //                    synapseConnections.Add(Instantiate(synapseConnection, this.transform));
    //    //                    synapseConnections.Last().GetComponent<SynapseRef>().attachedSynapse = sendingSynapse;
    //    //                    synapseConnections.Last().GetComponent<UILineRenderer>().Points = PointsArr2;
    //    //                    synapseConnections.Last().transform.SetSiblingIndex(0);
    //    //                }
    //    //            }
    //    //        }
    //    //    }
    //    //}
    //    StopCoroutine(CoWaitForPos());
    //}

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
        foreach (Transform child in HiddenPanel.transform)
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
