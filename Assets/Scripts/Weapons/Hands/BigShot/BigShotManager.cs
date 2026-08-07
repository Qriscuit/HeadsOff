using Mirror;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using HeadsOffGlobals;
public class BigShotManager : NetworkBehaviour
{
    public float BallSpeed;
    public float BallDamage=20f;
    public float BallRotationSensitivity = 100f;
    public float ImpactTime;
    public float DamageRadius;

    Vector3 RotationAngle;
    public Vector3 ExplosionPoint;

    [Header("Class References")]
    public ExplosionManager _EM;
    public BasePlayerManager _BPM;  

    [Space]
    public Rigidbody BallRB;
    public MeshRenderer BallMeshRenderer;
    public SphereCollider BallCollider;

    public Team myTeam;

    private void Awake()
    {
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
        //CMD_LaunchBall(DirectionToGo);

        BallBounceFromPointTwo();
        BallRB.velocity = DirectionToGo * BallSpeed;
        RotationAngle = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    private void Update()
    {
        transform.Rotate(RotationAngle * Time.deltaTime * BallRotationSensitivity);
    }

    void BallBounceFromPointTwo()
    {
        Sequence mySequence = DOTween.Sequence();

        mySequence.Append(transform.DOScale(Vector3.one * 2.3f, 0.2f).SetEase(Ease.InCirc));
        mySequence.Append(transform.DOScale(Vector3.one * 1.8f, 0.1f).SetEase(Ease.InOutCirc));
        mySequence.Append(transform.DOScale(Vector3.one * 2f, 0.1f).SetEase(Ease.InCirc));

        mySequence.Play();
    }

    [Server]
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 7 || other.gameObject.layer == 9 || other.gameObject.layer == 6 || other.gameObject.layer == 8)
        {
            BallMeshRenderer.enabled = false;
            BallCollider.enabled = false;

            BallRB.velocity = new Vector3(0, 0, 0);

            Collided();
            CLNT_Collided();
            ExplosionPoint = other.GetContact(0).normal;
        }


    }

    public void StartExplosion()
    {
        _EM.ExplosionStarted = true;
        _EM._SphereCollider.enabled = true;
        _EM.transform.DOScale(DamageRadius, ImpactTime);

        StopCoroutine(DestroyBallFromTimeOutCoroutine);
        StartCoroutine(DestroyBall());
    }

    void Collided()
    {
        _EM.StartExplosion();
    }

    [ClientRpc]
    void CLNT_Collided()
    {
        //BallMeshRenderer.enabled = false;
        //BallCollider.enabled = false;

        _EM.StartExplosion();
    }

    public IEnumerator DestroyBall()
    {
        yield return new WaitForSeconds(ImpactTime);
        CMD_Delete();
        //Destroy(transform.parent.gameObject);
    }

    public void CMD_Delete()
    {
        NetworkServer.Destroy(gameObject);
    }
}