using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NonNetworkedBody))]
public class NNBCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NonNetworkedBody _thisBody = (NonNetworkedBody)target;

        if (GUILayout.Button("UpdateMesh"))
        {
            _thisBody.SetMeshProperty();
            Debug.Log("Tried updating");
        }
    }
}
