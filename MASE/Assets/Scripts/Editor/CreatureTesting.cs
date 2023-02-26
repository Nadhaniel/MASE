using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Creature))]
public class CreatureTesting : Editor
{
    public override void OnInspectorGUI()
    {
        Creature creature = (Creature)target;

        if (GUILayout.Button("Mutate"))
        {
            creature.brain.MutateBrain();
        }
    }
}
