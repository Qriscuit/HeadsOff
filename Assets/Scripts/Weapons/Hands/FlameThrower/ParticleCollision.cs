using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;

public class ParticleCollision : NetworkBehaviour
{
    public Team myTeam;
    public ParticleSystem _PS;
    public List<ParticleCollisionEvent> collisionEvents;
    public bool _PSplayState;
    public Transform MainCam;
    public FlameThrower _FlameThrower;

    public float BurnDamagePsec=1;
    public float BurnTimeToAdd=1;
    public float TickRate=0.5f;


    private void Awake()
    {
        _PS = GetComponent<ParticleSystem>();
    }

    public void TurnOnFlame()
    {
        _PS.Play();
        _PSplayState = true;
    }

    [Command]
    public void CMD_TurnOffFlame()
    {
        CLNT_TurnOffFlame();
        _PS.Stop();
        _PSplayState = false;
    }
    [ClientRpc]
    public void CLNT_TurnOffFlame()
    {
        _PS.Stop();
        _PSplayState = false;
    }

    [Server]
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Fire check fire check and other objects layer is " + other.layer);

        if (other.gameObject.layer == 10)//for fevi burn
        {
            other.gameObject.GetComponent<FeviTopChild>().CLNT_startBurning();
        }

        if (other.layer != 9)//not sure if this is the layer
            return;

        Debug.Log("I've hit a player");
        DamageManager DM = other.gameObject.GetComponent<DamageManager>();

        if (myTeam == DM._BPM._Team)
            if (!DM._BPM._WM.FriendlyFire)
                return;

        DM.DamageOverTime(BurnDamagePsec, TickRate, BurnTimeToAdd);

        _FlameThrower._WM._BPM._UIM.MakeDamageCrossHairAppear();

            /* for preciese collision check not working properly
            if(_PS != null)
            {
                Debug.Log("_ps is not null");
                int numCollisionEvents = _PS.GetCollisionEvents(other, collisionEvents);
                int i = 0;
                while (i < numCollisionEvents)
                {
                    Debug.Log("Damageing them now");
                    _damageManager.DamageOverTime(BurnDamagePsec, TickRate, BurnTimeToAdd);
                    i++;
                }
            }
            */
        
    }

    private void Update()
    {
        if(_PSplayState)
        {
            if (isServer)
                return;
            transform.localEulerAngles = new Vector3(_FlameThrower._WM._BPM._Cam.transform.localEulerAngles.x,0,0);

            CMD_syncRotation(_FlameThrower._WM._BPM._Cam.transform.localEulerAngles.x);
        }
    }
    [Command]
    void CMD_syncRotation(float x)
    {
        //Debug.Log("test rot : "+x);
        transform.localEulerAngles = new Vector3(x, 0, 0);
        CLNT_syncRotation(x);

    }
    [ClientRpc]
    void CLNT_syncRotation(float x)
    {
        if (isLocalPlayer)
            return;
        transform.localEulerAngles = new Vector3(x, 0, 0);
    }

}

/*
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    public ParticleSystem _PS;
    public List<ParticleCollisionEvent> collisionEvents;
    public bool _PSplayState;
    public Transform MainCam;
    //public FlameThrower _FlameThrower;

    public float BurnDamagePsec;
    public float BurnTimeToAdd;

    public PhotonView _MyPV;

    public class PlayerBurnData
    {
        public GameObject Player;
        public float BurnTime;
        public float TimeBurnt;
        public float LastFDamage;
        public float CurrentFDamage;
    };

    public List<PlayerBurnData> _PlayerBurnDatas;

    private void Awake()
    {
        _PS = GetComponent<ParticleSystem>();
    }
    void Start()
    {
        transform.Rotate(new Vector3(0, 0, 0));
        _PlayerBurnDatas = new List<PlayerBurnData>();

        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == 8)//not sure if this is the layer
        {
            int numCollisionEvents = _PS.GetCollisionEvents(other, collisionEvents);
            int i = 0;

            // int index = _PlayerBurnDatas.FindIndex(x => x.Player == other);//incomplete code...need to add a proper value which identifies the player

            while (i < numCollisionEvents)
            {
                Debug.Log(other + "Recieved Damage");
                //call the damage funtion in the player...damageoverTime

                /*
                if (index != -1)//that means player is in the list
                {
                    _PlayerBurnDatas[index].BurnTime += BurnTimeToAdd;
                }
                else
                {
                    PlayerBurnData newPlayer = new PlayerBurnData();
                    newPlayer.Player = other;
                    newPlayer.BurnTime += BurnTimeToAdd;
                }

                i++; * /
            }
        }
    }

    private void Update()
    {
        if (_PSplayState)
        {
            transform.localEulerAngles = new Vector3(MainCam.localEulerAngles.x, 0, 0);
            Debug.Log(transform.localRotation.y);
        }
        /*
        foreach(PlayerBurnData b in _PlayerBurnDatas)
        {
            if(b.BurnTime>b.TimeBurnt)
            {
                b.TimeBurnt += Time.deltaTime;
                b.CurrentFDamage = b.TimeBurnt * BurnDamagePsec;
                Debug.Log(b.CurrentFDamage - b.LastFDamage);//change this with some damage function
                b.LastFDamage = b.CurrentFDamage;
            }
            else
            {
                b.BurnTime = 0f;
                b.TimeBurnt = 0f;
                b.CurrentFDamage = 0f;
                b.LastFDamage = 0f;
            }
        }
        * /
    }
}
*/