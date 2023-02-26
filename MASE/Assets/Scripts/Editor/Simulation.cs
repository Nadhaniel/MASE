using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimulationManager))]
public class Simulation : Editor
{
    public override void OnInspectorGUI()
    {
        SimulationManager sim = (SimulationManager)target;

        if (DrawDefaultInspector())
        {
            
        }
        if (GUILayout.Button("Spawn"))
        {
            sim.Spawn();
        }
    }
}
