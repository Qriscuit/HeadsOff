using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;

public class ChestMeshChild : MonoBehaviour
{
    public ChestType _Type;

    public List<SkinnedMeshRenderer> _Renderers = new List<SkinnedMeshRenderer>();

    private void Awake()
    {
        foreach (Transform item in transform)
        {
            _Renderers.Add(item.GetComponent<SkinnedMeshRenderer>()); 
        }
    }
}
