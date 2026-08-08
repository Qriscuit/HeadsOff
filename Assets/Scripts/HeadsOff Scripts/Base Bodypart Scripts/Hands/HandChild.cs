using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;
using System;

public class HandChild : MonoBehaviour
{
    public HandType _Type;

    public enum Hand { LH, RH };
    public Hand _Hand;

    public List<SkinnedMeshRenderer> _Renderers = new List<SkinnedMeshRenderer>();

    private void Awake()
    {
        string HandTYPE = name[0].ToString() + name[1].ToString();

        switch (HandTYPE)
        {
            case "LH":
                _Hand = Hand.LH;
                break;

            case "RH":
                _Hand = Hand.RH;
                break;
        }

        string WeaponType = name.ToString().Remove(0, 3);
        Enum.TryParse(WeaponType, out HandType myStatus);

        _Type = myStatus;

        foreach (Transform item in transform)
        {
            _Renderers.Add(item.GetComponent<SkinnedMeshRenderer>());
        }
    }
}
