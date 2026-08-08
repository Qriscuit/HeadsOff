using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MoreMountains.Feedbacks;
using System;
using HeadsOffGlobals;
using UnityEngine.SceneManagement;
using FirstGearGames.FlexSceneManager;
using FirstGearGames.FlexSceneManager.Events;

public class NM_GC : NetworkManager
{
    //public Dictionary<int, NetworkConnection> PlayerNumberDict = new Dictionary<int, NetworkConnection>();
    //public Dictionary<NetworkConnection, int> PlayerConnDict = new Dictionary<NetworkConnection, int>();

    public int MinPlayers;
    public RoomPlayer _NRPPrefab;
    public BasePlayerManager _NGPPrefab;
    public GameObject _SpectatorPrefab;
    public PlayerLobbyCanvas _PLC;
    public PlayerLobbyCanvas SpawnedPLC;

    [SerializeField]public List<NetworkConnection> networkConnections = new List<NetworkConnection>();

    public List<NetworkRoomPlayer> RediedRoomPlayers = new List<NetworkRoomPlayer>();
    public List<RoomPlayer> NetworkRoomPlayers = new List<RoomPlayer>();
    public List<BasePlayerManager> NetworkGamePlayers = new List<BasePlayerManager>();
    public List<BasePlayerManager> RedGamePlayers = new List<BasePlayerManager>();
    public List<BasePlayerManager> BlueGamePlayers = new List<BasePlayerManager>();
    public List<GameObject> Spectators = new List<GameObject>();
    public List<NetworkIdentity> LocalClientsLoaded = new List<NetworkIdentity>();

    public string LocalPlayersName = "";
    public bool IsServer = false;

    public RoomPlayer LocalNRP;
    public BasePlayerManager LocalBPM;
    
    public GameObject BodyPrefab;
    public GameObject GameManagerPrefab;
    [HideInInspector]
    public GameManager _GM;
    public GameObject SceneSwitcher;
    [Space]
    public Camera WorldCam;
    public MMFeedbacks LoadingScreenFeedBack;
    public GameObject LSImages;

    public static NM_GC Inst;
    public override void Awake()
    {
        Inst = this;
        Time.timeScale = 1;
    }

    public static event Action<NetworkConnection> OnServerRedied;
    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public override void OnStartServer()
    {
        base.OnStartServer();
        IsServer = true;

        FlexSceneManager.OnLoadSceneStart += OnNewSceneLoadingStarted;
        FlexSceneManager.OnLoadScenePercentChange += OnNewSceneLoading;
        FlexSceneManager.OnLoadSceneEnd += OnNewSceneLoaded;

        Debug.Log("New Server Started and its netadress is " + networkAddress);
    }

    public override void OnStopServer()
    {
        FlexSceneManager.OnLoadSceneStart -= OnNewSceneLoadingStarted;
        FlexSceneManager.OnLoadScenePercentChange -= OnNewSceneLoading;
        FlexSceneManager.OnLoadSceneEnd -= OnNewSceneLoaded;

        networkConnections.Clear();

        if(LocalLevelManager.Inst != null) LocalLevelManager.Inst.DeleteNMGC(this.gameObject);
    }
    
    private void SpawnBodyAndGM()
    {
        for (int i = 0; i < LocalLevelManager.Inst.BodySpawns.Length; i++)
        {
            GameObject BodyInstance = Instantiate(BodyPrefab, LocalLevelManager.Inst.BodySpawns[i].position +Vector3.up * 0.5f, LocalLevelManager.Inst.BodySpawns[i].rotation);
            NetworkServer.Spawn(BodyInstance);
        }

        GameObject GM = Instantiate(GameManagerPrefab);
        NetworkServer.Spawn(GM);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        FlexSceneManager.OnLoadSceneStart += OnNewSceneLoadingStarted;
        FlexSceneManager.OnLoadScenePercentChange += OnNewSceneLoading;
        FlexSceneManager.OnLoadSceneEnd += OnNewSceneLoaded;

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        FlexSceneManager.OnLoadSceneStart -= OnNewSceneLoadingStarted;
        FlexSceneManager.OnLoadScenePercentChange -= OnNewSceneLoading;
        FlexSceneManager.OnLoadSceneEnd -= OnNewSceneLoaded;

        OnClientDisconnected?.Invoke();
    }

    int PlayerIndex = 0;
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (!networkConnections.Contains(conn))
        {
            bool IsLeader = NetworkRoomPlayers.Count == 0;
            RoomPlayer RoomPlayerInstance = Instantiate(playerPrefab).GetComponent<RoomPlayer>();
            NetworkServer.AddPlayerForConnection(conn, RoomPlayerInstance.gameObject);
            RoomPlayerInstance._Team = (NetworkRoomPlayers.Count > 1) ? Team.Blue : Team.Red;
            RoomPlayerInstance.isLeader = IsLeader;

            NetworkRoomPlayers.Add(RoomPlayerInstance);
            networkConnections.Add(conn);
            
            FlexSceneManager.OnServerAddPlayer(conn);
        }
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        FlexSceneManager.OnServerConnect(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<RoomPlayer>();
            NetworkRoomPlayers.Remove(player);
        }
        FlexSceneManager.OnServerDisconnect(conn);
        base.OnServerDisconnect(conn);
    }
    
    public void DeleteClientFromNetwork(NetworkConnection Conn)
    {
        //NetworkServer.DestroyPlayerForConnection(Conn);
        StopClient();
    }

    public override void OnStopClient()
    {
        networkConnections.Clear();
        LocalLevelManager.Inst.DeleteNMGC(this.gameObject);
    }
    
    bool IsReadyToStart()
    {
        if(numPlayers < MinPlayers) { return false; }
        foreach (var item in NetworkRoomPlayers)
        {
            if (!item.IsReady)
            { return false; }
        }

        return true;
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        FlexSceneManager.ResetInitialLoad();
        OnClientConnected?.Invoke();
    }
    
    public void AllPlayersPresent()
    {
        for (int i = 0; i < NetworkRoomPlayers.Count; i++)
        {
            Team _team = NetworkRoomPlayers[i]._Team;
            
            var conn = NetworkRoomPlayers[i].connectionToClient;
            Debug.Log("The team this network room player "+" {"+NetworkRoomPlayers.Count+"} "+"  is, and the current i is :-" + _team + "  " + i);
            
            if (_team == Team.Red)
            {
                var RedPlayerInstance = Instantiate(_NGPPrefab);

                RedPlayerInstance.SetDisplayName(NetworkRoomPlayers[i].DisplayName);

                //NetworkRoomPlayers.Remove(NetworkRoomPlayers[i]);
                NetworkGamePlayers.Add(RedPlayerInstance);

                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, RedPlayerInstance.gameObject, true);

                FlexSceneManager.OnServerAddPlayer(conn);

                RedPlayerInstance._Team = _team;
            }

            if (_team == Team.Blue)
            {
                var BluePlayerInstance = Instantiate(_NGPPrefab);

                BluePlayerInstance.SetDisplayName(NetworkRoomPlayers[i].DisplayName);

                //NetworkRoomPlayers.Remove(NetworkRoomPlayers[i]);
                NetworkGamePlayers.Add(BluePlayerInstance);

                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, BluePlayerInstance.gameObject, true);

                FlexSceneManager.OnServerAddPlayer(conn);

                BluePlayerInstance._Team = _team;
            }

            if(_team == Team.Spectator)
            {
                Debug.Log("Spectator Found");

                var SpectatorInstance = Instantiate(_SpectatorPrefab);

                //NetworkRoomPlayers.Remove(NetworkRoomPlayers[i]);
                Spectators.Add(SpectatorInstance);


                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, SpectatorInstance, true);

                FlexSceneManager.OnServerAddPlayer(conn);
            }
        }
        NetworkRoomPlayers.Clear();
    }
    
    void OnNewSceneLoadingStarted(LoadSceneStartEventArgs LSSEA)
    {
        //Debug.Log("current levels build index is " + SceneManager.GetActiveScene().buildIndex);
        
        //if (SceneManager.GetActiveScene().buildIndex == 1 && !IsServer)
        //    LoadingScreenFeedBack.PlayFeedbacks();
    }

    void OnNewSceneLoading(LoadScenePercentEventArgs LSPEA)
    {
        //Debug.Log("Current Load Percent " + LSPEA.Percent * 100);
    }

    void OnNewSceneLoaded(LoadSceneEndEventArgs LSEEA)
    {
        //Debug.Log("current levels build index is " + SceneManager.GetActiveScene().buildIndex);

        if (SceneManager.GetActiveScene().buildIndex == 1 && IsServer)
        {
            PlayerLobbyCanvas PLC = Instantiate(_PLC).GetComponent<PlayerLobbyCanvas>();       
            NetworkServer.Spawn(PLC.gameObject);
            SpawnedPLC = PLC;
            PLC.UpdateDisplay();
        }

        if (SceneManager.GetActiveScene().buildIndex > 1 && IsServer)
        {
            AllPlayersPresent();
        }

        if (!IsServer)
        {
            LoadingScreenFeedBack.PlayFeedbacks();
        }
    }

    public bool AllPlayersReady = false;
    public bool FirstInitialize = true;
    //[Server]
    private void Update()
    {
        if (!IsServer) return;

        if(SceneManager.GetActiveScene().buildIndex > 1 && FirstInitialize && AllPlayersReady)
        {
            if (LocalLevelManager.Inst == null) return;
            if (NetworkGamePlayers.Count + Spectators.Count != networkConnections.Count) return;

            FirstInitialize = false;

            LocalLevelManager.Inst.PopulateListValues();

            SpawnBodyAndGM();

            for (int i = 0; i < NetworkGamePlayers.Count; i++)
            {
                if (NetworkGamePlayers[i]._Team == Team.Blue)
                    BlueGamePlayers.Add(NetworkGamePlayers[i]);
                else if (NetworkGamePlayers[i]._Team == Team.Red)
                    RedGamePlayers.Add(NetworkGamePlayers[i]);
            }

            for (int i = 0; i < BlueGamePlayers.Count; i++)
            {
                BlueGamePlayers[i].transform.position = LocalLevelManager.Inst.BlueSpawns[i].position;
                BlueGamePlayers[i].transform.rotation = LocalLevelManager.Inst.BlueSpawns[i].rotation;
                BlueGamePlayers[i].CLNT_CorrectPlayerTransform(LocalLevelManager.Inst.BlueSpawns[i].position, LocalLevelManager.Inst.BlueSpawns[i].rotation);
            }

            for (int i = 0; i < RedGamePlayers.Count; i++)
            {
                RedGamePlayers[i].transform.position = LocalLevelManager.Inst.RedSpawns[i].position;
                RedGamePlayers[i].transform.rotation = LocalLevelManager.Inst.RedSpawns[i].rotation;
                RedGamePlayers[i].CLNT_CorrectPlayerTransform(LocalLevelManager.Inst.RedSpawns[i].position, LocalLevelManager.Inst.RedSpawns[i].rotation);
            }

            StartCoroutine(FiveSecDelay());
        }
    }

    private IEnumerator FiveSecDelay()
    {

        yield return new WaitForSeconds(5);
        for (int i = 0; i < NetworkGamePlayers.Count; i++)
        {
            NetworkGamePlayers[i]._MH.IsMovementAllowed = true;
            NetworkGamePlayers[i]._UIM.UpdateGMHealthUI = true;
        }

        GameManager.Inst.StartTimer();
    }

    public void OnLoadingFeedbackFinished()
    {
        if(LoadingScreenFeedBack.Direction == MMFeedbacks.Directions.BottomToTop) LSImages.SetActive(false);
    }
}
