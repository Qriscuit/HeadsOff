using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using HeadsOffGlobals;

public class MainMenuManager : MonoBehaviour
{
    public string NetworkAddress = "localhost";
    public NetworkManager NMGC;

    public GameObject ConnectionUI;
    public Team _team;

    [HideInInspector] public bool PlayingMagma = true;

    public static MainMenuManager Inst;
    private void Awake()
    {
        Inst = this;
    }

    public void StartServer()
    {
        NMGC.StartServer();
        ConnectionUI.SetActive(false);
    }

    public void SelectLocal()
    {
        NetworkAddress = "localhost";
    }

    public void MagmaLevel()
    {
        PlayingMagma = true;
    }

    public void SkyLevel()
    {
        PlayingMagma = false;
    }

    public void JoinRedClient()
    {
        NMGC.StartClient(NetworkAddress);
        ConnectionUI.SetActive(false);
        _team = Team.Red;
    }

    public void JoinBlueClient()
    {
        NMGC.StartClient(NetworkAddress);
        ConnectionUI.SetActive(false);
        _team = Team.Blue;
    }

    public void InitiateHost()
    {
        NMGC.StartHost();
        ConnectionUI.SetActive(false);
    }
    
    public Team NM_GC_OnClientConnected()
    {
        ConnectionUI.SetActive(false);
        return _team;
    }

    private void OnEnable()
    {
        //NM_GC.OnClientConnected += NM_GC_OnClientConnected;
    }

    private void OnDisable()
    {
        //NM_GC.OnClientConnected -= NM_GC_OnClientConnected;
    }

    //IEnumerator ProcessingTime(Team _team)
    //{
    //    yield return new WaitForSeconds(1.5f);
    //    GameManager.Inst.LocalTeam = _team;
    //    ConnectionUI.SetActive(false);
    //}
}
