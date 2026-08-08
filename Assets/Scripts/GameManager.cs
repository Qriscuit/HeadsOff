using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using HeadsOffGlobals;
using UnityEngine.UI;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public bool GameEnded = true;

    public GameObject RedVictory, RedDefeat;
    public GameObject BlueVictory, BlueDefeat;

    public Image Red1Health;
    public Image Red2Health;

    public Image Blue1Health;
    public Image Blue2Health;

    public TMP_Text TXT_RedScore;
    public TMP_Text TXT_BlueScore;

    public TMP_Text TXT_Time;

    [Header("Death Notifications")]
    public Transform NotificationParent;
    public GameObject KillBlock;

    public Canvas _canvas;

    private NM_GC room;
    private NM_GC Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NM_GC;
        }
    }

    [SyncVar]
    public int RedScore = 0;
    [SyncVar]
    public int BlueScore = 0;

    public static GameManager Inst;
    private void Awake()
    {
        Inst = this;
    }
    
    public void CMD_UpdateDisplay()
    {
        TXT_RedScore.text = RedScore.ToString();
        TXT_BlueScore.text = BlueScore.ToString();

        CLNT_UpdateDisplay(RedScore, BlueScore);
    }

    [ClientRpc]
    public void CLNT_UpdateDisplay(int redScore, int blueScore)
    {
        TXT_RedScore.text = redScore.ToString();
        TXT_BlueScore.text = blueScore.ToString();
    }

    public void CMD_RedScored(bool DidBodyDie)
    {
        if (DidBodyDie)
            RedScore += 3;
        else
            RedScore += 5;

        CMD_UpdateDisplay();
    }
    
    public void CMD_BlueScored(bool DidBodyDie)
    {
        if(DidBodyDie)
            BlueScore += 3;
        else
            BlueScore += 5;

        CMD_UpdateDisplay();
    }

    int TotalSeconds = 300;
    public void StartTimer()
    {
        CLNT_StartTimer();
        StartCoroutine(Timer());
    }

    [ClientRpc]
    void CLNT_StartTimer()
    {
        StartCoroutine(Timer());

        foreach (BasePlayerManager item in Room.NetworkGamePlayers)
        {
            if (isServer)
                return;

            if (room.LocalBPM == null) return;

            if (!item.isLocalPlayer)
            {
                item._UIM.HealthCanvas.worldCamera = room.LocalBPM._Cam;

                if(item._Team == room.LocalBPM._Team)
                {
                    item._UIM.HealthCanvas.gameObject.SetActive(true);
                    item._UIM.FriendHealth.gameObject.SetActive(true);
                    item._UIM.EnemyHealth.gameObject.SetActive(false);
                    item._UIM.HealthCanvas.gameObject.SetActive(false);
                }
                else
                {
                    item._UIM.HealthCanvas.gameObject.SetActive(true);
                    item._UIM.FriendHealth.gameObject.SetActive(false);
                    item._UIM.EnemyHealth.gameObject.SetActive(true);
                    item._UIM.HealthCanvas.gameObject.SetActive(false);
                }
            }

            //item._MH.IsMovementAllowed = false;
            //item._MH.IsPlayerSpinAllowed = false;
        }
    }

    IEnumerator Timer()
    {
        while(TotalSeconds > -1)
        {
            ResetTimer:
            yield return new WaitForSeconds(1);
            TotalSeconds--;
            
            TXT_Time.text = (TotalSeconds / 60) + ":" + (TotalSeconds % 60).ToString();

            if(TotalSeconds == 0)
            {
                if (RedScore == BlueScore)
                {
                    TotalSeconds = 30;
                    goto ResetTimer;
                }

                StartCoroutine(TimeSlowDown());
                break;
            }
        }
    }

    bool DisplayEndUI = false;
    IEnumerator TimeSlowDown()
    {
        while(Time.timeScale > 0)
        {
            yield return new WaitForSeconds(0.01f);
            if (Time.timeScale - 0.02f < 0)
                Time.timeScale = 0;
            else
                Time.timeScale -= 0.02f;

            if(Time.timeScale < 0.3f && !DisplayEndUI)
            {
                DisplayEndUI = true;


                if (isServer)
                {
                    GameEnded = true;
                    foreach (BasePlayerManager item in Room.NetworkGamePlayers)
                    {
                        //item._MH.IsMovementAllowed = false;
                        //item._MH.IsPlayerSpinAllowed = false;
                    }
                }
                else
                {
                    if (room.LocalBPM == null) break;
                    GameEnded = true;
                    Debug.Log("only in the client");
                    Debug.Log("The blue score is " + BlueScore + " the red score is " + RedScore);

                    if (RedScore > BlueScore)
                    {
                        if (Room.LocalBPM._Team == Team.Red)
                            RedVictory.SetActive(true);
                        else
                            BlueDefeat.SetActive(true);
                    }
                    else
                    {
                        if (Room.LocalBPM._Team == Team.Red)
                            RedDefeat.SetActive(true);
                        else
                            BlueVictory.SetActive(true);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if(_canvas.gameObject.activeSelf)
                _canvas.gameObject.SetActive(false);
            else
                _canvas.gameObject.SetActive(true);
        }
        if (!GameEnded) return;


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isServer)
            {
                Debug.Log("escape pressed");
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Room.StopServer();
            }
        }
    }

    [ClientRpc]
    public void CLNT_SpawnDeathNotification(string KillersName, string KilledsName, bool diddyakillHead)
    {
        Debug.Log("Client death notification spawn function called");

        GameObject Notif = Instantiate(KillBlock, NotificationParent);

        Notif.GetComponent<KillBlockUI>().CreateNewKillBlockUI(KillersName, KilledsName, diddyakillHead);
    }

    public void SpawnDeathNotification(string KillersName, string KilledsName, bool diddyakillHead)
    {
        Debug.Log("Server death notification spawn function called");

        CLNT_SpawnDeathNotification(KillersName, KilledsName, diddyakillHead);
    }
}
