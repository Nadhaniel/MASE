using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DetailedCapsule))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DetailedCapsule cap = (DetailedCapsule) target;
        if (GUILayout.Button("Add to front")) {
            cap.AddToFront();
        }
        if (GUILayout.Button("Add to back"))
        {
            cap.AddToFront();
        }
        if (GUILayout.Button("remove from front"))
        {
            cap.AddToFront();
        }
        if (GUILayout.Button("remove from back"))
        {
            cap.AddToFront();
        }
    }
}
