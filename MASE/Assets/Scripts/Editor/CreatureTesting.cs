using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CreatureJobMove))]
public class CreatureTesting : Editor
{
    public override void OnInspectorGUI()
    {
        CreatureJobMove creature = (CreatureJobMove)target;

        DrawDefaultInspector();
        if (GUILayout.Button("Mutate"))
        {
            creature.brain.MutateNN(1);
        }
    }
}
