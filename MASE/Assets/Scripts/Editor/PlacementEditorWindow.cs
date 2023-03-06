using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlacementGenerator))]
public class PlacementEditorWindow : Editor
{
    public override void OnInspectorGUI()
    {
        PlacementGenerator pg = (PlacementGenerator)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Place Plants"))
        {
            pg.Generate();
        }
    }
}
