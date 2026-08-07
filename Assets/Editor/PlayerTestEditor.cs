using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UI_Manager))]
public class PlayerTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UI_Manager _UI = (UI_Manager)target;

        if (GUILayout.Button("Damage"))
        {
            _UI.MakeDamageCrossHairAppear();
        }

        if (GUILayout.Button("KilledAPlayer"))
        {
            _UI.MakeDamageCrossHairAppear();
        }
    }
}
