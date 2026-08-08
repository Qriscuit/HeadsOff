using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Body))]
public class BodyCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Body _thisBody = (Body)target;

        if(GUILayout.Button("UpdateMesh"))
        {
           _thisBody.SetMeshProperty();
            Debug.Log("Tried updating");
        }
    }
}
