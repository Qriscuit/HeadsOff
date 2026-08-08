using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using FirstGearGames.FlexSceneManager.LoadUnloadDatas;
using FirstGearGames.FlexSceneManager.Events;
using FirstGearGames.FlexSceneManager;

public class PlayerLobbyCanvas : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] public Canvas _canvas;
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private GameObject LoadingPage = null;
    [SerializeField] public TMP_Text LoadPercentage;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] public Image[] PlayerBGs = new Image[4];
    [SerializeField] public Button startGameButton = null;
    [SerializeField] public Button SwitchTeamButton = null;

    [Space]

    public Sprite BlueNotReady;
    public Sprite RedReady;
    public Sprite RedNotReady;
    public Sprite BlueReady;
    public Sprite YellowReady;
    public Sprite YellowNotReady;

    [Space]

    public Image OceanFactory;
    public Image Skyscraper;
    public Image JunkYard;

    public static PlayerLobbyCanvas Inst;
    public void Awake()
    {
        Inst = this;
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

    public void UpdateDisplay()
    {
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting...";
        }

        int N = (Room.NetworkRoomPlayers.Count > 4) ? 4 : Room.NetworkRoomPlayers.Count;

        for (int i = 0; i < N; i++)
        {
            playerNameTexts[i].text = Room.NetworkRoomPlayers[i].DisplayName;

            if (Room.NetworkRoomPlayers[i]._Team == HeadsOffGlobals.Team.Red)
            {
                if (Room.NetworkRoomPlayers[i].IsReady)
                    PlayerBGs[i].sprite = RedReady;
                else
                    PlayerBGs[i].sprite = RedNotReady;
            }
            else if (Room.NetworkRoomPlayers[i]._Team == HeadsOffGlobals.Team.Blue)
            {
                if (Room.NetworkRoomPlayers[i].IsReady)
                    PlayerBGs[i].sprite = BlueReady;
                else
                    PlayerBGs[i].sprite = BlueNotReady;
            }
            else if (Room.NetworkRoomPlayers[i]._Team == HeadsOffGlobals.Team.Spectator)
            {

            }
        }
    }

    [ClientRpc]
    public void CLNT_UpdateDisplay()
    {
        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting...";
        }

        for (int i = 0; i < Room.NetworkRoomPlayers.Count; i++)
        {
            playerNameTexts[i].text = Room.NetworkRoomPlayers[i].DisplayName;

            if (Room.NetworkRoomPlayers[i]._Team == HeadsOffGlobals.Team.Red)
            {
                if (Room.NetworkRoomPlayers[i].IsReady)
                    PlayerBGs[i].sprite = RedReady;
                else
                    PlayerBGs[i].sprite = RedNotReady;
            }
            else
            {
                if (Room.NetworkRoomPlayers[i].IsReady)
                    PlayerBGs[i].sprite = BlueReady;
                else
                    PlayerBGs[i].sprite = BlueNotReady;
            }
        }
    }

    public void startgame()
    {
        Room.LocalNRP.LoadNextLevel();
    }

    [Command]
    public void CMD_StartGame(string LevelName)
    {
        Debug.Log(LevelName);
        int Index = 0;
        NetworkIdentity[] NewNetIds = new NetworkIdentity[Room.NetworkRoomPlayers.Count];
        foreach (var item in Room.NetworkRoomPlayers)
        {
            NewNetIds[Index] = item.netIdentity;
            Index++;
        }
        
        SingleSceneData asd = null;
        if (LevelName != null)
            asd = new SingleSceneData(LevelName, NewNetIds);

        FlexSceneManager.LoadNetworkedScenes(asd, null);

        //Room.StartGameLoading(LevelName);
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        //if (!isLeader) { return; }

        startGameButton.interactable = readyToStart;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _canvas.worldCamera = Levelloader.Inst._camera;
    }
    
    public void ReadyUpLocalPlayer()
    {
        AudioManger.Inst.ButtonClick1.Play();
        Room.LocalNRP.CMD_ReadyUp();
    }

    public void OnClick_SwitchSides()
    {
        Room.LocalNRP.CMD_SwitchTeam();
    }
    
    IEnumerator StartClientAsLoadingScreensAnimationEnds()
    {
        yield return new WaitForSeconds(2f);
        Room.LocalNRP.LoadNextLevel();
    }
}
