using UnityEngine;
using HeadsOffGlobals;
using UnityEngine.Animations.Rigging;

public class NonNetworkedBody : MonoBehaviour
{
    [Header("General")]
    public BasePlayerManager _BPM;
    public GameObject Head;
    public GameObject Abdomen;
    public Animator _Animator;

    [Header("Accent GameObjects")]
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

    [Header("Renderers")]
    public SkinnedMeshRenderer _PumpUpMeshRenderer;
    public SkinnedMeshRenderer _ShieldAccentsRenderer;
    [Space]
    public SkinnedMeshRenderer _LHParticleAcceleratorAccentsRenderer;
    public SkinnedMeshRenderer _LHExplodingMinesAccentsRenderer;
    public SkinnedMeshRenderer _LHPunchGloveAccentsRenderer;
    [Space]
    public SkinnedMeshRenderer _RHParticleAcceleratorAccentsRenderer;
    public SkinnedMeshRenderer _RHExplodingMinesAccentsRenderer;
    public SkinnedMeshRenderer _RHPunchGloveAccentsRenderer;

    [Header("Animators")]
    public Animator _PumpUpAccentsAnimator;
    public Animator _ShieldAccentsAnimator;
    public Animator _LHParticleAcceleratorAccentsAnimator;
    public Animator _LHExplodingMinesAccentsAnimator;
    public Animator _RHParticleAcceleratorAccentsAnimator;
    public Animator _RHExplodingMinesAccentsAnimator;

    [Header("Scriptable Objects")]
    public SO_Chest[] _ChestSO;
    public SO_Legs[] _LegsSO;
    public SO_LeftHand[] _LeftHandSO;
    public SO_RightHand[] _RightHandSO;

    [Header("Renderer")]
    public SkinnedMeshRenderer ChestRenderer;
    public SkinnedMeshRenderer LeftHandRenderer;
    public SkinnedMeshRenderer RightHandRenderer;
    public SkinnedMeshRenderer LegRenderer;
    
    [Header("VFX")]
    public GameObject LeftElectroBall;
    public GameObject RightElectroBall;
    public GameObject PumpUpSmoke1;
    public GameObject PumpUpSmoke2;

    [Header("Right Hand Animation Rigging")]
    public TwoBoneIKConstraint RightHandRig;
    public Transform RightHandTaget;
    public TwoBoneIKConstraint RHRecoilRig;

    [Header("Left Hand Animation Rigging")]
    public TwoBoneIKConstraint LeftHandRig;
    public Transform LeftHandTarget;
    public TwoBoneIKConstraint LHRecoilRig;
    
    [Header("Type")]
    [SerializeField] ChestType _ChestTypeInspector;
    [SerializeField] LegType _LegTypeInspector;
    [SerializeField] HandType _LeftHandTypeInspector;
    [SerializeField] HandType _RightHandTypeInspector;

    public delegate void ChestChanged();
    public event ChestChanged _ChestChanged;

    public delegate void LHChanged();
    public event LHChanged _LHChanged;

    public delegate void RHChanged();
    public event RHChanged _RHChanged;

    public delegate void LegsChanged();
    public event LegsChanged _LegsChanged;

    [HideInInspector]
    public ChestType _chestType;
    public ChestType chestType
    {
        get
        {
            return _chestType;
        }
        set
        {
            _chestType = value;
            UpdateChestMesh();
            _ChestChanged?.Invoke();
        }
    }
    void UpdateChestMesh()
    {
        int i = (int)chestType;
        _ChestTypeInspector = chestType;
        
        ChestRenderer.sharedMesh = _ChestSO[i].Mesh;
        switch (_BPM._Team)
        {
            case Team.Red:
                ChestRenderer.sharedMaterials = new Material[_ChestSO[i].RedMaterials.Count];
                ChestRenderer.sharedMaterials = _ChestSO[i].RedMaterials.ToArray();

                if (chestType == ChestType.PumpUp)
                {
                    _PumpUpAccents.SetActive(true);
                    _PumpUpMeshRenderer.material = _ChestSO[i].RedAccentMaterial;
                }
                else _PumpUpAccents.SetActive(false);
                if (chestType == ChestType.Shield)
                {
                    _ShieldAccents.SetActive(true);
                }
                else _ShieldAccents.SetActive(false);

                break;

            case Team.Blue:
                ChestRenderer.sharedMaterials = new Material[_ChestSO[i].BlueMaterials.Count];
                ChestRenderer.sharedMaterials = _ChestSO[i].BlueMaterials.ToArray();

                if (chestType == ChestType.PumpUp)
                {
                    _PumpUpAccents.SetActive(true);
                    _PumpUpMeshRenderer.material = _ChestSO[i].BlueAccentMaterial;
                }
                else _PumpUpAccents.SetActive(false);
                if (chestType == ChestType.Shield)
                {
                    _ShieldAccents.SetActive(true);
                }
                else _ShieldAccents.SetActive(false);
                break;

            case Team.NA:
                ChestRenderer.sharedMaterials = new Material[_ChestSO[i].GreyMaterials.Count];
                ChestRenderer.sharedMaterials = _ChestSO[i].GreyMaterials.ToArray();
                break;
        }
    }

    [HideInInspector]
    public LegType _legsType;
    public LegType legsType
    {
        get
        {
            return _legsType;
        }
        set
        {
            _legsType = value;
            UpdateLegMesh();
            _LegsChanged?.Invoke();
        }
    }
    void UpdateLegMesh()
    {
        int i = (int)legsType;
        _LegTypeInspector = legsType;
        LegRenderer.sharedMesh = _LegsSO[i].Mesh;

        switch (_BPM._Team)
        {
            case Team.Red:
                LegRenderer.sharedMaterials = new Material[_LegsSO[i].RedMaterials.Count];
                LegRenderer.sharedMaterials = _LegsSO[i].RedMaterials.ToArray();
                break;

            case Team.Blue:
                LegRenderer.sharedMaterials = new Material[_LegsSO[i].BlueMaterials.Count];
                LegRenderer.sharedMaterials = _LegsSO[i].BlueMaterials.ToArray();
                break;

            case Team.NA:
                LegRenderer.sharedMaterials = new Material[_LegsSO[i].GreyMaterials.Count];
                LegRenderer.sharedMaterials = _LegsSO[i].GreyMaterials.ToArray();
                break;
        }
    }

    [HideInInspector]
    public HandType _leftHandType;
    public HandType leftHandType
    {
        get
        {
            return _leftHandType;
        }
        set
        {
            _leftHandType = value;
            UpdateLeftHandMesh();
            _LHChanged?.Invoke();
        }
    }
    void UpdateLeftHandMesh()
    {
        int i = (int)leftHandType;
        _LeftHandTypeInspector = leftHandType;
        LeftHandRenderer.sharedMesh = _LeftHandSO[i].Mesh;

        if (leftHandType == HandType.PortalGun)
        {
            _BPM._VFX.CMD_VFX_BelectricLineL();
            _BPM._VFX.CMD_VFX_HportalStartL();
        }
        else
        {
            _BPM._VFX.CMD_VFX_HportalStopL();
        }

        if (leftHandType == HandType.ElectroBall)
        {
            Debug.Log("electroball vfx command call");
            _BPM._VFX.CMD_VFX_BelectricLineL();
        }
        else
        {
        }

        switch (_BPM._Team)
        {
            case Team.Red:
                LeftHandRenderer.sharedMaterials = new Material[_LeftHandSO[i].RedMaterials.Count];
                LeftHandRenderer.sharedMaterials = _LeftHandSO[i].RedMaterials.ToArray();

                if (leftHandType == HandType.ExplodingMines)
                {
                    _LHExplodingMinesAccents.SetActive(true);
                    _LHExplodingMinesAccentsRenderer.material = _LeftHandSO[i].RedAccentMaterial;
                }
                else _LHExplodingMinesAccents.SetActive(false);
                if (leftHandType == HandType.ParticleAccelerator)
                {
                    _LHParticleAcceleratorAccents.SetActive(true);
                    _LHParticleAcceleratorAccentsRenderer.material = _LeftHandSO[i].RedAccentMaterial;
                }
                else _LHParticleAcceleratorAccents.SetActive(false);
                if (leftHandType == HandType.PunchGlove)
                {
                    _LHPunchGloveAccents.SetActive(true);
                    _LHPunchGloveAccentsRenderer.material = _LeftHandSO[i].RedAccentMaterial;
                }
                else _LHPunchGloveAccents.SetActive(false);

                break;

            case Team.Blue:
                LeftHandRenderer.sharedMaterials = new Material[_LeftHandSO[i].BlueMaterials.Count];
                LeftHandRenderer.sharedMaterials = _LeftHandSO[i].BlueMaterials.ToArray();

                if (leftHandType == HandType.ExplodingMines)
                {
                    _LHExplodingMinesAccents.SetActive(true);
                    
                    _LHExplodingMinesAccentsRenderer.material = _LeftHandSO[i].BlueAccentMaterial;
                }
                else _LHExplodingMinesAccents.SetActive(false);
                if (leftHandType == HandType.ParticleAccelerator)
                {
                    _LHParticleAcceleratorAccents.SetActive(true);
                    
                    _LHParticleAcceleratorAccentsRenderer.material = _LeftHandSO[i].BlueAccentMaterial;
                }
                else _LHParticleAcceleratorAccents.SetActive(false);
                if (leftHandType == HandType.PunchGlove)
                {
                    _LHPunchGloveAccents.SetActive(true);
                    
                    _LHPunchGloveAccentsRenderer.material = _LeftHandSO[i].BlueAccentMaterial;
                }
                else _LHPunchGloveAccents.SetActive(false);

                break;

            case Team.NA:
                LeftHandRenderer.sharedMaterials = new Material[_LeftHandSO[i].GreyMaterials.Count];
                LeftHandRenderer.sharedMaterials = _LeftHandSO[i].GreyMaterials.ToArray();
                break;
        }
    }

    [HideInInspector]
    public HandType _rightHandType;
    public HandType rightHandType
    {
        get
        {
            return _rightHandType;
        }
        set
        {
            _rightHandType = value;
            UpdateRightHandMesh();
            _RHChanged?.Invoke();
        }
    }
    void UpdateRightHandMesh()
    {

        int i = (int)rightHandType;
        _RightHandTypeInspector = rightHandType;
        RightHandRenderer.sharedMesh = _RightHandSO[i].Mesh;
        
        if (rightHandType == HandType.PortalGun)
        {
            _BPM._VFX.CMD_VFX_BelectricLineR();
            _BPM._VFX.CMD_VFX_HportalStartR();
        }
        else
        {
            _BPM._VFX.CMD_VFX_HportalStopR();
        }

        if (rightHandType == HandType.ElectroBall)
        {
            _BPM._VFX.CMD_VFX_BelectricLineR();
        }
        else
        {
        }
        
        switch (_BPM._Team)
        {
            case Team.Red:
                RightHandRenderer.sharedMaterials = new Material[_RightHandSO[i].RedMaterials.Count];
                RightHandRenderer.sharedMaterials = _RightHandSO[i].RedMaterials.ToArray();

                if (rightHandType == HandType.ExplodingMines)
                {
                    _RHExplodingMinesAccents.SetActive(true);
                    
                    _RHExplodingMinesAccentsRenderer.material = _RightHandSO[i].RedAccentMaterial;
                }
                else _RHExplodingMinesAccents.SetActive(false);
                if (rightHandType == HandType.ParticleAccelerator)
                {
                    _RHParticleAcceleratorAccents.SetActive(true);
                    
                    _RHParticleAcceleratorAccentsRenderer.material = _RightHandSO[i].RedAccentMaterial;
                }
                else _RHParticleAcceleratorAccents.SetActive(false);
                if (rightHandType == HandType.PunchGlove)
                {
                    _RHPunchGloveAccents.SetActive(true);
                    
                    _RHPunchGloveAccentsRenderer.material = _RightHandSO[i].RedAccentMaterial;
                }
                else _RHPunchGloveAccents.SetActive(false);

                break;

            case Team.Blue:
                RightHandRenderer.sharedMaterials = new Material[_RightHandSO[i].BlueMaterials.Count];
                RightHandRenderer.sharedMaterials = _RightHandSO[i].BlueMaterials.ToArray();

                if (rightHandType == HandType.ExplodingMines)
                {
                    _RHExplodingMinesAccents.SetActive(true);
                    
                    _RHExplodingMinesAccentsRenderer.material = _RightHandSO[i].BlueAccentMaterial;
                }
                else _RHExplodingMinesAccents.SetActive(false);
                if (rightHandType == HandType.ParticleAccelerator)
                {
                    _RHParticleAcceleratorAccents.SetActive(true);
                    
                    _RHParticleAcceleratorAccentsRenderer.material = _RightHandSO[i].BlueAccentMaterial;
                }
                else _RHParticleAcceleratorAccents.SetActive(false);
                if (rightHandType == HandType.PunchGlove)
                {
                    _RHPunchGloveAccents.SetActive(true);
                    
                    _RHPunchGloveAccentsRenderer.material = _RightHandSO[i].BlueAccentMaterial;
                }
                else _RHPunchGloveAccents.SetActive(false);

                break;

            case Team.NA:
                RightHandRenderer.sharedMaterials = new Material[_RightHandSO[i].GreyMaterials.Count];
                RightHandRenderer.sharedMaterials = _RightHandSO[i].GreyMaterials.ToArray();
                break;
        }
    }

    private void Awake()
    {
        //_ExplodingMinesAccentAnimator = _ExplodingMinesAccents.GetComponent<Animator>();
        //_LatchAccentAnimator = _LatchAccents.GetComponent<Animator>();
        //_ParticleAcceleratorAccentsAnimator = _ParticleAcceleratorAccents.GetComponent<Animator>();
        //_PrintingAccentsAnimator = _PrintingAccents.GetComponent<Animator>();
        //_PumpUpAccentsAnimator = _PumpUpAccents.GetComponent<Animator>();
        //_ShieldAccentsAnimator = _ShieldAccents.GetComponent<Animator>();
        //_DashAccentAnimator = _DashAccents.GetComponent<Animator>();
        //_SuperJumpAccentAnimator = _SuperJumpAccents.GetComponent<Animator>();
    }
    //public void SetMeshProperty()
    //{
    //    chestType = _ChestTypeInspector;
    //    legsType = _LegTypeInspector;
    //    leftHandType = _LeftHandTypeInspector;
    //    rightHandType = _RightHandTypeInspector;
    //}

    public void OnButton_UpdateMesh()
    {
        chestType = _ChestTypeInspector;
        legsType = _LegTypeInspector;
        leftHandType = _LeftHandTypeInspector;
        rightHandType = _RightHandTypeInspector;
    }

    private void Start()
    {
        _Animator = GetComponent<Animator>();
        _BPM = GetComponentInParent<BasePlayerManager>();
    }

    public void SetMeshProperty()
    {
        chestType = _ChestTypeInspector;
        legsType = _LegTypeInspector;
        leftHandType = _LeftHandTypeInspector;
        rightHandType = _RightHandTypeInspector;

        UpdateChestMesh();
        UpdateLegMesh();
        UpdateRightHandMesh();
        UpdateLeftHandMesh();
    }

    public void UpdateMesh(Body BodyInPossession)
    {
        chestType = BodyInPossession.chestType;
        //if (BodyInPossession._chestType == ChestType.PumpUp)
        //{
        //    PumpUpSmoke1.SetActive(true);
        //    PumpUpSmoke2.SetActive(true);
        //}
        //else
        //{
        //    PumpUpSmoke1.SetActive(false);
        //    PumpUpSmoke2.SetActive(false);
        //}

        legsType = BodyInPossession.legsType;
        rightHandType = BodyInPossession.rightHandType;

        //if (BodyInPossession._rightHandType == HandType.ElectroBall || BodyInPossession._rightHandType == HandType.PortalGun)
        //    RightElectroBall.SetActive(true);
        //else
        //    RightElectroBall.SetActive(false);

        leftHandType = BodyInPossession.leftHandType;

        //if (BodyInPossession._leftHandType == HandType.ElectroBall || BodyInPossession._leftHandType == HandType.PortalGun)
        //    LeftElectroBall.SetActive(true);
        //else
        //    LeftElectroBall.SetActive(false);
        TurnOnAccentsNVfx();
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
        if (_leftHandType == HandType.ExplodingMines)
        {
            _LHExplodingMinesAccents.SetActive(true);
        }
        else _LHExplodingMinesAccents.SetActive(false);
        if (_leftHandType == HandType.ParticleAccelerator)
        {
            _LHParticleAcceleratorAccents.SetActive(true);
        }
        else _LHParticleAcceleratorAccents.SetActive(false);
        if (_leftHandType == HandType.PunchGlove)
        {
            _LHPunchGloveAccents.SetActive(true);
        }
        else _LHPunchGloveAccents.SetActive(false);

        if (_rightHandType == HandType.ExplodingMines)
        {
            _RHExplodingMinesAccents.SetActive(true);
        }
        else _RHExplodingMinesAccents.SetActive(false);
        if (_rightHandType == HandType.ParticleAccelerator)
        {
            _RHParticleAcceleratorAccents.SetActive(true);
        }
        else _RHParticleAcceleratorAccents.SetActive(false);
        if (_rightHandType == HandType.PunchGlove)
        {
            _RHPunchGloveAccents.SetActive(true);
        }
        else _RHPunchGloveAccents.SetActive(false);

        if (_chestType == ChestType.PumpUp)
        {
            _PumpUpAccents.SetActive(true);
        }
        else _PumpUpAccents.SetActive(false);
        if (_chestType == ChestType.Shield)
        {
            _ShieldAccents.SetActive(true);
        }
        else _ShieldAccents.SetActive(false);
    }

    void RandomizeBodyParts()
    {
        Debug.Log("Randomize called");

        int i = Random.Range((int)0, _ChestSO.Length);
        chestType = _ChestSO[i].Name;
        Debug.Log(chestType);
        i = Random.Range((int)0, _LegsSO.Length);
        legsType = _LegsSO[i].Name;
        Debug.Log(legsType);
        i = Random.Range((int)0, _LeftHandSO.Length);
        leftHandType = _LeftHandSO[i].Name;
        Debug.Log(leftHandType);
        i = Random.Range((int)0, _RightHandSO.Length);
        rightHandType = _RightHandSO[i].Name;
        Debug.Log(rightHandType);

        Debug.Log("---------------------------------------------------");
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
}
