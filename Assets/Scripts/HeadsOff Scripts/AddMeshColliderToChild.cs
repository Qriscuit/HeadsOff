using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMeshColliderToChild : MonoBehaviour
{
    public List<MeshCollider> colliders = new List<MeshCollider>();
    [ContextMenu("Add colliders to child")]
    void addCollider()
    {
        Transform[] colliders = this.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform c in colliders)
        {
           if(c.GetComponent<MeshCollider>())
            {
                
            }
           else
            {
                c.gameObject.AddComponent<MeshCollider>();
            }
        }
    }  
}



