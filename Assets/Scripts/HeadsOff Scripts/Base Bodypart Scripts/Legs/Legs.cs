using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;

public class Legs : MonoBehaviour
{
    public Body _Body;
    LegType _Type;
    public LegType Type
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

    public List<LegChild> AllLegsAvailable = new List<LegChild>();

    private void Awake()
    {
        foreach (Transform F in transform)
        {
            AllLegsAvailable.Add(F.GetComponent<LegChild>());
        }
    }

    public void UpdateMesh()
    {
        switch (_Type)
        {
            case LegType.BullRush:
                turnOneOnVsOtherOff(0);
                break;

            case LegType.Dash:
                turnOneOnVsOtherOff(1);
                break;

            case LegType.Stomp:
                turnOneOnVsOtherOff(2);
                break;

            case LegType.SuperJump:
                turnOneOnVsOtherOff(3);
                break;
        }
    }

    void turnOneOnVsOtherOff(int o)
    {
        for (int i = 0; i < AllLegsAvailable.Count; i++)
        {
            AllLegsAvailable[i].gameObject.SetActive(false);

            if (o == i)
            {
                AllLegsAvailable[o].gameObject.SetActive(true);
            }
        }
    }
}
