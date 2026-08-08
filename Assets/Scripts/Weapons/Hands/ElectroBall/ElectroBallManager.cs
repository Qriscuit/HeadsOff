using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;
using HeadsOffGlobals;

public class ElectroBallManager : NetworkBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] float radiusOfInfluence;
    public Rigidbody BallRB;
    SphereCollider _SC;
    public GameObject MR_MainBall;
    public GameObject MR_InfluenceMat;
    //To Rotate Ball
    public Vector3 RotationAngle;
    public float BallRotationSensitivity;
    public float BallSpeed;
    public float StunTime;//Time after which electroball effect gets deleted
    public float SingleDamage = 50;
    public float DamageOverTimeDamage = 5;
    public float DamageOverTimeTime = 5;
    public float WeaponLockTime=3;
    public float TickRate = 0.5f;

    public Collider[] collidedwith;

    public BasePlayerManager _BPM;
    public Team myTeam;

    private void Awake()
    {
        radiusOfInfluence = 0f;
        if(BallRB==null)
        {
            BallRB = GetComponent<Rigidbody>();
        }
        _SC = GetComponent<SphereCollider>();
        DestroyBallFromTimeOutCoroutine = StartCoroutine(DestroyBallFromTimeOut());
    }

    Coroutine DestroyBallFromTimeOutCoroutine;
    IEnumerator DestroyBallFromTimeOut()
    {
        yield return new WaitForSeconds(7);
        //PhotonNetwork.Destroy(_MyPV);
        Destroy(this.gameObject);
    }




    public void LaunchBall(Vector3 DirectionToGo)
    {
        BallBounceFromPointTwo();
        BallRB.velocity = DirectionToGo * BallSpeed;
        RotationAngle = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    // Scalling efecting while instantiating ball
    void BallBounceFromPointTwo()
    {
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(transform.DOScale(Vector3.one * 7.3f, 0.5f).SetEase(Ease.InCirc));
        mySequence.Append(transform.DOScale(Vector3.one * 6.8f, 0.2f).SetEase(Ease.InOutCirc));
        mySequence.Append(transform.DOScale(Vector3.one * 7f, 0.2f).SetEase(Ease.InCirc));
        
        mySequence.Play();
    }

    private void Update()
    {
        transform.Rotate(RotationAngle * Time.deltaTime * BallRotationSensitivity);
        freezeWapon();
    }


    void freezeWapon()
    {
        collidedwith = Physics.OverlapSphere(transform.position, radiusOfInfluence, layerMask);
        if (collidedwith.Length > 0)
        {
            DamageManager DM =collidedwith[0].gameObject.GetComponent<DamageManager>();

            if (myTeam == DM._BPM._Team)
                if (!DM._BPM._WM.FriendlyFire)
                    return;

            //DM._BPM._WM.TurnOffWeapons(StunTime);
            DM.StunPlayer(StunTime, 1);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radiusOfInfluence);
    }

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            Collided();
            CLNT_Collided();
            //some code to check if ball collided with player or not and then calculating score or freezing then
        }
        if (other.gameObject.layer == 9)
        {
            DamageManager DM = other.gameObject.GetComponent<DamageManager>();

            if (myTeam == DM._BPM._Team)
                if (!DM._BPM._WM.FriendlyFire)
                    return;

            DM.SingleHit(SingleDamage, _BPM);
            _BPM._UIM.MakeDamageCrossHairAppear();
        }
    }


    void Collided()
    {
        _SC.enabled=false;
        StartCoroutine(Destroythisgameobject());
        radiusOfInfluence = 14f;
        //stopping the ball from moving further
        BallRB.velocity = new Vector3(0, 0, 0);
    }

    [ClientRpc]
    void CLNT_Collided()
    {
        _SC.enabled = false;
        StartCoroutine(Destroythisgameobject());
        radiusOfInfluence = 14f;
        //stopping the ball from moving further
        BallRB.velocity = new Vector3(0, 0, 0);
    }

    //this function gets called whenever player collide with the Influence Zone
    public void CollisionWithInfluenceZone()
    {
        //some code
    }

    IEnumerator Destroythisgameobject()
    {
        MR_MainBall.SetActive(false);
        MR_InfluenceMat.SetActive(true);
        yield return new WaitForSeconds(StunTime);
        //turnng off meshrenderer
        MR_InfluenceMat.SetActive(false);
        Destroy(this.gameObject);
    }
}
