using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Mirror;
using HeadsOffGlobals;
using System;
using FirstGearGames.FlexSceneManager.Events;
using FirstGearGames.FlexSceneManager;

public class BasePlayerManager : NetworkBehaviour
{
    //TODO add a callback when the scene loads and when all the players scenes are loaded the server allows player movement;
    public MovementHandler _MH;
    public MechanicManager _MM;
    public AnimationHandler _AM;
    public MasterInputManager _IM;
    public UI_Manager _UIM;
    public WeaponManager _WM;
    public VFXHandler _VFX;
    public DamageManager _DM;
    public CharacterController _CC;
    [Space]
    public Camera _Cam;
    public CinemachineBrain _CMBrain;
    public CinemachineFreeLook _CMHead;
    public CinemachineFreeLook _CMBody;
    public CinemachineVirtualCamera _CMBodyADS;
    public AudioListener _AudioListener;
    public MeshRenderer HeadMesh;
    public NonNetworkedBody _NNBody;
    public PlayerAudioManager _PAM;
    [Space]
    public int TestRed;
    public Material TestBlue;
    [Space]
    public Vector3 StartPosition = new Vector3();
    public Quaternion StartRotation = new Quaternion();

    [SyncVar]
    public int Score = 0;
    [SyncVar] public string Username = "";
    [SyncVar] public string displayName = "Loading...";

    [SyncVar(hook = nameof(ChangeHeadMaterial))]
    public Team _Team;
    void ChangeHeadMaterial(Team OldTeam, Team NewTeam)
    {
        if(NewTeam == Team.Blue)
        {
            HeadMesh.materials[0] = new Material(TestBlue);
            HeadMesh.materials[1] = new Material(TestBlue);
        }
    }

    // Without Body CC Collider data:
    // Radius - 1.25
    // Height - 2
    // Y Offset - 0.35

    // With Body CC Collider data:
    // Radius - 2.3
    // Height - 6.5
    // Y Offset - 2.7

    [HideInInspector]
    //[SyncVar(hook = nameof())]
    public bool BodyOccupied;
    public Body _BodyInPossession;
    public Body BodyInPossession
    {
        get
        {
            if (_BodyInPossession != null) return _BodyInPossession;
            else return null;
        }
        set
        {
            _BodyInPossession = value;
            if (value == null)
            {
                BodyOccupied = false;
                _NNBody.gameObject.SetActive(false);
                _NNBody.Head.gameObject.SetActive(false);
                _NNBody.Abdomen.gameObject.SetActive(false);

                _CC.radius = 1.25f;
                _CC.height = 2;
                _CC.center = Vector3.up * 0.35f;
                _bodyAttached.Invoke(false);
            }
            else
            {
                BodyOccupied = true;
                _NNBody.gameObject.SetActive(true);
                _NNBody.Head.gameObject.SetActive(true);
                _NNBody.Abdomen.gameObject.SetActive(true);

                _CC.radius = 2.3f;
                _CC.height = 6.5f;
                _CC.center = new Vector3(0, 2.7f, 0);
                _bodyAttached.Invoke(true);
            }
        }
    }

    public delegate void BodyAttached(bool Value);
    public event BodyAttached _bodyAttached;

    [Space]

    public const byte HEALTH_CHANGED = 1;


    [SyncVar(hook = nameof(HealthUpdateHook))]
    public float _HeadHealth = 100;
    public void HealthUpdateHook(float OldValue, float NewValue)
    {
        _HeadHealth = NewValue;
    }

    public int CurrentPlayerNumber;

    [Space]

    public bool TESTING = true;

    public bool IsThisBPMofMyTeam()
    {
        return false;
    }

    bool FirstInitialize = true;

    private NM_GC room;
    private NM_GC Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NM_GC;
        }
    }

    //private void OnEnable()
    //{
    //    _IM._ADSButton += ADSHeldExecute;
    //}

    //private void OnDisable()
    //{
    //    _IM._ADSButton -= ADSHeldExecute;
    //}

    //public bool ADSHeld = false;
    //public void ADSHeldExecute(bool Held)
    //{
    //    if (!BodyOccupied) return;

    //    if(Held)
    //    {
    //        _CMBodyADS.gameObject.SetActive(true);
    //        _CMBody.gameObject.SetActive(false);
    //    }
    //    else
    //    {
    //        _CMBodyADS.gameObject.SetActive(false);
    //        _CMBody.gameObject.SetActive(true);
    //    }
    //    ADSHeld = Held;
    //}

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        Room.NetworkGamePlayers.Add(this);
        
        if (!isLocalPlayer)
        {
            //_MH.enabled = false;
            _MM.enabled = false;
            _Cam.enabled = false;
            _IM.enabled = false;
            //_UIM.enabled = false;
            _CMBrain.enabled = false;
            _AudioListener.enabled = false;
            _UIM._Canvas.gameObject.SetActive(false);
            _CMHead.gameObject.SetActive(false);
            _CMBody.gameObject.SetActive(false);
        }

        if (isServer)
        {
            _Cam.gameObject.SetActive(true);
            _Cam.enabled = true;
            _CMBrain.enabled = true;
            _CMBody.gameObject.SetActive(true);
        }

        else if (isLocalPlayer)
        {
            _AudioListener.enabled = true;
            FlexSceneManager.OnLoadSceneEnd += OnNewSceneLoaded;
            Room.LocalBPM = this;
            CMD_SetName(Username);

            
        }
    }
    
    [Command]
    public void CMD_SetName(string Username)
    {

    }
    
    public override void OnStopClient()
    {
        Room.NetworkGamePlayers.Remove(this);
    }

    void OnNewSceneLoaded(LoadSceneEndEventArgs LSEEA)
    {
        InformServerOfLocalPlayersLoadComplete(this.netIdentity);
    }

    [Command]
    void InformServerOfLocalPlayersLoadComplete(NetworkIdentity ID)
    {
        Room.LocalClientsLoaded.Add(ID);

        Debug.Log("Local Clients Loaded = " + Room.LocalClientsLoaded.Count);
        Debug.Log("Network game players = " + Room.NetworkGamePlayers.Count);
        Debug.Log("Spectators    Loaded = " + Room.Spectators.Count);

        if (Room.LocalClientsLoaded.Count == Room.NetworkGamePlayers.Count + Room.Spectators.Count)
        {
            Room.AllPlayersReady = true;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        //_UM.UpdateHealthUI(Health);
    }

    float HealthChangedCallBack(float value)
    {
        float NewValue = 0;
        NewValue = value;

        //pump up
        if (BodyInPossession != null)
        {
            if (BodyInPossession.chestType == HeadsOffGlobals.ChestType.PumpUp)
            {
                if (!_WM._PumpUp.AbilityInUse) NewValue = _WM._PumpUp.NegateDamage(value);
                //else NewValue *= _WM._PumpUp.DamageMultiplierOnActivated;
            }

            if(BodyInPossession.chestType == HeadsOffGlobals.ChestType.Shield)
            {
                if(_WM._Shield.IsPersonalShieldActive) _VFX.FBCall_ShieldDamageStart();
                NewValue = _WM._Shield.DecreaseHealthBy(value);
            }
        }

        Debug.Log("Damage Being Done " + NewValue);

        return NewValue;
    }

    //[Server]
    bool isPlayerDead = false;
    public void HealthReduce(float Damage)
    {
        if (isClient) return;

        Damage = HealthChangedCallBack(Damage);

        if (_BodyInPossession != null)
        {
            _BodyInPossession._BodyHealth -= Damage;
            if (_BodyInPossession._BodyHealth <= 0)
            {
                if (_Team == Team.Red) GameManager.Inst.CMD_BlueScored(true);
                else GameManager.Inst.CMD_RedScored(true);
                CMD_KillPlayerBody();
                GameManager.Inst.SpawnDeathNotification(_DM.LastDamageBy.displayName, displayName, false);
                _VFX.CMD_DestroyBody();
                CLNT_playBodyDeathAudio();
            }
        }
        else
        {
            _HeadHealth -= Damage;
            if (_HeadHealth <= 0 && !isPlayerDead)
            {
                isPlayerDead = true;
                if (_Team == Team.Red) GameManager.Inst.CMD_BlueScored(false);
                else GameManager.Inst.CMD_RedScored(false);
                CMD_KillPlayerOnServer();
                GameManager.Inst.SpawnDeathNotification(_DM.LastDamageBy.displayName, displayName, true);
                _VFX.CMD_DestroyBody();
                CLNT_playHeadDeathAudio();
                _DM.LastDamageBy._VFX.CLNT_PlayKillEnemy();
            }
           
        }

        CLNT_showDamageIndicator();
    }

    [ClientRpc]
    void CLNT_playBodyDeathAudio()
    {
        _PAM.BodyDeath.Play();
    }

    [ClientRpc]
    void CLNT_playHeadDeathAudio()
    {
        _PAM.HeadDeath.Play();
    }

    Coroutine showDamageIndicators;
    [ClientRpc]
    void CLNT_showDamageIndicator()
    {
        if (isLocalPlayer)
            return;

        if (showDamageIndicators != null)
        {
            StopCoroutine(showDamageIndicators);
        }

        showDamageIndicators = StartCoroutine(showdamageind());
    }

    IEnumerator showdamageind()
    {
        if(_BodyInPossession!=null)
        {
            _UIM.FriendHealth.fillAmount = _BodyInPossession._BodyHealth / 150;
            _UIM.EnemyHealth.fillAmount = _BodyInPossession._BodyHealth / 150;
        }
        else
        {
            _UIM.FriendHealth.fillAmount = _HeadHealth / 100;
            _UIM.EnemyHealth.fillAmount = _HeadHealth / 100;
        }
        _UIM.HealthCanvas.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        _UIM.HealthCanvas.gameObject.SetActive(false);
    }




    [ClientRpc]
    void CLNT_Respawn()
    {
        Debug.Log("Player dead and respawned on all Client");

        _HeadHealth = 100;
        transform.position = StartPosition;
        transform.rotation = StartRotation;
    }

    void CMD_KillPlayerOnServer()
    {
        Debug.Log("Player dead and respawned on Server");

        _DM.LastDamageBy._UIM.MakeKillCrossHairAppear();

        _MH.IsMovementAllowed = false;
        CLNT_EditHeadVisibilityAndDamageCollidersForClients(false);
        CMD_stopFeedbacks();
        _VFX.FBCall_DeathGreyScaleStart();
        StartCoroutine(RespawnRoutine());
        _WM._JetPackChest.DeLaunch();
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(5);

        isPlayerDead = false;
        _MH.IsMovementAllowed = true;
        CLNT_EditHeadVisibilityAndDamageCollidersForClients(true);

        _HeadHealth = 100;
        transform.position = StartPosition;
        transform.rotation = StartRotation;
        CLNT_Respawn();
    }

    [ClientRpc] void CLNT_EditHeadVisibilityAndDamageCollidersForClients(bool Enabled)
    {
        if (isLocalPlayer) _VFX.FB_DeathGreyScale.PlayFeedbacks();
        HeadMesh.enabled = Enabled;
        _DM.HeadCollider.enabled = Enabled;
        _DM.BodyCollider.enabled = Enabled;
    }  

    void CMD_KillPlayerBody()
    {
        CLNT_DeLaunchweapon();
        CMD_stopFeedbacks();
        CMD_BodyDied();
        _DM.LastDamageBy._UIM.MakeKillCrossHairAppear();

        Debug.Log("Team is " + _Team + " so the score can be added");
    }

    [ClientRpc]
    void CLNT_DeLaunchweapon()
    {
        if (!isLocalPlayer)
            return;

        _WM._BubbleGun.DeLaunchL();
        _WM._BubbleGun.DeLaunchR();
        _WM._FlameThrower.DeLaunchL();
        _WM._FlameThrower.DeLaunchR();
        _WM._JetPackChest.DeLaunch();
    }

    //[ClientRpc]
    void CMD_stopFeedbacks()
    {
        _VFX.FBCall_BullrushStunStop();
        _VFX.FBCall_CamShakeStop();
        _VFX.FBCall_ElectroballStunStop();
        _VFX.FBCall_FlameDamageStop();
        _VFX.FBCall_GeneralDamageFlashStop();
        _VFX.FBCall_ShieldDamageStop();
        _VFX.FBCall_StickyGlueStop();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();


        //GameManager.Inst.UpdateTeam += UpdateTeam;
    }

    public void CMD_BodyDied()
    {
        Debug.Log("leaving body");
        BodyInPossession._BodyHealth = 150;
        BodyInPossession.gameObject.tag = "Unoccupied";
        BodyInPossession.transform.position = BodyInPossession.StartPosition;
        BodyInPossession.transform.localRotation = BodyInPossession.StartRotation;
        BodyInPossession.PlayerBodyIsAttachedTo = null;
        BodyInPossession.netIdentity.RemoveClientAuthority();
        BodyInPossession = null;
        transform.position = new Vector3(transform.position.x, transform.position.y + 6, transform.position.z);
        CLNT_BodyDied();
    }

    [ClientRpc]
    void CLNT_BodyDied()
    {
        Debug.Log("is body connected " + (_BodyInPossession != null));

        //if (!isLocalPlayer) return;
        if (BodyInPossession == null) return;

        BodyInPossession._BodyHealth = 150;
        BodyInPossession.gameObject.tag = "Unoccupied";
        BodyInPossession.gameObject.SetActive(true);
        BodyInPossession.transform.position = BodyInPossession.StartPosition;
        BodyInPossession.transform.localRotation = BodyInPossession.StartRotation;
        BodyInPossession.PlayerBodyIsAttachedTo = null;
        BodyInPossession.HeadRenderer.enabled = true;
        BodyInPossession.ChestRenderer.enabled = true;
        BodyInPossession.AbdomenRenderer.enabled = true;
        BodyInPossession.RightHandRenderer.enabled = true;
        BodyInPossession.LeftHandRenderer.enabled = true;
        BodyInPossession.LegRenderer.enabled = true;
        BodyInPossession._boxCollider.enabled = true;
        BodyInPossession.UpdateMesh();
        BodyInPossession = null;

        if (isLocalPlayer)
        {
            _CMHead.gameObject.SetActive(true);
            _CMBody.gameObject.SetActive(false);
        }
        
        HeadMesh.enabled = true;
        _NNBody.gameObject.SetActive(false);
        _NNBody.Head.gameObject.SetActive(false);
        _NNBody.Abdomen.gameObject.SetActive(false);
        transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
    }


    [ClientRpc]
    void CLNT_SetTeam(Team _team)
    {
        _Team = _team;
    }

    [ClientRpc]
    void ChangePlayerColor(bool Red)
    {
        //HeadMesh.materials = new Material[2];
        //FlexSceneManager M = FlexSceneManager.on
        if (Red) HeadMesh.materials[1] = HeadMesh.materials[0];
        else HeadMesh.materials[0] = HeadMesh.materials[1];
        //{
        //    Debug.Log("Turning Red");
        //    HeadMesh.materials[0] = TestRed;
        //    HeadMesh.materials[1] = TestRed;
        //}
        //else
        //{
        //    Debug.Log("Turning Blue");
        //    HeadMesh.materials[0] = TestBlue;
        //    HeadMesh.materials[1] = TestBlue;
        //}
    }

    //[Command]
    //void CMD_RegisterPlayerInNMGC()
    //{
    //    //Debug.Log("This players team is " + _team + " and his local value is " + _Team);

    //    if (_Team == Team.Red) GameManager.Inst.RedPlayers.Add(this);
    //    else GameManager.Inst.BluePlayers.Add(this);
    //}

    // something very cool happened here in function OnStartLocalPlayer the team name is set in a command so the local player gets the updated team after it comes from the server so the CMD_RegisterplayerInNMGC
    // passes red even though it just set itself to blue on the server a line before

    void HealthFinishedCallback(Vector3 StartPos)
    {
        //Debug.Log("Health finished call back");
        //CMD_Respawn(StartPos);
    }

    
    
    //void ReadyToRecieveControl()
    //{
    //    CMD_TakeAuthority();
    //}

    //[Command]
    public void GiveAuthority()
    {
        netIdentity.AssignClientAuthority(base.connectionToClient);
    }

    [ClientRpc]
    public void CLNT_CorrectPlayerTransform(Vector3 Position, Quaternion Rotation)
    {
        StartPosition = Position;
        StartRotation = Rotation;

        transform.position = StartPosition;
        transform.rotation = StartRotation;
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
        //FlexSceneManager.OnLoadSceneStart += OnSceneLoaded;
    }


    private void Update()
    {
        if(GameManager.Inst != null)
            if(GameManager.Inst.GameEnded && Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                CMD_startDeletion();
                Room.StopClient();
            }
    }

    [Command]
    void CMD_startDeletion()
    {
        ClientDisconnector(base.connectionToClient);
        Debug.Log("Delete");
        NetworkServer.DestroyPlayerForConnection(base.connectionToClient);
    }
    
    void ClientDisconnector(NetworkConnection Conn)
    {
        NetworkServer.DestroyPlayerForConnection(base.connectionToClient);
        Room.StopClient();
    }
}