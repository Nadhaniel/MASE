using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipSystem : MonoBehaviour
{
    public static ToolTipSystem instance;

    public ToolTip toolTip;

    public void Awake()
    {
        instance = this;
        instance.toolTip.gameObject.SetActive(false);
    }

    public static void Show()
    {
        instance.toolTip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        instance.toolTip.gameObject.SetActive(false);
    }
}
