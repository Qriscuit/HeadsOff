using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;
using Mirror;
public class PG_BaseLaunchPoint : NetworkBehaviour
{
    [SerializeField] float Damage;
    [SerializeField] float PunchSpeed;
    [SerializeField] float radiusOfInfluence;
    [SerializeField] Rigidbody PunchRB;
    [SerializeField] float RotationAngle; // the less it is more the curve will we sharper 
    [SerializeField] bool isPunchAllowed;
    [SerializeField] float reloadTime;
    // [SerializeField] float 

    [Header("Distance")]
    [SerializeField] float DistanceMax;
    [SerializeField] float DistanceCurrent;
    Vector3 oldPos;

    [SerializeField] Collider[] collidedwith;
    GameObject _collidedplayer;
    [SerializeField] LayerMask layerMask;

    bool isFollowPlayerAllowed;

    public BasePlayerManager _BPM;
    public Team myTeam;

    private void Awake()
    {
        isFollowPlayerAllowed = false;
        isPunchAllowed = true;

    }
    private void Start()
    {
        oldPos = transform.position;
    }

    public void LaunchPunch(Vector3 DirectionToGo)
    {
        if (!isPunchAllowed)
            return;

        PunchRB.velocity = DirectionToGo * PunchSpeed;
        transform.forward = DirectionToGo;

        isPunchAllowed = false;
        StartCoroutine(reload());
    }

    IEnumerator reload()
    {
        yield return new WaitForSeconds(reloadTime);
        isPunchAllowed = false;
    }

    private void Update()
    {
        if (!isServer)
            return;

        CalDistance();
        if (!isFollowPlayerAllowed)
            SearchPlayer();

        if (isFollowPlayerAllowed)
            followplayer();
    }

    void followplayer()
    {
        //NEED TO TEST HUMMING
        //Vector2 direction = (Vector2)TargetJoint2D.position - PunchRB.position;
        //direction.Normalize();
        //float rotateAmount = Vector3.Cross(direction, transform.up).z;
        //PunchRB.angularVelocity = -rotateAmount * rotateSpeed;
        //PunchRB.velocity = transform.up * SpeedUpdate;


        //this is working for opposite vector
        //PunchRB.AddForce(-_collidedplayer.transform.position);
        
        //this is working sharp
        PunchRB.MovePosition(Vector3.Lerp(transform.position,_collidedplayer.transform.position,0.1f));
        Debug.Log("following player");
    }

    [Server]
    void SearchPlayer()
    {
        Debug.Log("searching for player");
        collidedwith=Physics.OverlapSphere(transform.position, radiusOfInfluence, layerMask);
       
        if (collidedwith.Length>0)
        {
            if (collidedwith[0].gameObject.layer != 9)
                return;
            
            
            DamageManager DM = collidedwith[0].gameObject.GetComponent<DamageManager>();
            if (myTeam == DM._BPM._Team)
                if (!DM._BPM._WM.FriendlyFire)
                    return;
                    
            Debug.Log(collidedwith);
            isFollowPlayerAllowed = true;
            _collidedplayer = collidedwith[0].gameObject;
        }
        
    }

    void CalDistance()
    {
        Vector3 distanceVector = transform.position - oldPos;
        float distanceThisFrame = distanceVector.magnitude;
        DistanceCurrent += distanceThisFrame;
        oldPos = transform.position;

        if(DistanceCurrent>=DistanceMax)
        {
            NetworkServer.Destroy(this.gameObject);
        }

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radiusOfInfluence);
        Debug.Log("drawing influence");
    }

    [Server]
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer==9)
        {
            DamageManager DM = collision.gameObject.GetComponent<DamageManager>();
            if (myTeam == DM._BPM._Team)
                if (!DM._BPM._WM.FriendlyFire)
                    return;

            DM.SingleHit(Damage, _BPM);
            _BPM._UIM.MakeDamageCrossHairAppear();
            NetworkServer.Destroy(this.gameObject);
        }
        else
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }
}
