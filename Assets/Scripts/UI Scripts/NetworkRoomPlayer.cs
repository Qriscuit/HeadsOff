using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using HeadsOffGlobals;
using FirstGearGames.FlexSceneManager.LoadUnloadDatas;
using FirstGearGames.FlexSceneManager.Events;
using FirstGearGames.FlexSceneManager;

public class NetworkRoomPlayer : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private GameObject LoadingPage = null;
    [SerializeField] public TMP_Text LoadPercentage;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] private Button startGameButton = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    [SyncVar]
    public Team _team;

    [SyncVar(hook = nameof(IsLeaderSet))]
    public bool isLeader;
    public void IsLeaderSet(bool OldVlaue, bool NewValue)
    {
        isLeader = NewValue;
        startGameButton.interactable = NewValue;
    }

    private NM_GC room;
    private NM_GC Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NM_GC;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if (!isLocalPlayer)
            lobbyUI.gameObject.SetActive(false);
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerInputName.Inst.DisplayName);
        
        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        //Room.NetworkRoomPlayers.Add(this);
        Debug.Log("is leader " + isLeader);
        FlexSceneManager.OnLoadSceneStart += OnNewSceneLoadingStarted;
        FlexSceneManager.OnLoadScenePercentChange += OnNewSceneLoading;
        FlexSceneManager.OnLoadSceneEnd += OnNewSceneLoaded;
        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        //Room.NetworkRoomPlayers.Remove(this);
        FlexSceneManager.OnLoadSceneStart -= OnNewSceneLoadingStarted;
        FlexSceneManager.OnLoadScenePercentChange -= OnNewSceneLoading;
        FlexSceneManager.OnLoadSceneEnd -= OnNewSceneLoaded;
        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        //if (!hasAuthority)
        //{
        //    foreach (var player in Room.NetworkRoomPlayers)
        //    {
        //        if (player.hasAuthority)
        //        {
        //            //player.UpdateDisplay();
        //            break;
        //        }
        //    }

        //    return;
        //}

        //for (int i = 0; i < playerNameTexts.Length; i++)
        //{
        //    playerNameTexts[i].text = "Waiting For Player...";
        //    playerReadyTexts[i].text = string.Empty;
        //}

        //for (int i = 0; i < Room.NetworkRoomPlayers.Count; i++)
        //{
        //    playerNameTexts[i].text = Room.NetworkRoomPlayers[i].DisplayName;
        //    playerReadyTexts[i].text = Room.NetworkRoomPlayers[i].IsReady ?
        //        "<color=green>Ready</color>" :
        //        "<color=red>Not Ready</color>";
        //}
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader) { return; }

        startGameButton.interactable = readyToStart;
    }

    public void Update()
    {
        Debug.Log(hasAuthority);
    }

    private void OnDestroy()
    {

    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        //Room.NotifyPlayersOfReadyState();
    }

    public void startgame()
    {
        Debug.Log("check if level is chosen = " + LogInMainMenu.Inst.LevelChosen + " " + LogInMainMenu.Inst.LevelName);
        if (LogInMainMenu.Inst.LevelChosen)
            CmdStartGame(LogInMainMenu.Inst.LevelName);
    }

    [Command]
    public void CmdStartGame(string LevelName)
    {
        if (Room.NetworkRoomPlayers[0].connectionToClient != connectionToClient) { return; }
        Debug.Log("about to change level");

        SingleSceneData asd = null;
        if (LevelName != null)
            asd = new SingleSceneData(LevelName);

        FlexSceneManager.LoadNetworkedScenes(asd, null);

        //Room.StartGameLoading(LevelName);
    }
    
    void OnNewSceneLoadingStarted(LoadSceneStartEventArgs LSSEA)
    {
        Debug.Log("current levels build index is " + SceneManager.GetActiveScene().buildIndex);
        LoadingPage.SetActive(true);
        lobbyUI.SetActive(false);
    }

    void OnNewSceneLoading(LoadScenePercentEventArgs LSPEA)
    {
        LoadPercentage.text = LSPEA.Percent + "...";
        Debug.Log("Current Load Percent " + LSPEA.Percent);
    }

    void OnNewSceneLoaded(LoadSceneEndEventArgs LSEEA)
    {
        LoadingPage.SetActive(false);
        lobbyUI.SetActive(true);
        Debug.Log("current levels build index is " + SceneManager.GetActiveScene().buildIndex);
        Debug.Log("FUUUUUUUUUUUUUCCCCCCCKKKKKKK");
        AddReadiedPlayer();
    }

    [Command]
    void AddReadiedPlayer()
    {
        room.RediedRoomPlayers.Add(this);

        Debug.Log("Redied players " + room.RediedRoomPlayers.Count);
        Debug.Log("Network Room players " + room.NetworkRoomPlayers.Count);

        if (room.RediedRoomPlayers.Count == room.NetworkRoomPlayers.Count)
        {
            room.AllPlayersPresent();
        }
    } 
}


/*
 [Header("UI")]
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private GameObject LoadingPage = null;
    [SerializeField] public TMP_Text LoadPercentage;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] private Button startGameButton = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    [SyncVar]
    public Team _team;

    [SyncVar(hook = nameof(IsLeaderSet))]
    public bool isLeader;
    public void IsLeaderSet(bool OldVlaue, bool NewValue)
    {
        isLeader = NewValue;
        startGameButton.interactable = NewValue;
    }

    private NM_GC room;
    private NM_GC Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NM_GC;
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if (!isLocalPlayer)
            lobbyUI.gameObject.SetActive(false);
    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerInputName.Inst.DisplayName);
        
        lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.NetworkRoomPlayers.Add(this);
        Debug.Log("is leader " + isLeader);
        FlexSceneManager.OnLoadSceneStart += OnNewSceneLoadingStarted;
        FlexSceneManager.OnLoadScenePercentChange += OnNewSceneLoading;
        FlexSceneManager.OnLoadSceneEnd += OnNewSceneLoaded;
        UpdateDisplay();
        Debug.Log(NetworkServer.connections.Count);
    }

    public override void OnStopClient()
    {
        Room.NetworkRoomPlayers.Remove(this);
        FlexSceneManager.OnLoadSceneStart -= OnNewSceneLoadingStarted;
        FlexSceneManager.OnLoadScenePercentChange -= OnNewSceneLoading;
        FlexSceneManager.OnLoadSceneEnd -= OnNewSceneLoaded;
        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            foreach (var player in Room.NetworkRoomPlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
            playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < Room.NetworkRoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.NetworkRoomPlayers[i].DisplayName;
            playerReadyTexts[i].text = Room.NetworkRoomPlayers[i].IsReady ?
                "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader) { return; }

        startGameButton.interactable = readyToStart;
    }
    
    private void OnDestroy()
    {

    }

    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        //Room.NotifyPlayersOfReadyState();
    }

    public void startgame()
    {
        Debug.Log("check if level is chosen = " + LogInMainMenu.Inst.LevelChosen + " " + LogInMainMenu.Inst.LevelName);
        if (LogInMainMenu.Inst.LevelChosen)
            CmdStartGame(LogInMainMenu.Inst.LevelName);
    }

    [Command]
    public void CmdStartGame(string LevelName)
    {
        if (Room.NetworkRoomPlayers[0].connectionToClient != connectionToClient) { return; }
        Debug.Log("about to change level");

        SingleSceneData asd = null;
        if (LevelName != null)
            asd = new SingleSceneData(LevelName);

        FlexSceneManager.LoadNetworkedScenes(asd, null);

        //Room.StartGameLoading(LevelName);
    }
    
    void OnNewSceneLoadingStarted(LoadSceneStartEventArgs LSSEA)
    {
        Debug.Log("current levels build index is " + SceneManager.GetActiveScene().buildIndex);
        LoadingPage.SetActive(true);
        lobbyUI.SetActive(false);
    }

    void OnNewSceneLoading(LoadScenePercentEventArgs LSPEA)
    {
        LoadPercentage.text = LSPEA.Percent + "...";
        Debug.Log("Current Load Percent " + LSPEA.Percent);
    }

    void OnNewSceneLoaded(LoadSceneEndEventArgs LSEEA)
    {
        LoadingPage.SetActive(false);
        lobbyUI.SetActive(true);
        Debug.Log("current levels build index is " + SceneManager.GetActiveScene().buildIndex);

        AddReadiedPlayer();
    }

    [Command]
    void AddReadiedPlayer()
    {
        room.RediedRoomPlayers.Add(this);

        if(room.RediedRoomPlayers.Count == room.NetworkRoomPlayers.Count)
        {
            room.AllPlayersPresent();
        }
    } 
     */
