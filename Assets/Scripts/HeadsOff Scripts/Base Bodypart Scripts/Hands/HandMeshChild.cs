using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;

public class HandMeshChild : MonoBehaviour
{
    public HandType _Type;

    public List<SkinnedMeshRenderer> _Renderers = new List<SkinnedMeshRenderer>();

    private void Awake()
    {
        foreach (Transform item in transform)
        {
            _Renderers.Add(item.GetComponent<SkinnedMeshRenderer>());
        }
    }
}
