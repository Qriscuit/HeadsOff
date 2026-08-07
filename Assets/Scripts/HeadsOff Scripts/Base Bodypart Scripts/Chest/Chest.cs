using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;

public class Chest : MonoBehaviour
{
    public Body _Body;
    ChestType _Type;

    public ChestType Type
    {
        get
        {
            return _Type;
        }

        set
        {
            _Type = value;
            UpdateMesh();
        }
    }

    public List<ChestChild> AllChestsAvailable = new List<ChestChild>();

    private void Awake()
    {
        foreach(Transform F in transform)
        {
            AllChestsAvailable.Add(F.GetComponent<ChestChild>());
        }
    }

    public void UpdateMesh()
    {
        switch (_Type)
        {
            case ChestType.JetPack:
                turnOneOnVsOtherOff(0);
                break;

            case ChestType.Printing:
                turnOneOnVsOtherOff(1);
                break;

            case ChestType.PumpUp:
                turnOneOnVsOtherOff(2);
                break;

            case ChestType.Shield:
                turnOneOnVsOtherOff(3);
                break;
        }
    }

    void turnOneOnVsOtherOff(int o)
    {
        for (int i = 0; i < AllChestsAvailable.Count; i++)
        {
            AllChestsAvailable[i].gameObject.SetActive(false);

            if (o == i)
            {
                AllChestsAvailable[o].gameObject.SetActive(true);
            }
        }
    }
}
