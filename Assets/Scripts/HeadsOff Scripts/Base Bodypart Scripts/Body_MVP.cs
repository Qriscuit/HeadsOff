using System.Collections;
using System;
using UnityEngine;
using HeadsOffGlobals;
using Mirror;

// BODY HEALTH - 250
// HEAD HEALTH - 100

// BIGSHOT DAMAGE = 50


// Bodys health regenerates when not connected to the body


public class Body_MVP : NetworkBehaviour
{
    public bool TESTING = true;
    [Space]
    public GameObject Head;
    [Space]
    public Animator _Animator;

    #region Body Part properties

    public Chest _Chest;
    public Legs _Legs;
    public LeftHands _L_Hands;
    public RightHands _R_Hands;

    public ChestType chestType
    {
        get { return _Chest.Type; }
        set { _Chest.Type = value; }
    }

    public LegType legsType
    {
        get { return _Legs.Type; }
        set { _Legs.Type = value; }
    }

    public HandType rightHandType
    {
        get { return _R_Hands.Type; }
        set { _R_Hands.Type = value; }
    }

    public HandType leftHandType
    {
        get { return _L_Hands.Type; }
        set { _L_Hands.Type = value; }
    }

    #endregion
    #region update mesh through inspector

    [Space]

    [SerializeField] ChestType _ChestTypeInspector;
    [SerializeField] LegType _LegTypeInspector;
    [SerializeField] HandType _LeftHandTypeInspector;
    [SerializeField] HandType _RightHandTypeInspector;
    
    public void SetMeshProperty()
    {
        chestType= _ChestTypeInspector;
        legsType= _LegTypeInspector;
        leftHandType= _LeftHandTypeInspector;
        rightHandType= _RightHandTypeInspector;
    }

    #endregion

    [HideInInspector] public int BodyNumber = 0;
    
    private void Start()
    {
        RandomizeBodyPart();    
    }

    void RandomizeBodyPart()
    {
        chestType = (ChestType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(ChestType)).Length-1);
        legsType = (LegType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(LegType)).Length-1);
        leftHandType = (HandType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(HandType)).Length-1);
        rightHandType = (HandType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(HandType)).Length-1);
    }

    public void RandomizeBodyPart_RPCCaller(ChestType C, LegType L, HandType LH, HandType RH)
    {
        RPC_RandomizeParts(C, L, LH, RH);
    }

    public void RPC_RandomizeParts(ChestType C, LegType L, HandType LH, HandType RH)
    {
        chestType = C;
        legsType = L;
        leftHandType = LH;
        rightHandType = RH;
    }

    public void MeshUpdate()
    {
        UpdateMesh();
    }

    void UpdateMeshRPC()
    {
        _Chest.UpdateMesh();
        _Legs.UpdateMesh();
        _L_Hands.UpdateMesh();
        _R_Hands.UpdateMesh();
    }

    public void UpdateMesh()
    {
        _Chest.Type = chestType;
        _Legs.Type = legsType;
        _L_Hands.Type = leftHandType;
        _R_Hands.Type = rightHandType;

        _Chest.UpdateMesh();
        _Legs.UpdateMesh();
        _L_Hands.UpdateMesh();
        _R_Hands.UpdateMesh();
    }

    public void ResetJumpBooleans()
    {
        _Animator.SetBool("Jumping", false);
        _Animator.SetBool("Falling", false);
        _Animator.SetBool("Landed", false);
    }

    public int IndexForSpawnListing = 0;
    public void IndexSetRPCCaller(int I)
    {
        RPC_SetIndex(I);
    }

    public void RPC_SetIndex(int Index)
    {
        IndexForSpawnListing = Index;
    }

    bool FirstInitialize = true;
}
