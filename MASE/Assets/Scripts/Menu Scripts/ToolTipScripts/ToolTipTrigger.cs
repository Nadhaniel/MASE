using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool selected;
    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipSystem.Show();
        selected = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipSystem.Hide();
        selected = false;
    }
}
