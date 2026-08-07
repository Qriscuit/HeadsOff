using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;
using Mirror;

public class MechanicManager : NetworkBehaviour
{
    BasePlayerManager _BPM;
    public List<GameObject> BodiesCloseBy;

    [Header("Head Launch")]
    [SerializeField] bool isLaunching;
    [SerializeField] float LaunchSpeed;
    [SerializeField] float H_LaunchTime;
    private void Awake()
    {
        _BPM = GetComponent<BasePlayerManager>();
    }

    private void OnEnable()
    {
        _BPM._IM._HeadDeAt_tachmentStarted += HeadAttachDetachButton;
        _BPM._IM._BodyPartSwapStarted += BodySwapButton;
        _BPM._IM._ChestAbility += OtherInputCheck;
        _BPM._IM._HeadThrowStarted += HeadLaunch;
    }

    private void OnDisable()
    {
        _BPM._IM._HeadDeAt_tachmentStarted -= HeadAttachDetachButton;
        _BPM._IM._BodyPartSwapStarted -= BodySwapButton;
        _BPM._IM._ChestAbility -= OtherInputCheck;
        _BPM._IM._HeadThrowStarted -= HeadLaunch;
    }

    bool IsOtherButtonPressed;
    void OtherInputCheck(bool Button)
    {
        IsOtherButtonPressed = Button;
    }

    void HeadAttachDetachButton(bool HeadAttachDetachButton)
    {

        // If youre near a body and someone occupies that body, the problem is the reffernece of, a now, Occupied body  which fails on line 46
        Occupied:
        if (BodiesCloseBy.Count > 0)
        {
            if (BodiesCloseBy[0].tag == "Occupied")
            {
                BodiesCloseBy.RemoveAt(0);
                goto Occupied;
            }
        }


        if (_BPM.BodyInPossession == null && BodiesCloseBy.Count > 0 && BodiesCloseBy[0].tag == "Unoccupied")
        {
            //AttachToBody(BodiesCloseBy[0].GetComponent<Body>());
            CMD_requestAuthorityForBody(BodiesCloseBy[0].GetComponent<Body>().netIdentity);
            _BPM._VFX.CMD_VFX_HeadLaunch();
            _BPM._CMHead.gameObject.SetActive(false);
            _BPM._CMBody.gameObject.SetActive(true);
        }
        else if (_BPM.BodyInPossession != null)
        {
            if (!_BPM._CC.isGrounded)
                return;

            _BPM._VFX.CMD_VFX_HeadLaunchBody();
            Debug.Log("Is body in possession ?: " + _BPM.BodyInPossession);
            CMD_giveBackBodyAuthorityToServer(transform.eulerAngles);
            _BPM._CMHead.gameObject.SetActive(true);
            _BPM._CMBody.gameObject.SetActive(false);
        }
    }

    void BodySwapButton(Vector2 value)
    {
        if (IsOtherButtonPressed) return;

        if (value.x==-1 && _BPM.BodyInPossession != null && BodiesCloseBy.Count > 0)
        {
            SwitchLeftHand();
        }

        if (value.y==-1 && _BPM.BodyInPossession != null && BodiesCloseBy.Count > 0)
        {
            SwitchLegOut();
        }

        if (value.x==1 && _BPM.BodyInPossession != null && BodiesCloseBy.Count > 0)
        {
            SwitchRightHand();
        }

        if (value.y==1 && _BPM.BodyInPossession != null && BodiesCloseBy.Count > 0)
        {
            SwitchChestOut();
        }
    }

    [Command]
    void CMD_requestAuthorityForBody(NetworkIdentity ID)
    {
        ID.AssignClientAuthority(base.connectionToClient);

        _BPM.BodyInPossession = ID.GetComponent<Body>();

        _BPM.BodyInPossession.PlayerBodyIsAttachedTo = _BPM;

        ID.gameObject.tag = "Occupied";

        CLNT_JoinBodyCallBack(ID);
    }

    [ClientRpc]
    void CLNT_JoinBodyCallBack(NetworkIdentity ID)
    {
        _BPM.BodyInPossession = ID.GetComponent<Body>();

        if(BodiesCloseBy.Count!=0)
        BodiesCloseBy.RemoveAt(0);


        ID.gameObject.tag = "Occupied";
        _BPM.HeadMesh.enabled = false;
        _BPM._NNBody.gameObject.SetActive(true);
        _BPM._NNBody.Head.gameObject.SetActive(true);
        _BPM._NNBody.Abdomen.gameObject.SetActive(true);
        _BPM._NNBody.UpdateMesh(_BPM.BodyInPossession);
        //MyBody.transform.localPosition = ID.gameObject.transform.position;

        _BPM.BodyInPossession.UpdateMesh();

        Body Buffer = ID.gameObject.GetComponent<Body>();

        Buffer.HeadRenderer.enabled = false;
        Buffer.ChestRenderer.enabled = false;
        Buffer.AbdomenRenderer.enabled = false;
        Buffer.RightHandRenderer.enabled = false;
        Buffer.LeftHandRenderer.enabled = false;
        Buffer.LegRenderer.enabled = false;

        Buffer.TurnOffAccentsNVfx();

        Buffer._boxCollider.enabled = false;

        Buffer.PlayerBodyIsAttachedTo = _BPM;

        Debug.Log("client now has the authority of this object: " + ID.gameObject.name);
    }


    [Command]
    public void CMD_giveBackBodyAuthorityToServer(Vector3 Euler)
    {
        Debug.Log("leaving body");

        _BPM.BodyInPossession.gameObject.tag = "Unoccupied";

        _BPM.BodyInPossession.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        _BPM.BodyInPossession.transform.localRotation = Quaternion.Euler(Euler);

        _BPM.BodyInPossession.PlayerBodyIsAttachedTo = null;

        _BPM.BodyInPossession.netIdentity.RemoveClientAuthority();
        _BPM.BodyInPossession = null;

        //transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);

        //_BPM.BodyInPossession.transform.position = transform.position;
        CLNT_LeaveBodyCallBack();
    }

    [ClientRpc]
    void CLNT_LeaveBodyCallBack()
    {
        _BPM.BodyInPossession.gameObject.tag = "Unoccupied"; 

        _BPM.BodyInPossession.gameObject.SetActive(true);
        _BPM.BodyInPossession.transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        _BPM.BodyInPossession.transform.localRotation = transform.rotation;

        _BPM.BodyInPossession.PlayerBodyIsAttachedTo = null;
        
        _BPM.BodyInPossession.HeadRenderer.enabled = true;
        _BPM.BodyInPossession.ChestRenderer.enabled = true;
        _BPM.BodyInPossession.AbdomenRenderer.enabled = true;
        _BPM.BodyInPossession.RightHandRenderer.enabled = true;
        _BPM.BodyInPossession.LeftHandRenderer.enabled = true;
        _BPM.BodyInPossession.LegRenderer.enabled = true;

        _BPM.BodyInPossession.TurnOnAccentsNVfx();

        _BPM.BodyInPossession._boxCollider.enabled = true;

        _BPM.BodyInPossession = null;
        
        _BPM.HeadMesh.enabled = true;

        //_BPM._Body.gameObject.SetActive(false);
        _BPM._NNBody.TurnOffAccentsNVfx();
        _BPM._NNBody.Head.gameObject.SetActive(false);
        _BPM._NNBody.Abdomen.gameObject.SetActive(false);

        _BPM._MH.IsJumping = false;

        transform.position = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
    }
    
    private void HeadLaunch(bool HeadThrowButton)
    {
        if (!_BPM._CC.isGrounded)
            return;

        if (_BPM.BodyInPossession != null && isLaunching==false)
        {
            CMD_giveBackBodyAuthorityToServer(transform.eulerAngles);
            _BPM._VFX.CMD_VFX_HeadLaunch();
            _BPM._VFX.VFX_DashActive(H_LaunchTime, true);
            _BPM._CMHead.gameObject.SetActive(true);
            _BPM._CMBody.gameObject.SetActive(false);
            isLaunching = true;
            _BPM._MH.overRideMoveVal(_BPM._Cam.transform.forward,LaunchSpeed);
            StartCoroutine(HeadLaunchTimmer());
        }
    }

    IEnumerator HeadLaunchTimmer()
    {
        yield return new WaitForSeconds(H_LaunchTime);
        _BPM._MH.stopOverRideMove();
        isLaunching = false;
        _BPM._MH.IsMovementAllowed = true;
    }

    void Update()
    {

        /*
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(_BPM.BodyInPossession == null && BodiesCloseBy.Count > 0 && BodiesCloseBy[0].tag == "Unoccupied") AttachToBody_RPCCaller();
            else if (_BPM.BodyInPossession != null) DettachBody_RPCCaller();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.A) && _BPM.BodyInPossession != null && BodiesCloseBy.Count > 0)
            {
                _BPM._PV.RPC("SwitchLeftHand", Photon.Pun.RpcTarget.All, BodiesCloseBy[0]);
            }

            if (Input.GetKeyDown(KeyCode.S) && _BPM.BodyInPossession != null && BodiesCloseBy.Count > 0)
            {
                _BPM._PV.RPC("SwitchLegOut", Photon.Pun.RpcTarget.All, BodiesCloseBy[0]);
            }

            if (Input.GetKeyDown(KeyCode.D) && _BPM.BodyInPossession != null && BodiesCloseBy.Count > 0)
            {
                _BPM._PV.RPC("SwitchRightHand", Photon.Pun.RpcTarget.All, BodiesCloseBy[0]);
            }

            if (Input.GetKeyDown(KeyCode.W) && _BPM.BodyInPossession != null && BodiesCloseBy.Count > 0)
            { 
                _BPM._PV.RPC("SwitchChestOut", Photon.Pun.RpcTarget.All, BodiesCloseBy[0]);
            }
            
        }*/
    }

    
    private void SwitchChestOut()
    {
        ChestType Buffer = BodiesCloseBy[0].GetComponent<Body>().chestType;
        _BPM._NNBody.chestType = Buffer;
        _BPM.BodyInPossession.CMD_SwitchChest(BodiesCloseBy[0].gameObject);
        CMD_UdpateNNBChest(Buffer);
    }
    [Command] void CMD_UdpateNNBChest(ChestType CT) => CLNT_UpdateNNBChest(CT);
    [ClientRpc] void CLNT_UpdateNNBChest(ChestType CT) => _BPM._NNBody.chestType = CT; 
    
    private void SwitchLegOut()
    {
        LegType Buffer = BodiesCloseBy[0].GetComponent<Body>().legsType;
        _BPM._NNBody.legsType = Buffer;
        _BPM.BodyInPossession.CMD_SwitchLegs(BodiesCloseBy[0].gameObject);
        CMD_UdpateNNBLegs(Buffer);
    }
    [Command] void CMD_UdpateNNBLegs(LegType CT) => CLNT_UpdateNNBLegs(CT);
    [ClientRpc] void CLNT_UpdateNNBLegs(LegType CT) => _BPM._NNBody.legsType = CT;
    
    private void SwitchRightHand()
    {
        HandType BufferHands = BodiesCloseBy[0].GetComponent<Body>().rightHandType;
        _BPM._NNBody.rightHandType = BufferHands;
        _BPM.BodyInPossession.CMD_SwitchRightHand(BodiesCloseBy[0].gameObject);
        CMD_UdpateNNBRH(BufferHands);
    }
    [Command] void CMD_UdpateNNBRH(HandType CT) => CLNT_UpdateNNBRH(CT);
    [ClientRpc] void CLNT_UpdateNNBRH(HandType CT) => _BPM._NNBody.rightHandType = CT;
    
    private void SwitchLeftHand()
    {
        HandType BufferHands = BodiesCloseBy[0].GetComponent<Body>().leftHandType;
        _BPM._NNBody.leftHandType = BufferHands;
        _BPM.BodyInPossession.CMD_SwitchLeftHand(BodiesCloseBy[0].gameObject);
        CMD_UdpateNNBLH(BufferHands);
    }
    [Command] void CMD_UdpateNNBLH(HandType CT) => CLNT_UpdateNNBLH(CT);
    [ClientRpc] void CLNT_UpdateNNBLH(HandType CT) => _BPM._NNBody.leftHandType = CT;

    /*
     void AttachToBody_RPCCaller()
     {
         BodiesCloseBy[0].GetComponent<RequestOwnerShip>().transferOwnerShip(PhotonNetwork.LocalPlayer.ActorNumber);
         _BPM._PV.RequestOwnership();
         _BPM._PV.RPC("AttachToBody_RPC", RpcTarget.All, BodiesCloseBy[0].GetComponent<Body>()._PV.ViewID);
         BodiesCloseBy.Remove(BodiesCloseBy[0]);
     }


     [PunRPC]
     void AttachToBody_RPC(int ViewID)
     {
         Debug.Log("attach");

         Body BodyToAttachTo = MainGameManager.Inst.ReturnBody(ViewID);

         transform.position = BodyToAttachTo.Head.transform.position;
         BodyToAttachTo.transform.parent = transform;
         BodyToAttachTo.gameObject.tag = "Occupied";
         BodyToAttachTo.Head.SetActive(true);

         _BPM.HeadMesh.enabled = false;
         _BPM.BodyInPossession = BodyToAttachTo;

         BodyToAttachTo.transform.localRotation = Quaternion.Euler(Vector3.zero);
         BodyToAttachTo.transform.localPosition = new Vector3(0, -0.5f, 0);
     }
      */

    void AttachToBody(Body BodyToAttachTo)
    {
        BodiesCloseBy.Remove(BodiesCloseBy[0]);

        transform.position = BodyToAttachTo.Head.transform.position;
        BodyToAttachTo.transform.parent = transform;
        BodyToAttachTo.gameObject.tag = "Occupied";
        BodyToAttachTo.Head.SetActive(true);

        _BPM.HeadMesh.enabled = false;
        _BPM.BodyInPossession = BodyToAttachTo;
        _BPM._CMHead.gameObject.SetActive(false);
        _BPM._CMBody.gameObject.SetActive(true);

        BodyToAttachTo.transform.localRotation = Quaternion.Euler(Vector3.zero);
        BodyToAttachTo.transform.localPosition = new Vector3(0, -0.5f, 0);
    }

    /*
    void DettachBody_RPCCaller()
    {
        _BPM._PV.RPC("DettachBody_RPC", RpcTarget.All);

        _BPM._CMHead.gameObject.SetActive(true);
        _BPM._CMBody.gameObject.SetActive(false);
    }

    [PunRPC]
    void DettachBody_RPC()
    {
        _BPM.BodyInPossession.transform.parent = null;
        _BPM.BodyInPossession.tag = "Unoccupied";
        _BPM.BodyInPossession.Head.SetActive(false);

        _BPM.HeadMesh.enabled = true;

        _BPM.BodyInPossession = null;
    }
    */

    void DettachBody()
    {
        _BPM.BodyInPossession.transform.parent = null;
        _BPM.BodyInPossession.tag = "Unoccupied";
        _BPM.BodyInPossession.Head.SetActive(false);
        _BPM._CMHead.gameObject.SetActive(true);
        _BPM._CMBody.gameObject.SetActive(false);
        _BPM.HeadMesh.enabled = true;

        _BPM.BodyInPossession = null;
    }
}
