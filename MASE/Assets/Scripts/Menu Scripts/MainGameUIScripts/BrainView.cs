using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

public class BrainView : MonoBehaviour
{
    public static BrainView instance;
    public GameObject selectedCreature;
    public GameObject brainNodeImage;
    public GameObject synapseConnection;
    private List<GameObject> NodesImages;
    private List<GameObject> synapseConnections;
    private List<Node> InputNodes;
    private List<Node> HiddenNodes;
    private List<Node> OutputNodes;
    private List<Synapse> InputSynapses;

    public GameObject InputPanel;
    public GameObject OutputPanel;

    private Vector3 FirstNodePos;
    private Vector3 UpdatedNodePos;

    private Vector2 origin;
    private Vector2 end;

    private void Start()
    {
        this.gameObject.SetActive(false);
        Canvas.ForceUpdateCanvases();
        instance = this;
    }

    public void Init()
    {
        FirstNodePos = new Vector3(-370, 430);
        NodesImages = new List<GameObject>();
        synapseConnections = new List<GameObject>();
        InputSynapses = new List<Synapse>();
        selectedCreature = InfoGetter.instance.selectedCreature;
        AssignNodesandSynapses();
        PlaceInputOutputNodes();
        PlaceSynapses();
    }

    public void PlaceInputOutputNodes()
    {
        UpdatedNodePos = FirstNodePos;
        for (int i = 0; i < InputNodes.Count; i++)
        {
            NodesImages.Add(Instantiate(brainNodeImage, InputPanel.transform));
            NodesImages[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.ToString();
            NodesImages[i].GetComponent<NodeRef>().attachedNode = InputNodes[i];
        }
        UpdatedNodePos = new Vector3(370, 430);
        for (int i = 0; i < OutputNodes.Count; i++)
        {
            NodesImages.Add(Instantiate(brainNodeImage, OutputPanel.transform));
            NodesImages[InputNodes.Count + i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text = i.ToString();
            NodesImages[InputNodes.Count + i].GetComponent<NodeRef>().attachedNode = OutputNodes[i];
        }
    }

    public void PlaceSynapses()
    {
        Vector2[] PointsArr = new Vector2[2];
        for (int i = 0; i < NodesImages.Count; i++)
        {
            if (NodesImages[i].GetComponent<NodeRef>().attachedNode.NodeType == NodeTypes.Input)
            {
                for (int s = 0; s < NodesImages[i].GetComponent<NodeRef>().attachedNode.SendingSynapses.Count; s++)
                {
                    if (NodesImages[i].GetComponent<NodeRef>().attachedNode.SendingSynapses[s].ReceivingNode.NodeType == NodeTypes.Output)
                    {
                        StartCoroutine(CoWaitForPos(NodesImages[i].GetComponent<RectTransform>(), NodesImages.Find(x => x.GetComponent<NodeRef>().attachedNode.ID == NodesImages[i].GetComponent<NodeRef>().attachedNode.SendingSynapses[s].ReceivingNode.ID).GetComponent<RectTransform>()));
                        PointsArr[0] = origin;
                        PointsArr[1] = end;

                        synapseConnections.Add(Instantiate(synapseConnection, this.transform));
                        synapseConnections[s].transform.SetSiblingIndex(0);
                        synapseConnections[s].GetComponent<SynapseRef>().attachedSynapse = NodesImages[i].GetComponent<NodeRef>().attachedNode.SendingSynapses[s];
                        synapseConnections[s].GetComponent<UILineRenderer>().Points = PointsArr;
                    }
                }
            }
        }
    }

    IEnumerator CoWaitForPos(RectTransform recttrans1, RectTransform recttrans2)
    {
        yield return new WaitForEndOfFrame();
        origin = transform.InverseTransformPoint(recttrans1.position);
        end = transform.InverseTransformPoint(recttrans2.position);
        StopCoroutine("CoWaitForPos");
    }

    public void AssignNodesandSynapses()
    {
        InputNodes = new List<Node>();
        HiddenNodes = new List<Node>();
        OutputNodes = new List<Node>();

        for (int i = 0; i < selectedCreature.GetComponent<Creature>().brain.Network.Count; i++)
        {
            if (selectedCreature.GetComponent<Creature>().brain.Network[i].NodeType == NodeTypes.Input)
            {
                InputNodes.Add(selectedCreature.GetComponent<Creature>().brain.Network[i]);
            }
            if (selectedCreature.GetComponent<Creature>().brain.Network[i].NodeType == NodeTypes.Hidden)
            {
                HiddenNodes.Add(selectedCreature.GetComponent<Creature>().brain.Network[i]);
            }
            if (selectedCreature.GetComponent<Creature>().brain.Network[i].NodeType == NodeTypes.Output)
            {
                OutputNodes.Add(selectedCreature.GetComponent<Creature>().brain.Network[i]);
            }
        }
    }

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
        foreach (Transform synapse in this.transform)
        {
            if (synapse.gameObject.tag == "SynapseUI")
            {
                Destroy(synapse.gameObject);
            }
        }
    }
}
