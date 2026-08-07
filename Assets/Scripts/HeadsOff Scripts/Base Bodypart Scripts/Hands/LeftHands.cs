using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;

public class LeftHands : MonoBehaviour
{
    public Body _Body;

    public HandType Type
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

    HandType _Type;
    
    public List<HandChild> AllHandsAvailable = new List<HandChild>();

    private void Awake()
    {
        foreach (Transform F in transform)
        {
            AllHandsAvailable.Add(F.GetComponent<HandChild>());
        }
    }

    public void UpdateMesh()
    {
        switch (_Type)
        {
            case HandType.BigShot:
                turnOneOnVsOtherOff(0);
                break;

            case HandType.BubbleGun:
                turnOneOnVsOtherOff(1);
                break;

            case HandType.ElectroBall:
                turnOneOnVsOtherOff(2);
                break;

            case HandType.ExplodingMines:
                turnOneOnVsOtherOff(3);
                break;

            case HandType.FeviTop:
                turnOneOnVsOtherOff(4);
                break;

            case HandType.FlameThrower:
                turnOneOnVsOtherOff(5);
                break;

            case HandType.Latch:
                turnOneOnVsOtherOff(6);
                break;

            case HandType.ParticleAccelerator:
                turnOneOnVsOtherOff(7);
                break;

            case HandType.PortalGun:
                turnOneOnVsOtherOff(8);
                break;

            case HandType.PunchGlove:
                turnOneOnVsOtherOff(9);
                break;
        }
    }

    void turnOneOnVsOtherOff(int o)
    {
        for (int i = 0; i < AllHandsAvailable.Count; i++)
        {
            AllHandsAvailable[i].gameObject.SetActive(false);

            if (o == i)
            {
                AllHandsAvailable[o].gameObject.SetActive(true);
            }
        }
    }
}
