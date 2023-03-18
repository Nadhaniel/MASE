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
            ToolTipSystem.instance.toolTip.contentField.text = attachedNode.NodeValue.ToString();
        }
    }
}
