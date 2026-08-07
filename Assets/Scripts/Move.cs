using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using FirstGearGames.Mirrors.Assets.FlexNetworkAnimators;

public class Move : NetworkBehaviour
{
    public float MoveSpeed;

    public Animator animator;
    public FlexNetworkAnimator _Fna;

    public Canvas _canvas;
    public TMP_Text _Text;
    
    Vector3 Movement;

    private void Start()
    {
        //_canvas.worldCamera = NM_GC.Inst.WorldCam;

        //_Text.text = isLocalPlayer.ToString();
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKey(KeyCode.W))
        {
            Movement += Vector3.forward;
            //_Text.text = "W pressed";
        }

        if (Input.GetKey(KeyCode.A))
        {
            Movement += Vector3.left;
            //_Text.text = "A pressed";
        }

        if (Input.GetKey(KeyCode.S))
        {
            Movement += Vector3.back;
            //_Text.text = "S pressed";
        }

        if (Input.GetKey(KeyCode.D))
        {
            Movement += Vector3.right;
            //_Text.text = "D pressed";
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Asking For player number");
            AskForMyPlayerNumber();
        }

        animator.SetFloat("Right", Movement.x);
        animator.SetFloat("Forward", Movement.z);

        transform.position += Movement * MoveSpeed * Time.deltaTime;
        Movement = Vector3.zero;
    }

    [Command]
    void AskForMyPlayerNumber()
    {
        Debug.Log("Recieved a request to identify players playernumber");
        //ReturnPlayerNumber(NetworkServer.connections[connectionToClient.connectionId]);
    }

    [ClientRpc]
    void ReturnPlayerNumber(int PNum)
    {
        _Text.text = PNum.ToString();
    }
}