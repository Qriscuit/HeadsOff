using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System;
using Mirror;

public class WeaponManager : NetworkBehaviour
{
    public BasePlayerManager _BPM;

    [SyncVar] public bool isWeaponAllowed;
    public bool FriendlyFire;
    [Header("Chest Abilities")]
    public PumpUp _PumpUp;
    public Printing _Printing;
    public JetPackChest _JetPackChest;
    public Shield _Shield;

    [Header("Hands Weapons")]
    public Latch _Latch;
    public BigShot _BigShot;
    public ElectroBall _ElectroBall;
    public FlameThrower _FlameThrower;
    public FeviTop _FeviTop;
    public ExplodingMines _ExplodingMines;
    public PunchGlove _PunchGlove;
    public ParticleAccelerator _ParticleAccelerator;
    public PortalGun _PortalGun;
    public BubbleGun _BubbleGun;
    public MachineGun _MachineGun;
    public ShotGun _ShotGun;
    public Sniper _Sniper;

    [Header("Legs Ability")]
    public BullRush _BullRush;
    public SuperJump _SuperJump;
    public DashLegs _DashLegs;
    public StompLegs _StompLegs;

    [Space]
    public LayerMask EnemyHitLayer;
    public float ShotgunRange;

    [Space]
    [SerializeField] LayerMask layerMask;
    public Vector3 targetPoint;


    [Space]
    public Transform LeftHandObject;
    public Vector3 LeftHandSpawnOffset;
    [HideInInspector]public Vector3 LeftHandSpawnPoint;

    [Space]
    public Transform RightHandObject;
    public Vector3 RightHandSpawnOffset;
    [HideInInspector]public Vector3 RightHandSpawnPoint;

    [Space]
    public Vector3 ChestSpawnPoint;

    public GameObject test;

    private void Awake()
    {
        isWeaponAllowed = true;
    }

    private void OnEnable()
    {
        _BPM = GetComponent<BasePlayerManager>();
        _BPM._IM._LeftArmButton += UseLeftWeapon;
        _BPM._IM._RightArmButton += UseRightWeapon;
        _BPM._IM._LegAbilityStarted += UseLegAbilityOnce;
        _BPM._IM._ChestAbility += UseChestAbility;
    }


    private void OnDisable()
    {
        _BPM._IM._LeftArmButton -= UseLeftWeapon;
        _BPM._IM._RightArmButton -= UseRightWeapon;
        _BPM._IM._LegAbilityStarted -= UseLegAbilityOnce;
        _BPM._IM._ChestAbility -= UseChestAbility;
    }


    #region Chest
    void UseChestAbility(bool Button)
    {
        if(isWeaponAllowed)
        if (Button)
        {
            if (_BPM.BodyInPossession == null) return;

            switch (_BPM.BodyInPossession.chestType)
            {
                    case HeadsOffGlobals.ChestType.JetPack:

                        Debug.Log("JetPack Used");
                        _JetPackChest.Launch();

                        break;

                    case HeadsOffGlobals.ChestType.Shield:

                        Debug.Log("Shield Used");
                        _Shield.LaunchBall(targetPoint, ChestSpawnPoint);

                        break;

                    case HeadsOffGlobals.ChestType.Printing:

                        Debug.Log("Printing Used");
                        _Printing.Launch();

                        break;


                    case HeadsOffGlobals.ChestType.PumpUp:

                        _PumpUp.Launch();

                        break;
                }
        }
    }
    #endregion

    #region Legs
    void UseLegAbilityOnce(bool Button)
    {
        if(isWeaponAllowed)
        if(Button)
        {
            if (_BPM.BodyInPossession == null) return;

            switch (_BPM.BodyInPossession.legsType)
            {
                case HeadsOffGlobals.LegType.BullRush:

                    Debug.Log("BullRush Used");
                    _BullRush.Launch();

                    break;

                case HeadsOffGlobals.LegType.SuperJump:

                    Debug.Log("SuperJump Used");
                    _SuperJump.Launch();

                    break;

                case HeadsOffGlobals.LegType.Dash:

                    Debug.Log("Dash Used");
                    _DashLegs.Launch();

                    break;

                case HeadsOffGlobals.LegType.Stomp:

                    Debug.Log("Stomp Used");
                    _StompLegs.Launch();

                    break;
            }
        }
    }

    #endregion

    #region LeftHand

    void UseLeftWeapon(bool Button)
    {

        if (isWeaponAllowed)
            if (Button)//this bool remains true while button is pressed act as a get key down
            {
                if (_BPM.BodyInPossession == null) return;
                StartCoroutine(LeftHandShoot());
                _BPM._AM.ActivateAnimRiggingLefttHand();
            }
            else //  act as a get key UP
            {
                if (_BPM.BodyInPossession != null)
                {
                    _BPM._AM.DeActivateAnimRiggingLeftHand();
                    //Debug.Log("left hand key up");
                    switch (_BPM.BodyInPossession.leftHandType)
                    {

                        case HeadsOffGlobals.HandType.FlameThrower:

                            Debug.Log("FlameThrower Deused");
                            _FlameThrower.DeLaunchL();
                            break;

                        case HeadsOffGlobals.HandType.BubbleGun:
                            Debug.Log("BUBBLE Deused");
                            _BubbleGun.DeLaunchL();
                            break;

                        case HeadsOffGlobals.HandType.ParticleAccelerator:
                            Debug.Log("Partical Accel used");
                            _ParticleAccelerator.Launch(targetPoint, LeftHandSpawnPoint, 0);
                            break;
                    }
                }
            }
    }

    IEnumerator LeftHandShoot()
    {
        yield return new WaitUntil(_BPM._AM.IsLeftArmInPositionToShoot);

        switch (_BPM.BodyInPossession.leftHandType)
        {
            case HeadsOffGlobals.HandType.Latch:

                Debug.Log("Latch Used");
                _Latch.Launch(targetPoint, LeftHandSpawnPoint);

                break;

            case HeadsOffGlobals.HandType.BigShot:

                Debug.Log("BigShot Used");
                _BigShot.Launch(LeftHandSpawnPoint, targetPoint, 0);

                break;

            case HeadsOffGlobals.HandType.ElectroBall:

                Debug.Log("ElectroBall Used");
                _ElectroBall.Launch(LeftHandSpawnPoint, targetPoint, 0);

                break;

            case HeadsOffGlobals.HandType.FlameThrower:

                Debug.Log("FlameThrower Used");
                _FlameThrower.Launch(LeftHandSpawnPoint, targetPoint, 0);

                break;

            case HeadsOffGlobals.HandType.PortalGun:

                Debug.Log("pORTAL usED");
                _PortalGun.Launch(targetPoint, RightHandSpawnPoint);

                break;

            case HeadsOffGlobals.HandType.ExplodingMines:

                Debug.Log("Mines used");
                _ExplodingMines.Launch(targetPoint, LeftHandSpawnPoint, 0);
                _BPM._NNBody._LHExplodingMinesAccentsAnimator.Play("LH_ExplodingMine");
                break;

            case HeadsOffGlobals.HandType.ParticleAccelerator:
                Debug.Log("Partical Accel used");
                _ParticleAccelerator.Recharge(0);
                break;

            case HeadsOffGlobals.HandType.PunchGlove:

                Debug.Log("Punch used");
                _PunchGlove.LaunchGlove(targetPoint, LeftHandSpawnPoint, 0);

                break;

            case HeadsOffGlobals.HandType.FeviTop:

                Debug.Log("Fevi used");
                _FeviTop.Launch(LeftHandSpawnPoint, targetPoint, 0);

                break;

            case HeadsOffGlobals.HandType.BubbleGun:
                Debug.Log("BUBBLE used");
                _BubbleGun.Launch(LeftHandSpawnPoint, targetPoint, 0);
                break;

            case HeadsOffGlobals.HandType.MachineGun:
                _MachineGun.LaunchBullet(LeftHandSpawnPoint, targetPoint, 0);
                break;

            case HeadsOffGlobals.HandType.ShotGun:
                _ShotGun.LaunchBullet(LeftHandSpawnPoint, targetPoint, 0);
                break;

            case HeadsOffGlobals.HandType.Sniper:
                _Sniper.LaunchBullet(LeftHandSpawnPoint, targetPoint, 0);
                break;
        }

    }

    #endregion

    #region RightHand
    void UseRightWeapon(bool Button)
    {
        if (isWeaponAllowed)
            if (Button)//this bool remains true while button is pressed act as a get key down
            {
                if (_BPM.BodyInPossession == null) return;
                StartCoroutine(RightHandShoot());
                _BPM._AM.ActivateAnimRiggingRightHand();
            }
            else //  act as a get key UP
            {
                if (_BPM.BodyInPossession != null)
                {
                    _BPM._AM.DeActivateAnimRiggingRightHand();
                    Debug.Log("Right keyup");
                    switch (_BPM.BodyInPossession.rightHandType)
                    {
                        case HeadsOffGlobals.HandType.FlameThrower:

                            Debug.Log("FlameThrower Deused");
                            _FlameThrower.DeLaunchR();
                            

                            break;

                        case HeadsOffGlobals.HandType.BubbleGun:
                            Debug.Log("Fevi Deused");
                            _BubbleGun.DeLaunchR();
                            break;

                        case HeadsOffGlobals.HandType.ParticleAccelerator:
                            Debug.Log("particle accel used");
                            _ParticleAccelerator.Launch(targetPoint, RightHandSpawnPoint, 1);

                            break;
                    }
                }
            }
    }

    IEnumerator RightHandShoot()
    {
        yield return new WaitUntil(_BPM._AM.IsRightArmInPositionToShoot);

        switch (_BPM.BodyInPossession.rightHandType)
        {
            case HeadsOffGlobals.HandType.Latch:

                Debug.Log("Latch Used");
                _Latch.Launch(targetPoint, RightHandSpawnPoint);

                break;

            case HeadsOffGlobals.HandType.BigShot:

                Debug.Log("BigShot Used");
                _BigShot.Launch(RightHandSpawnPoint, targetPoint, 1);

                break;

            case HeadsOffGlobals.HandType.ElectroBall:

                Debug.Log("ElectroBall Used");
                _ElectroBall.Launch(RightHandSpawnPoint, targetPoint, 1);

                break;

            case HeadsOffGlobals.HandType.FlameThrower:

                Debug.Log("FlameThrower Used");
                _FlameThrower.Launch(RightHandSpawnPoint, targetPoint, 1);

                break;

            case HeadsOffGlobals.HandType.ExplodingMines:

                Debug.Log("Mines used");
                _ExplodingMines.Launch(targetPoint, RightHandSpawnPoint, 1);
                _BPM._NNBody._RHExplodingMinesAccentsAnimator.Play("RH_ExplodingMine");
                break;

            case HeadsOffGlobals.HandType.ParticleAccelerator:
                Debug.Log("particle accel used");
                _ParticleAccelerator.Recharge(1);

                break;

            case HeadsOffGlobals.HandType.PunchGlove:

                Debug.Log("Punch used");
                _PunchGlove.LaunchGlove(targetPoint, RightHandSpawnPoint, 1);

                break;

            case HeadsOffGlobals.HandType.FeviTop:

                Debug.Log("Fevi used");
                _FeviTop.Launch(RightHandSpawnPoint, targetPoint, 1);

                break;

            case HeadsOffGlobals.HandType.PortalGun:

                Debug.Log("PortalGun used");
                _PortalGun.Launch(targetPoint, LeftHandSpawnPoint);

                break;

            case HeadsOffGlobals.HandType.BubbleGun:
                Debug.Log("Fevi used");
                _BubbleGun.Launch(RightHandSpawnPoint, targetPoint, 1);
                break;

            case HeadsOffGlobals.HandType.MachineGun:
                _MachineGun.LaunchBullet(RightHandSpawnPoint, targetPoint, 1);
                break;

            case HeadsOffGlobals.HandType.ShotGun:
                _ShotGun.LaunchBullet(RightHandSpawnPoint, targetPoint, 1);
                break;

            case HeadsOffGlobals.HandType.Sniper:
                _Sniper.LaunchBullet(RightHandSpawnPoint, targetPoint, 1);
                break;
        }
    }

    #endregion

    private void Update()
    {
        RightHandSpawnPoint = RightHandObject.TransformPoint(RightHandSpawnOffset);
        LeftHandSpawnPoint = LeftHandObject.TransformPoint(LeftHandSpawnOffset);
            RaycastHit hit;
            if (Physics.Raycast(_BPM._Cam.transform.position, _BPM._Cam.transform.forward, out hit, Mathf.Infinity, layerMask))
            {
            targetPoint = hit.point;
            }
    }

    public void TurnOffWeapons(float Time)
    {
        isWeaponAllowed = false;
        StartCoroutine(_turnOfWeapon(Time));
    }

    IEnumerator _turnOfWeapon(float Time)
    {
        yield return new WaitForSeconds(Time);
        isWeaponAllowed = true;
    }

    [Command]
    public void CMD_SpawnBigShot(Vector3 PositionToSpawn, Vector3 CamForward)
    {
        GameObject BallBeingShot = Instantiate(_BigShot.BigShotPrefab, PositionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(BallBeingShot);

        BigShotManager _BSM = BallBeingShot.GetComponent<BigShotManager>();

        _BSM.myTeam = _BPM._Team;
        _BSM._BPM = _BPM;

        Vector3 direction = CamForward - BallBeingShot.transform.position;
        _BSM.LaunchBall(direction.normalized);
    }

    [Command]
    public void CMD_SpawnBubble(Vector3 PositionToSpawn, Vector3 CamForward)
    {
        GameObject BallBeingShot = Instantiate(_BubbleGun.BubbleGunPrefab, PositionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(BallBeingShot);

        BubbleGunManager _BSM = BallBeingShot.GetComponent<BubbleGunManager>();

        _BSM.myTeam = _BPM._Team;
        _BSM._BPM = _BPM;

        Vector3 direction = CamForward - BallBeingShot.transform.position;
        _BSM.LaunchBall(direction.normalized);
    }

    [Command]
    public void CMD_SpawnMachineGunBullet(Vector3 PositionToSpawn, Vector3 CamForward)
    {
        GameObject BulletBeingShot = Instantiate(_MachineGun._MachineGunBullet.gameObject, PositionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(BulletBeingShot);

        BulletBeingShot.GetComponent<MachineGunBullet>().myTeam = _BPM._Team;

        Vector3 direction = CamForward - BulletBeingShot.transform.position;
        BulletBeingShot.GetComponent<MachineGunBullet>().LaunchBullet(direction.normalized);
    }

    [Command]
    public void CMD_SpawnShotGunBullet(Vector3 PositionToSpawn, Vector3 CamForward)
    {
        Quaternion RandomRotation = Random.rotation;
        GameObject BulletBeingShot = Instantiate(_ShotGun._ShotGunBullet.gameObject, PositionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(BulletBeingShot);

        BulletBeingShot.GetComponent<ShotGunBullet>().myTeam = _BPM._Team;

        Vector3 direction = CamForward - BulletBeingShot.transform.position;

        BulletBeingShot.transform.forward = direction;

        BulletBeingShot.transform.rotation = Quaternion.RotateTowards(BulletBeingShot.transform.rotation, RandomRotation, _ShotGun.ShotgunSpread);

        //Debug.Log("Euler Representation of RandomRotation is " + RandomRotation.eulerAngles);
        //Debug.Log("Euler Representation of DirectionToGoTowards is " + DirectionToGoTowards.eulerAngles);
        //Debug.Log("Euler Representation of RotateTowards is " + SpreadQuaternion.eulerAngles);
        
        BulletBeingShot.GetComponent<ShotGunBullet>().LaunchBullet();
        //BulletBeingShot.GetComponent<ShotGunBullet>().LaunchBullet(direction.normalized);
    }

    [Command]
    public void CMD_RaycastShotGunBullet(Vector3 PositionToSpawn, Vector3 CamForward)
    {
        Quaternion RandomRotation = Random.rotation;
        Vector3 RandomDirection = Random.insideUnitSphere.normalized;

        Vector3 CorrectDirection = CamForward - PositionToSpawn;

        Ray NewRay = new Ray(PositionToSpawn, RandomDirection);
        RaycastHit HitInfo = new RaycastHit();

        Debug.Log("Checking Raycast");
        Debug.DrawRay(PositionToSpawn, CorrectDirection * ShotgunRange, Color.blue, 5f);
        Debug.Log("Blue Ray has been cast");

        Vector3 RayRotation = Vector3.Lerp(RandomDirection, CorrectDirection, _ShotGun.ShotgunSpread);

        Debug.DrawRay(PositionToSpawn, RayRotation * ShotgunRange, Color.red, 5f);
        Debug.Log("Red Ray has been cast");

        if (Physics.Raycast(NewRay, out HitInfo, ShotgunRange, EnemyHitLayer))
        {
            HitInfo.collider.gameObject.GetComponent<DamageManager>();

        }

        //Debug.Log("Euler Representation of RandomRotation is " + RandomRotation.eulerAngles);
        //Debug.Log("Euler Representation of DirectionToGoTowards is " + DirectionToGoTowards.eulerAngles);
        //Debug.Log("Euler Representation of RotateTowards is " + SpreadQuaternion.eulerAngles);

        //BulletBeingShot.GetComponent<ShotGunBullet>().LaunchBullet();
        //BulletBeingShot.GetComponent<ShotGunBullet>().LaunchBullet(direction.normalized);
    }


    [Command]
    public void CMD_SpawnSniperBullet(Vector3 PositionToSpawn, Vector3 CamForward)
    {
        GameObject BulletBeingShot = Instantiate(_Sniper._SniperBullet.gameObject, PositionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(BulletBeingShot);

        BulletBeingShot.GetComponent<SniperBullet>().myTeam = _BPM._Team;

        Vector3 direction = CamForward - BulletBeingShot.transform.position;
        BulletBeingShot.GetComponent<SniperBullet>().LaunchBullet(direction.normalized);
    }


    /*[Command]
    public void CMD_SpawnBubbleShot(Vector3 PositionToSpawn, Vector3 CamForward)
    {
        GameObject BallBeingShot = Instantiate(_BubbleGun.BigShotPrefab, PositionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(BallBeingShot);

        BubbleCollision _BC = BallBeingShot.GetComponent<BubbleCollision>();

        _BC.myTeam = _BPM._Team;
        _BC._BPM = _BPM;

        Vector3 direction = CamForward - BallBeingShot.transform.position;
        _BC.LaunchBall(direction.normalized);
    }
    */
    [Command]
    public void CMD_SpawnEBall(Vector3 PositionToSpawn, Vector3 CamForward)
    {
        GameObject BallBeingShot = Instantiate(_ElectroBall.EballPrefab, PositionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(BallBeingShot);

        ElectroBallManager _EBM = BallBeingShot.GetComponent<ElectroBallManager>();

        _EBM.myTeam = _BPM._Team;
        _EBM._BPM = _BPM;

        Vector3 direction = CamForward - BallBeingShot.transform.position;
        _EBM.LaunchBall(direction.normalized);
    }

    [Command]
    public void CMD_SpawnFire(Vector3 PositionToSpawn, int LeftRight)
    {
        GameObject Fire = Instantiate(_FlameThrower.CollisionParticle, PositionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(Fire,connectionToClient);
        Debug.Log("instantiating fire");
        if (LeftRight == 0)
        {
            CLNT_setFireRef(Fire,0);
            _FlameThrower._PCL = Fire.GetComponent<ParticleCollision>();
            _FlameThrower._PCL.myTeam = _BPM._Team;

            _FlameThrower.LaunchPCL();
        }

        else
        {
            CLNT_setFireRef(Fire, 1);
            _FlameThrower._PCR = Fire.GetComponent<ParticleCollision>();
            _FlameThrower._PCR.myTeam = _BPM._Team;
            _FlameThrower.LaunchPCR();
        }
           
    }
    [Command]
    public void DestroyFlame(int LR)
    {
        if (LR == 0)
            NetworkServer.Destroy(_FlameThrower._PCL.gameObject);
        if (LR == 1)
            NetworkServer.Destroy(_FlameThrower._PCR.gameObject);
    }


    [ClientRpc]
    void CLNT_setFireRef(GameObject Fire, int LeftRight)
    {
        if (LeftRight == 0)
        {
            _FlameThrower._PCL = Fire.GetComponent<ParticleCollision>();
            _FlameThrower.LaunchPCL();
        }

        else
        {
            _FlameThrower._PCR = Fire.GetComponent<ParticleCollision>();
            _FlameThrower.LaunchPCR();
        }
    }
    
    [Command]
    public void CMD_SpawnMine(Vector3 PositionToSpawn, Vector3 CamForward, bool LeftMine)
    {
        GameObject Mine = Instantiate(_ExplodingMines._Mine, PositionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(Mine);

        Mine _Mine = Mine.GetComponent<Mine>();
        _Mine._BPM = _BPM;
        _Mine._Team = _BPM._Team;
        
        Vector3 direction = CamForward - Mine.transform.position;
        _Mine.LaunchMineFromHand(direction.normalized);
        CLNT_setMineref();
    }
    [ClientRpc]
    void CLNT_setMineref()
    {
        _BPM._PAM.ExploadingMine.Play();
    }

    [Command]
    public void CMD_SpawnStomp(Vector3 PositionToSpawn, float EndValue)
    {
        GameObject stomp = Instantiate(_StompLegs._StompShockWaveParent, PositionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(stomp);

        StompShockWave _StompShockWave = stomp.GetComponent<StompShockWave>();

        _StompShockWave._StompLegs = _StompLegs;
        _StompShockWave._team = _BPM._Team;
        _StompShockWave.ServerStartScallingRPCCaller(EndValue);
    }

    [Command]
    public void CMD_SpawnPortal(Vector3 PositionToSpawn, int PortalN, Vector3 PlaneNormal)
    {
        if(PortalN==0)
        {
            GameObject PortalA = Instantiate(_PortalGun.TeleGameObjA, PositionToSpawn, Quaternion.identity);
            PortalA.transform.forward = PlaneNormal;
            NetworkServer.Spawn(PortalA);
            CLNT_setPortalValue(PortalA,PortalN);
            _PortalGun.TelePorter_A = PortalA.GetComponent<Portals>();
            _PortalGun.SetValuePortalA();
        }
        else if(PortalN == 1)
        {
            GameObject PortalB = Instantiate(_PortalGun.TeleGameObjB, PositionToSpawn, Quaternion.identity);
            PortalB.transform.forward = PlaneNormal;
            NetworkServer.Spawn(PortalB);
            CLNT_setPortalValue(PortalB, PortalN);
            _PortalGun.TelePorter_B = PortalB.GetComponent<Portals>();
            _PortalGun.SetValuePortalB();
        }
        else
        {
            NetworkServer.Destroy(_PortalGun.TelePorter_A.gameObject);
            NetworkServer.Destroy(_PortalGun.TelePorter_B.gameObject);
        }
       
    }

    [ClientRpc]
    void CLNT_setPortalValue(GameObject Portal, int PortalN)
    {
        if (PortalN == 0)
        {
            _PortalGun.TelePorter_A = Portal.GetComponent<Portals>();
            _PortalGun.SetValuePortalA();
        }

        else
        {
            _PortalGun.TelePorter_B = Portal.GetComponent<Portals>();
            _PortalGun.SetValuePortalB();
        }
    }

    [Command]
    public void CMD_SpawnPunch(Vector3 PositionToSpawn, Vector3 CamForward)
    {
        GameObject punchGlove = Instantiate(_PunchGlove._SpawnedBasePrefab, PositionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(punchGlove);

        Vector3 direction = CamForward - punchGlove.transform.position;

        PG_BaseLaunchPoint _PBG = punchGlove.GetComponent<PG_BaseLaunchPoint>();
        _PBG._BPM = _BPM;
        _PBG.myTeam = _BPM._Team;
        _PBG.LaunchPunch(direction.normalized);
    }

    [Command]
    public void CMD_SpawnShield(Vector3 PositionToSpawn, Vector3 CamForwad)
    {
        GameObject _SpawnedBall = Instantiate(_Shield._shieldBall.gameObject, transform.TransformPoint(PositionToSpawn), Quaternion.identity);
        NetworkServer.Spawn(_SpawnedBall);


        _SpawnedBall.transform.forward = transform.forward;
        _Shield._SpawnedBall = _SpawnedBall.GetComponent<ShieldBall>();

        //Vector3 direction = CamForwad - PositionToSpawn;
        _SpawnedBall.transform.forward = _BPM._Cam.transform.forward;
        _Shield._SpawnedBall.LaunchBall(CamForwad);

        CLNT_ShieldRef(_SpawnedBall);
    }

    [ClientRpc]
    public void CLNT_ShieldRef(GameObject _SpawnedBall)
    {
        _SpawnedBall.transform.forward = transform.forward;
        _Shield._SpawnedBall = _SpawnedBall.GetComponent<ShieldBall>();
    }

    [Command]
    public void CMD_SpawnPumpUp(Vector3 PositionToSpawn, float ScaleEndValue, float ScaleTime, float PumpUpDamage, float DeleteAfterThisTime)
    {
        GameObject PumpUp = Instantiate(_PumpUp.PumpUpVFX,PositionToSpawn,Quaternion.identity);
        NetworkServer.Spawn(PumpUp);
        PumpUpManager _PUM = PumpUp.GetComponent<PumpUpManager>();
        _PumpUp.CurrentPumpLevel = 0;


        _PUM._pumpup = _PumpUp;
        _PUM.StartScalling(ScaleEndValue, ScaleTime, PumpUpDamage, DeleteAfterThisTime);
        _PUM.MyTeam = _BPM._Team;
        CLNT_setPumpref();
    }
    [ClientRpc]
    public void CLNT_setPumpref()
    {
        _BPM._PAM.PumpUpCharge.Stop();
        _BPM._PAM.PumpUpExplosion.Play();
    }

    [Command]
    public void CMD_SpawnFeviTop(Vector3 PositionToSpawn, Vector3 _dir, float FTmoveSpeed)
    {
        GameObject FeviTop = Instantiate(_FeviTop.FeviTopBallPrefab.gameObject, PositionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(FeviTop, FeviTop);
        FeviTop.GetComponent<FeviTopParent>().Launch(transform.forward, FTmoveSpeed, this);
        FeviTop.GetComponent<FeviTopParent>().myTeam = _BPM._Team;
    }
    [ClientRpc]
    public void CLNT_setSpawnFEVIref()
    {
        _BPM._PAM.Fevitop.Play();
    }

    [Command]
    public void CMD_ToggleBullRushValues(bool Enabled)
    {
        _BullRush._BullCol.enabled = enabled;
        _BullRush.IsBullRunAllowed = enabled;
        _BPM._VFX.CLNT_VFX_LbullRushStop();
    }

    [ClientRpc]
    public void CLNT_UpdateCurrentPumpLevel(float currentPumpLevel)
    {
        _PumpUp.CurrentPumpLevel = currentPumpLevel;

        if(currentPumpLevel == 0)
            _BPM._NNBody._PumpUpAccentsAnimator.Play("PumpUp_Idle");

        if (currentPumpLevel > 0 && currentPumpLevel <= 50)
            _BPM._NNBody._PumpUpAccentsAnimator.Play("PumpUp_Slow");

        if (currentPumpLevel > 51 && currentPumpLevel <= 99)
        {
            if (!_BPM._PAM.PumpUpCharge.isPlaying)
                _BPM._PAM.PumpUpCharge.Play();
            _BPM._NNBody._PumpUpAccentsAnimator.Play("PumpUp_Mid");
        }
            _BPM._NNBody._PumpUpAccentsAnimator.Play("PumpUp_Mid");

        if (currentPumpLevel == 100)
            _BPM._NNBody._PumpUpAccentsAnimator.Play("PumpUp_Fast");
    }

    [ClientRpc]
    public void CLNT_UpdateShieldHealth(float shieldHealth)
    {
        _Shield.ShieldHealth = shieldHealth;
    }


    /*

    [Command]
    public void CMD_SpawnPA(Vector3 positionToSpawn, int i)
    {
        GameObject PA = Instantiate(_ParticleAccelerator.CylinderCollider, positionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(PA);

        _ParticleAccelerator._PA_Collision[i] = PA.GetComponent<PA_Collision>();
        CLNT_SetPAref(PA,i);
    }

    [ClientRpc]
    public void CLNT_SetPAref(GameObject PA, int i)
    {
        _ParticleAccelerator._PA_Collision[i] = PA.GetComponent<PA_Collision>();

    }
    */
}
