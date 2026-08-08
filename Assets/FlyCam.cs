using UnityEngine;
using System.Collections;
using Mirror;

using FirstGearGames.FlexSceneManager.Events;
using FirstGearGames.FlexSceneManager;
using UnityEngine.SceneManagement;

public class FlyCam : NetworkBehaviour
{
    public float mainSpeed = 10.0f; //regular speed
    public float shiftAdd = 20.0f; //multiplied by how long shift is held.  Basically running
    public float maxShift = 80.0f; //Maximum speed when holdin gshift
    public float camSens = 0.10f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;

    private NM_GC room;
    private NM_GC Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NM_GC;
        }
    }

    private void Awake()
    {
        //lastMouse = (Vector2)Input.mousePosition;
    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.None;

        // TRY 1 - Failed because it not take mouse delta for some reason
        lastMouse = Input.mousePosition - lastMouse;
        lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
        transform.eulerAngles = lastMouse;
        lastMouse = Input.mousePosition;

        // TRY 2 - Weird Fail
        //Vector2 mouseDelta = (Vector2)Input.mousePosition - (Vector2)lastMouse;

        //Debug.Log("Users Mouse pos: " + Input.mousePosition);
        //Debug.Log("Mouse delta is: " + mouseDelta);

        //float rotationSpeed = 2.0f;
        //transform.Rotate(Vector3.up, mouseDelta.x * rotationSpeed);
        //transform.Rotate(Vector3.left, mouseDelta.y * rotationSpeed);

        //Mouse  camera angle done.  

        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            camSens -= 0.01f;
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            camSens += 0.01f;
        }


        //Keyboard commands
        float f = 0.0f;
        Vector3 p = GetBaseInput();
        if (p.sqrMagnitude > 0)
        { // only move while a direction key is pressed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                totalRun += Time.deltaTime;
                p = p * totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p = p * mainSpeed;
            }

            p = p * Time.deltaTime;
            Vector3 newPosition = transform.position;
            if (Input.GetKey(KeyCode.Space))
            { //If player wants to move on X and Z axis only
                transform.Translate(p);
                newPosition.x = transform.position.x;
                newPosition.z = transform.position.z;
                transform.position = newPosition;
            }
            else
            {
                transform.Translate(p);
            }
        }
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();

        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            p_Velocity += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            p_Velocity += new Vector3(0, -1, 0);
        }
        return p_Velocity;
    }


    public override void OnStartClient()
    {
        base.OnStartClient();

        Room.Spectators.Add(this.gameObject);



        if (isLocalPlayer)
        {
            FlexSceneManager.OnLoadSceneEnd += OnNewSceneLoaded;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void OnNewSceneLoaded(LoadSceneEndEventArgs LSEEA)
    {
        Debug.Log("New scene loaded for SPECTATOR");
        InformServerOfLocalPlayersLoadComplete(this.netIdentity);
    }

    [Command]
    void InformServerOfLocalPlayersLoadComplete(NetworkIdentity ID)
    {
        Debug.Log("Server being informed of the fact that local player is done loading whos SPECTATOR");
        Room.LocalClientsLoaded.Add(ID);

        Debug.Log("Local Clients Loaded = " + Room.LocalClientsLoaded.Count);
        Debug.Log("Network game players = " + Room.NetworkGamePlayers.Count);
        Debug.Log("Spectators    Loaded = " + Room.Spectators.Count);

        if (Room.LocalClientsLoaded.Count == Room.NetworkGamePlayers.Count + Room.Spectators.Count)
        {
            Room.AllPlayersReady = true;
        }
    }
}