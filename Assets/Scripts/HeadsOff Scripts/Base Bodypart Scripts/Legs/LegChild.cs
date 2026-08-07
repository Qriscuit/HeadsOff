using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;
using System;

public class LegChild : MonoBehaviour
{
    public LegType _Type;

    public List<SkinnedMeshRenderer> _Renderers = new List<SkinnedMeshRenderer>();

    private void Start()
    {
        string WeaponType = name.ToString().Remove(0, 2);
        Enum.TryParse(WeaponType, out LegType myStatus);

        _Type = myStatus;

        foreach (Transform item in transform)
        {
            _Renderers.Add(item.GetComponent<SkinnedMeshRenderer>());
        }
    }
}
