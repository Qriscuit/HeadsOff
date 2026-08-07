using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;
using System;

public class ChestChild : MonoBehaviour
{
    public ChestType _Type;

    public List<SkinnedMeshRenderer> _Renderers = new List<SkinnedMeshRenderer>();

    private void Awake()
    {
        string WeaponType = name.ToString().Remove(0, 2);
        Enum.TryParse(WeaponType, out ChestType myStatus);

        _Type = myStatus;
        
        foreach (Transform item in transform)
        {
            _Renderers.Add(item.GetComponent<SkinnedMeshRenderer>()); 
        }
    }
}
