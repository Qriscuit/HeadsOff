using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;

public class Printing : MonoBehaviour
{
    [Header("ClassRefrence")]
    public WeaponManager _WM;
    public MasterInputManager _IM;

    [Header("PrintingChest")]
    public bool isPCactive;
    public bool isPrintingStarted=false;
    public bool isPrintedPartAvailable = false;
    public bool isPrintingAllowed=true;
    public float TimeToPrint;
    public float CooldownTime;


    [Header("Parts Available to print")]
    [SerializeField] LegType _Legs;
    [SerializeField] HandType _LeftHand;
    [SerializeField] HandType _RightHand;
    [SerializeField] ChestType _Chest;

    int PartToPrint;
    object PrintedPart;

    private void OnEnable()
    {
        //_IM._PrintingChest += PrintingChestInput;
    }

    public Vector2 ChestButton;
    void PrintingChestInput(Vector2 MouseMovementDir)
    {
        ChestButton = MouseMovementDir;
    }

    public void Launch()
    {
        if(isPrintedPartAvailable)
        {
            EquipPart();
            isPrintedPartAvailable = false;
            return;
        }

        if(isPrintingAllowed)
        {
            isPCactive = true;
            isPrintingAllowed = false;
        }
        
    }

    void EquipPart()
    {/*
        switch (PartToPrint)
        {
            case 1:
                _WM._BPM.BodyInPossession._chestType = _Chest;
                _WM._BPM.BodyInPossession.UpdateMesh();
                break;

            case 2:
                _WM._BPM.BodyInPossession.leftHandType = _LeftHand;
                _WM._BPM.BodyInPossession.UpdateMesh();
                break;

            case 3:
                _WM._BPM.BodyInPossession.legsType = _Legs;
                _WM._BPM.BodyInPossession.UpdateMesh();
                break;

            case 4:
                _WM._BPM.BodyInPossession.rightHandType = _RightHand;
                _WM._BPM.BodyInPossession.UpdateMesh();
                break;
        }*/
    }



    private void Update()
    {
        if(isPCactive && isPrintingStarted ==false && isPrintedPartAvailable==false)
        {
            if (_WM._BPM._MM.BodiesCloseBy.Count > 0) 
            {/*
                Body _Body = _WM._BPM._MM.BodiesCloseBy[0].GetComponent<Body>();
                _Legs = _Body.legsType;
                _LeftHand = _Body.leftHandType;
                _RightHand = _Body.rightHandType;
                _Chest = _Body.chestType;*/
            }

            if (ChestButton.y > 0)//1
            {
                PartToPrint = 1;
                StartCoroutine(StartPrinting(PartToPrint));
            }
            if (ChestButton.x < 0)//3
            {
                PartToPrint = 3;
                StartCoroutine(StartPrinting(PartToPrint));
            }
            if (ChestButton.y < 0)//2
            {
                PartToPrint = 2;
                StartCoroutine(StartPrinting(PartToPrint));
            }
            if (ChestButton.x > 0)//4
            {
                PartToPrint = 4;
                StartCoroutine(StartPrinting(PartToPrint));
            }
        }
    }

    public void DeLaunch()
    {
        isPCactive = false;
    }

    IEnumerator StartPrinting(int nbodyPart)
    {
        Debug.Log("printingstarted");
        isPrintingStarted = true;
        yield return new WaitForSeconds(TimeToPrint);
        switch(nbodyPart)
        {
            case 1:
                PrintedPart = _Chest;
                break;

            case 2:
                PrintedPart = _LeftHand;
                break;

            case 3:
                PrintedPart = _Legs;
                break;

            case 4:
                PrintedPart = _RightHand;
                break;
        }
        Debug.Log(PrintedPart + "printed");
        isPrintingStarted = false;
        isPrintedPartAvailable = true;
    }

    IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(CooldownTime);
        isPrintingAllowed = true;
    }
}
