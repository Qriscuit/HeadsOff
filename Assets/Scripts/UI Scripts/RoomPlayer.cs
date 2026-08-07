using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using Mirror;
using FirstGearGames.FlexSceneManager.LoadUnloadDatas;
using FirstGearGames.FlexSceneManager.Events;
using FirstGearGames.FlexSceneManager;
using HeadsOffGlobals;

public class RoomPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;
    [SyncVar(hook = nameof(IsLeaderSet))]
    public bool isLeader = false;

    [SyncVar(hook = nameof(HandleTeamNameChanged))]
    public Team _Team = Team.Red;

    public string SceneToGoToName = "";

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplayCall();
    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplayCall();
    public void HandleTeamNameChanged(Team oldValue, Team newValue) => UpdateDisplayCall();

    private void UpdateDisplayCall()
    {
        if (PlayerLobbyCanvas.Inst != null) PlayerLobbyCanvas.Inst.UpdateDisplay();
        else Debug.Log("PlayerLobbyCanvas aint ready Yet");
    }
    
    public void IsLeaderSet(bool OldVlaue, bool NewValue) => ChangeScene();

    private void ChangeScene()
    {
        SceneToGoToName = LogInMainMenu.Inst.LevelName;
        LoadLobbyLevel();
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

    public override void OnStartClient()
    {
        FlexSceneManager.OnLoadSceneStart += OnNewSceneLoadingStarted;
        FlexSceneManager.OnLoadScenePercentChange += OnNewSceneLoading;
        FlexSceneManager.OnLoadSceneEnd += OnNewSceneLoaded;

        if (isLocalPlayer)
        {
            Room.LocalNRP = this;
        }

        if (!Room.NetworkRoomPlayers.Contains(this))
            Room.NetworkRoomPlayers.Add(this);
    }

    public override void OnStopClient()
    {
        FlexSceneManager.OnLoadSceneStart -= OnNewSceneLoadingStarted;
        FlexSceneManager.OnLoadScenePercentChange -= OnNewSceneLoading;
        FlexSceneManager.OnLoadSceneEnd -= OnNewSceneLoaded;

        if (Room.NetworkRoomPlayers.Contains(this))
            Room.NetworkRoomPlayers.Remove(this);
    }

    [Command]
    void CMD_SetName(string Name)
    {
        DisplayName = Name;
        
        //PlayerLobbyCanvas.Inst.UpdateDisplay();
    }

    void OnNewSceneLoadingStarted(LoadSceneStartEventArgs LSSEA)
    {
        //Debug.Log("current levels build index is " + SceneManager.GetActiveScene().buildIndex);
    }

    void OnNewSceneLoading(LoadScenePercentEventArgs LSPEA)
    {
        //Debug.Log("Current Load Percent " + LSPEA.Percent * 100);
    }

    void OnNewSceneLoaded(LoadSceneEndEventArgs LSEEA)
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            //PlayerLobbyCanvas.Inst.UpdateDisplay();
            //Debug.Log(Room.LocalPlayersName);
            //CMD_SetName(Room.LocalPlayersName);
        }
    }

    [Command]
    void LoadLobbyLevel()
    {
        SingleSceneData ssd = new SingleSceneData("Player Lobby", new NetworkIdentity[] { netIdentity });
        FlexSceneManager.LoadNetworkedScenes(ssd, null);
    }

    [Command]
    public void CMD_ReadyUp()
    {
        IsReady = !IsReady;
        PlayerLobbyCanvas.Inst.UpdateDisplay();
    }

    bool FirstInitialize = true;
    //[Client]
    private void Update()
    {
        if (isServer) return;

        if (Input.GetKeyDown(KeyCode.M) && isLocalPlayer)
        {
            CMD_SwitchTeam();
        }

        if (Input.GetKeyDown(KeyCode.P) && isLocalPlayer)
        {
            CMD_BecomeSpectator();
        }

        if (FirstInitialize && PlayerLobbyCanvas.Inst != null && isLocalPlayer)
        {
            CMD_SetName(Room.LocalPlayersName);
            //UpdateDisplayCall();
            FirstInitialize = false;

            if(isLocalPlayer)
            PlayerLobbyCanvas.Inst.startGameButton.interactable = isLeader;
        }
    }
    
    public void LoadNextLevel()
    {
        if(isLeader) CMD_StartNextLevel(SceneToGoToName);
    }

    [Command]
    void CMD_StartNextLevel(string SceneName)
    {
        StartLoadingScreens();
        StartCoroutine(LoadScene(SceneName));
    }

    IEnumerator LoadScene(string SceneName)
    {
        yield return new WaitForSeconds(2f);

        int Index = 0;
        NetworkIdentity[] NewNetIds = new NetworkIdentity[Room.NetworkRoomPlayers.Count];
        foreach (var item in Room.NetworkRoomPlayers)
        {
            NewNetIds[Index] = item.netIdentity;
            Index++;
        }

        SingleSceneData ssd = null;
        if (SceneName != null)
            ssd = new SingleSceneData(SceneName, NewNetIds);

        FlexSceneManager.LoadNetworkedScenes(ssd, null);
    }
    

    [ClientRpc]
    void StartLoadingScreens()
    {
        AudioManger.Inst.MenuMusic.Stop();
        AudioManger.Inst.GameMusic.Play();
        Room.LoadingScreenFeedBack.PlayFeedbacks();
    }

    [Command]
    public void CMD_SwitchTeam()
    {
        _Team = (_Team == Team.Red) ? Team.Blue : Team.Red;
    }

    [Command]
    public void CMD_BecomeSpectator()
    {
        _Team = Team.Spectator;
    }
}
