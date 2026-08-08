using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;
using Mirror;

public class Body : NetworkBehaviour
{
    public GameObject Head;
    public Team BodyTeam = Team.NA;

    public bool IsBeingUsedByAHead
    {
        get
        {
            if (PlayerBodyIsAttachedTo != null)
                return false;
            return true;
        }
    }

    public Animator _Animator;
    public BoxCollider _boxCollider;
    public BasePlayerManager PlayerBodyIsAttachedTo;
    public Vector3 StartPosition;
    public Quaternion StartRotation;

    [Space]

    public GameObject _PumpUpAccents;
    public GameObject _ShieldAccents;

    [Space]

    public GameObject _LHParticleAcceleratorAccents;
    public GameObject _LHExplodingMinesAccents;
    public GameObject _LHPunchGloveAccents;

    [Space]

    public GameObject _RHParticleAcceleratorAccents;
    public GameObject _RHExplodingMinesAccents;
    public GameObject _RHPunchGloveAccents;

    [Space]

    Animator _PumpUpAccentsAnimator;
    Animator _ShieldAccentsAnimator;

    Animator _LHParticleAcceleratorAccentsAnimator;
    Animator _LHExplodingMinesAccentsAnimator;
    
    Animator _RHParticleAcceleratorAccentsAnimator;
    Animator _RHExplodingMinesAccentsAnimator;
        
    [Header("Scriptable Objects")]
    public SO_Chest[] _ChestSO;
    public SO_Legs[] _LegsSO;
    public SO_LeftHand[] _LeftHandSO;
    public SO_RightHand[] _RightHandSO;

    [Header("Renderer")]
    public SkinnedMeshRenderer HeadRenderer;
    public SkinnedMeshRenderer ChestRenderer;
    public SkinnedMeshRenderer AbdomenRenderer;
    public SkinnedMeshRenderer LeftHandRenderer;
    public SkinnedMeshRenderer RightHandRenderer;
    public SkinnedMeshRenderer LegRenderer;
    
    [Header("Type")]
    [SerializeField] ChestType _ChestTypeInspector;
    [SerializeField] LegType _LegTypeInspector;
    [SerializeField] HandType _LeftHandTypeInspector;
    [SerializeField] HandType _RightHandTypeInspector;
    


    //[SyncVar(hook = nameof(UpdateOccupancy))]
    //public BodyOccupancy _Occupancy;
    //void UpdateOccupancy()
    //{

    //}

    [SyncVar(hook = (nameof(UpdateBodyHealth)))]
    public float _BodyHealth = 250;
    public void UpdateBodyHealth(float OldValue, float NewValue)
    {
        if(NewValue < 0)
        {
            Debug.Log("Im Dead");
        }
    }

    [Space]

    [SyncVar(hook = nameof(HOOK_UpdateChestMesh))]
    public ChestType chestType;
    void HOOK_UpdateChestMesh(ChestType OldType, ChestType NewType) => UpdateChestMesh();
    private void UpdateChestMesh()
    {
        int i = (int)chestType;
        _ChestTypeInspector = chestType;

        if (IsBeingUsedByAHead)
        {
            if (chestType == ChestType.PumpUp)
            {
                _PumpUpAccents.SetActive(true);
            }
            else _PumpUpAccents.SetActive(false);
            if (chestType == ChestType.Shield)
            {
                _ShieldAccents.SetActive(true);
            }
            else _ShieldAccents.SetActive(false);
        }

        //if (i == 0) Debug.Log(_ChestSO[i].Mesh);
        ChestRenderer.sharedMesh = _ChestSO[i].Mesh;
        ChestRenderer.sharedMaterials = new Material[_ChestSO[i].GreyMaterials.Count];
        ChestRenderer.sharedMaterials = _ChestSO[i].GreyMaterials.ToArray();

        //switch (BodyTeam)
        //{
        //    case Team.Red:
        //        ChestRenderer.sharedMaterials = new Material[_ChestSO[i].RedMaterials.Count];
        //        ChestRenderer.sharedMaterials = _ChestSO[i].RedMaterials.ToArray();
        //        break;

        //    case Team.Blue:
        //        ChestRenderer.sharedMaterials = new Material[_ChestSO[i].BlueMaterials.Count];
        //        ChestRenderer.sharedMaterials = _ChestSO[i].BlueMaterials.ToArray();
        //        break;

        //    case Team.NA:
        //        ChestRenderer.sharedMaterials = new Material[_ChestSO[i].GreyMaterials.Count];
        //        ChestRenderer.sharedMaterials = _ChestSO[i].GreyMaterials.ToArray();
        //        break;
        //}
    }

    [SyncVar(hook = nameof(HOOK_UpdateLegMesh))]
    public LegType legsType;
    void HOOK_UpdateLegMesh(LegType OldType, LegType NewType) => UpdateLegMesh();
    private void UpdateLegMesh()
    {
        int i = (int)legsType;
        _LegTypeInspector = legsType;
        
        LegRenderer.sharedMesh = _LegsSO[i].Mesh;
        LegRenderer.sharedMaterials = new Material[_LegsSO[i].GreyMaterials.Count];
        LegRenderer.sharedMaterials = _LegsSO[i].GreyMaterials.ToArray();

        //switch (BodyTeam)
        //{
        //    case Team.Red:
        //        LegRenderer.sharedMaterials = new Material[_LegsSO[i].RedMaterials.Count];
        //        LegRenderer.sharedMaterials = _LegsSO[i].RedMaterials.ToArray();
        //        break;

        //    case Team.Blue:
        //        LegRenderer.sharedMaterials = new Material[_LegsSO[i].BlueMaterials.Count];
        //        LegRenderer.sharedMaterials = _LegsSO[i].BlueMaterials.ToArray();
        //        break;

        //    case Team.NA:
        //        LegRenderer.sharedMaterials = new Material[_LegsSO[i].GreyMaterials.Count];
        //        LegRenderer.sharedMaterials = _LegsSO[i].GreyMaterials.ToArray();
        //        break;
        //}
    }
    
    [SyncVar(hook = nameof(HOOK_UpdateLeftHandMesh))]
    public HandType leftHandType;
    void HOOK_UpdateLeftHandMesh(HandType OldType, HandType NewType) => UpdateLeftHandMesh();
    private void UpdateLeftHandMesh()
    {
        int i = (int)leftHandType;
        _LeftHandTypeInspector = leftHandType;

        if (IsBeingUsedByAHead)
        {
            if (leftHandType == HandType.ExplodingMines)
            {
                _LHExplodingMinesAccents.SetActive(true);
            }
            else _LHExplodingMinesAccents.SetActive(false);
            if (leftHandType == HandType.ParticleAccelerator)
            {
                _LHParticleAcceleratorAccents.SetActive(true);
            }
            else _LHParticleAcceleratorAccents.SetActive(false);
            if (leftHandType == HandType.PunchGlove)
            {
                _LHPunchGloveAccents.SetActive(true);
            }
            else _LHPunchGloveAccents.SetActive(false);
        } 
        
        LeftHandRenderer.sharedMesh = _LeftHandSO[i].Mesh;
        LeftHandRenderer.sharedMaterials = new Material[_LeftHandSO[i].GreyMaterials.Count];
        LeftHandRenderer.sharedMaterials = _LeftHandSO[i].GreyMaterials.ToArray();

        //switch (BodyTeam)
        //{
        //    case Team.Red:
        //        LeftHandRenderer.sharedMaterials = new Material[_LeftHandSO[i].RedMaterials.Count];
        //        LeftHandRenderer.sharedMaterials = _LeftHandSO[i].RedMaterials.ToArray();
        //        break;

        //    case Team.Blue:
        //        LeftHandRenderer.sharedMaterials = new Material[_LeftHandSO[i].BlueMaterials.Count];
        //        LeftHandRenderer.sharedMaterials = _LeftHandSO[i].BlueMaterials.ToArray();
        //        break;

        //    case Team.NA:
        //        LeftHandRenderer.sharedMaterials = new Material[_LeftHandSO[i].GreyMaterials.Count];
        //        LeftHandRenderer.sharedMaterials = _LeftHandSO[i].GreyMaterials.ToArray();
        //        break;
        //}
    }
    
    [SyncVar(hook = nameof(HOOK_UpdateRightHandMesh))]
    public HandType rightHandType;
    void HOOK_UpdateRightHandMesh(HandType OldType, HandType NewType) => UpdateRightHandMesh();
    private void UpdateRightHandMesh()
    {
        int i = (int)rightHandType;
        _RightHandTypeInspector = rightHandType;

        if (IsBeingUsedByAHead)
        {
            if (rightHandType == HandType.ExplodingMines)
            {
                _RHExplodingMinesAccents.SetActive(true);
            }
            else _RHExplodingMinesAccents.SetActive(false);
            if (rightHandType == HandType.ParticleAccelerator)
            {
                _RHParticleAcceleratorAccents.SetActive(true);
            }
            else _RHParticleAcceleratorAccents.SetActive(false);
            if (rightHandType == HandType.PunchGlove)
            {
                _RHPunchGloveAccents.SetActive(true);
            }
            else _RHPunchGloveAccents.SetActive(false);
        }
        RightHandRenderer.sharedMesh = _RightHandSO[i].Mesh;
        RightHandRenderer.sharedMaterials = new Material[_RightHandSO[i].GreyMaterials.Count];
        RightHandRenderer.sharedMaterials = _RightHandSO[i].GreyMaterials.ToArray();

        //switch (BodyTeam)
        //{
        //    case Team.Red:
        //        RightHandRenderer.sharedMaterials = new Material[_RightHandSO[i].RedMaterials.Count];
        //        RightHandRenderer.sharedMaterials = _RightHandSO[i].RedMaterials.ToArray();
        //        break;

        //    case Team.Blue:
        //        RightHandRenderer.sharedMaterials = new Material[_RightHandSO[i].BlueMaterials.Count];
        //        RightHandRenderer.sharedMaterials = _RightHandSO[i].BlueMaterials.ToArray();
        //        break;

        //    case Team.NA:
        //        RightHandRenderer.sharedMaterials = new Material[_RightHandSO[i].GreyMaterials.Count];
        //        RightHandRenderer.sharedMaterials = _RightHandSO[i].GreyMaterials.ToArray();
        //        break;
        //}
    }
    
    public void SetMeshProperty()
    {
        chestType = _ChestTypeInspector;
        legsType = _LegTypeInspector;
        leftHandType = _LeftHandTypeInspector;
        rightHandType = _RightHandTypeInspector;

        UpdateMesh();
    }
    
    private void Start()
    {
        _Animator = GetComponent<Animator>();
        StartPosition = transform.position;
        StartRotation = transform.rotation;
        if (isServer)
        {
            RandomizeBodyParts();
        } 
    }

    [ClientRpc]
    void CLNT_UpdateMesh()
    {
        //Debug.Log("In client callback");
        UpdateChestMesh();
        UpdateLeftHandMesh();
        UpdateRightHandMesh();
        UpdateLegMesh();
    }
    public void UpdateMesh()
    {

        UpdateChestMesh();
        UpdateLeftHandMesh();
        UpdateRightHandMesh();
        UpdateLegMesh();
    }
    
    public void TurnOffAccentsNVfx()
    {
        _PumpUpAccents.SetActive(false);
        _ShieldAccents.SetActive(false);
        _LHExplodingMinesAccents.SetActive(false);
        _LHParticleAcceleratorAccents.SetActive(false);
        _LHPunchGloveAccents.SetActive(false);
        _RHExplodingMinesAccents.SetActive(false);
        _RHParticleAcceleratorAccents.SetActive(false);
        _RHPunchGloveAccents.SetActive(false);
    }

    public void TurnOnAccentsNVfx()
    {
        if (leftHandType == HandType.ExplodingMines)
        {
            _LHExplodingMinesAccents.SetActive(true);
        }
        else _LHExplodingMinesAccents.SetActive(false);
        if (leftHandType == HandType.ParticleAccelerator)
        {
            _LHParticleAcceleratorAccents.SetActive(true);
        }
        else _LHParticleAcceleratorAccents.SetActive(false);
        if (leftHandType == HandType.PunchGlove)
        {
            _LHPunchGloveAccents.SetActive(true);
        }
        else _LHPunchGloveAccents.SetActive(false);

        if (rightHandType == HandType.ExplodingMines)
        {
            _RHExplodingMinesAccents.SetActive(true);
        }
        else _RHExplodingMinesAccents.SetActive(false);
        if (rightHandType == HandType.ParticleAccelerator)
        {
            _RHParticleAcceleratorAccents.SetActive(true);
        }
        else _RHParticleAcceleratorAccents.SetActive(false);
        if (rightHandType == HandType.PunchGlove)
        {
            _RHPunchGloveAccents.SetActive(true);
        }
        else _RHPunchGloveAccents.SetActive(false);

        if (chestType == ChestType.PumpUp)
        {
            _PumpUpAccents.SetActive(true);
        }
        else _PumpUpAccents.SetActive(false);
        if (chestType == ChestType.Shield)
        {
            _ShieldAccents.SetActive(true);
        }
        else _ShieldAccents.SetActive(false);
    }
    
    [Tooltip("put 69 for no bias")] [SerializeField] int ChestBias;
    [Tooltip("put 69 for no bias")] [SerializeField] int LegBias;
    [Tooltip("put 69 for no bias")] [SerializeField] int LeftHandBias;
    [Tooltip("put 69 for no bias")] [SerializeField] int RightHandBias;

    void RandomizeBodyParts()
    {
        //chest
        int i = ChestRandomizer();
        if (ChestBias != 69)
            i = ChestBias;

        chestType = _ChestSO[i].Name;

        //legs
        i = Random.Range(0, _LegsSO.Length);
        if (LegBias != 69)
            i = LegBias;

        legsType = _LegsSO[i].Name;

        //LeftHand
        i = HandRandomizer();
        if (LeftHandBias != 69)
            i = LeftHandBias;

        leftHandType = _LeftHandSO[i].Name;

        //RightHand
        i = HandRandomizer();
        if (RightHandBias != 69)
            i = RightHandBias;

        rightHandType = _RightHandSO[i].Name;


        CLNT_UpdateMesh();
    }

    int ChestRandomizer()
    {
        int i;
        i = Random.Range(0, _ChestSO.Length);
        while (i == 1)
        {
            i = Random.Range(0, _ChestSO.Length);
        }

        return i;
    }

    int HandRandomizer()
    {
        int i;
        i = Random.Range(0, _LeftHandSO.Length);
        while (i==6||i==4)
        {
            i = Random.Range(0, _LeftHandSO.Length);
        }

        return i;
    }

    public void ResetJumpBooleans()
    {
        _Animator.SetBool("Jumping", false);
        _Animator.SetBool("Falling", false);
        _Animator.SetBool("Landed", false);
    }

    

    [ContextMenu("Transfer Material from left to right hand")]
    void TransferMaterial()
    {
        for (int i = 0; i < _LeftHandSO.Length; i++)
        {
            foreach (Material m in _LeftHandSO[i].RedMaterials)
            {
                _RightHandSO[i].RedMaterials.Add(m);
            }

            foreach (Material m in _LeftHandSO[i].BlueMaterials)
            {
                _RightHandSO[i].BlueMaterials.Add(m);
            }
        }
    }

    
    private void Update()
    {
        if (isLocalPlayer)
        {
            if( _BodyHealth <= 0)
            {
                //PlayerAttachedRespawn();
            }
        }   
    }


    void PlayerAttachedRespawn()
    {
        //Debug.Log("leaving body");

        //gameObject.tag = "Unoccupied";
        
        //PlayerBodyIsAttachedTo = null;

        //netIdentity.RemoveClientAuthority();

        ////_BPM.BodyInPossession.transform.position = transform.position;
        //CLNT_LeaveBodyCallBack();
    }

    [Command]
    public void CMD_SwitchChest(GameObject BodyWeAreChangingWith)
    {
        ChestType NetBody = BodyWeAreChangingWith.GetComponent<Body>().chestType;
        ChestType ChestChange = chestType;
        chestType = NetBody;
        BodyWeAreChangingWith.GetComponent<Body>().chestType = ChestChange;
    }

    [Command]
    public void CMD_SwitchLeftHand(GameObject BodyWeAreChangingWith)
    {
        HandType NetBody = BodyWeAreChangingWith.GetComponent<Body>().leftHandType;
        HandType LHChange = leftHandType;
        leftHandType = NetBody;
        BodyWeAreChangingWith.GetComponent<Body>().leftHandType = LHChange;
    }

    [Command]
    public void CMD_SwitchRightHand(GameObject BodyWeAreChangingWith)
    {
        HandType NetBody = BodyWeAreChangingWith.GetComponent<Body>().rightHandType;
        HandType RHChange = rightHandType;
        rightHandType = NetBody;
        BodyWeAreChangingWith.GetComponent<Body>().rightHandType = RHChange;
    }

    [Command]
    public void CMD_SwitchLegs(GameObject BodyWeAreChangingWith)
    {
        LegType NetBody = BodyWeAreChangingWith.GetComponent<Body>().legsType;
        LegType LegChange = legsType;
        legsType = NetBody;
        BodyWeAreChangingWith.GetComponent<Body>().legsType = LegChange;
    }


    /*
     * [Command]
    void CMD_giveBackBodyAuthorityToServer(Vector3 Euler)
    {
        Debug.Log("leaving body");

        _BPM.BodyInPossession.gameObject.tag = "Unoccupied";

        _BPM.BodyInPossession.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        _BPM.BodyInPossession.transform.localRotation = Quaternion.Euler(Euler);

        _BPM.BodyInPossession.PlayerBodyIsAttachedTo = null;

        _BPM.BodyInPossession.netIdentity.RemoveClientAuthority();

        //_BPM.BodyInPossession.transform.position = transform.position;
        CLNT_LeaveBodyCallBack();
    }

    [ClientRpc]
    void CLNT_LeaveBodyCallBack()
    {
        _BPM.BodyInPossession.gameObject.tag = "Unoccupied"; 

        _BPM.BodyInPossession.gameObject.SetActive(true);
        _BPM.BodyInPossession.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        _BPM.BodyInPossession.transform.localRotation = transform.rotation;

        _BPM.BodyInPossession.PlayerBodyIsAttachedTo = null;
        
        _BPM.BodyInPossession.HeadRenderer.enabled = true;
        _BPM.BodyInPossession.ChestRenderer.enabled = true;
        _BPM.BodyInPossession.AbdomenRenderer.enabled = true;
        _BPM.BodyInPossession.RightHandRenderer.enabled = true;
        _BPM.BodyInPossession.LeftHandRenderer.enabled = true;
        _BPM.BodyInPossession.LegRenderer.enabled = true;

        _BPM.BodyInPossession._boxCollider.enabled = true;

        _BPM.BodyInPossession = null;
        
        _BPM.HeadMesh.enabled = true;
        
        _BPM._Body.gameObject.SetActive(false);
        _BPM._Body.Head.gameObject.SetActive(false);
    }
     * 
     * 
     * 
     [Command]
    void CMD_Respawn(Vector3 RespawnPoint)
    {
        _HeadHealth = 100;
        transform.position = RespawnPoint;
        RPC_Respawn(RespawnPoint);
    }

    [ClientRpc]
    void RPC_Respawn(Vector3 RespawnPoint)
    {
        _HeadHealth = 100;
        transform.position = RespawnPoint;
    }

    void LocalRespawn()
    {
        if (_Team == Team.Red) GameManager.Inst.CMD_BlueScored();
        else GameManager.Inst.CMD_RedScored();

        _HeadHealth = 100;
        transform.position = StartPos;
        CMD_Respawn(StartPos);
    }
     
     */
}
