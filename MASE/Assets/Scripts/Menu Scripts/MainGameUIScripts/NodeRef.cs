using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeRef : MonoBehaviour
{
    public Node attachedNode;

    private void Update()
    {
        if (this.GetComponent<ToolTipTrigger>().selected)
        {
            if (ToolTipSystem.instance.toolTip.headerField.text != attachedNode.NodeName)
            {
                ToolTipSystem.instance.toolTip.headerField.text = attachedNode.NodeName;
            }
            if (attachedNode.NodeType == Node_Type.Hidden)
            {
                string content = "Node Activation Function: " + attachedNode.Activation_Type + " \tValue: " + attachedNode.NodeValue.ToString();
                ToolTipSystem.instance.toolTip.contentField.text = content;

            }
            else
            {
                ToolTipSystem.instance.toolTip.contentField.text = attachedNode.NodeValue.ToString();
            }
        }
    }
}
