using System.Collections;
using UnityEngine;
using DG.Tweening;
using Mirror;
using HeadsOffGlobals;

public class FeviTopChild : NetworkBehaviour
{

    DamageManager _DM;

    public float SlowedSpeed;//value to slow player by
    public float SlowedSpeedDelay;//value to make player slow for certain time
    public float ScaleValue;//scale value of the glue's collider
    public float TimeToScale;
    public float FTPCDestroyTime;

    [Header("burning")]
    [HideInInspector]public Team myTeam;
    public float BurnDamagePsec = 1;
    public float BurnTimeToAdd = 1;
    public float TickRate = 0.5f;
    [SyncVar]public bool isburning;
    public float totalburntime;
    [SerializeField] ParticleSystem _feviburn1;
    [SerializeField] ParticleSystem _feviburn2;
    [SerializeField] ParticleSystem _feviburn3;
    [SerializeField] ParticleSystem _feviburn4;

    [ClientRpc]
    public void CLNT_startBurning()
    {
        if (isburning)
            return;

        Debug.Log("burn test");
        isburning = true;
        _feviburn1.Play();
        _feviburn2.Play();
        _feviburn3.Play();
        _feviburn4.Play();
        StopCoroutine(DestroyFTPC());
        StartCoroutine(destroyburn());
        //spawnFX
    }

    IEnumerator destroyburn()
    {
        yield return new WaitForSeconds(totalburntime);
        NetworkServer.Destroy(this.gameObject);
    }


    [Server]
    private void OnTriggerEnter(Collider other)
    {



        if (other.gameObject.layer != 9)
            return;

        Debug.Log("player");
        _DM = other.gameObject.GetComponent<DamageManager>();

        if (myTeam == _DM._BPM._Team)
            if (!_DM._BPM._WM.FriendlyFire)
                return;


        if (isburning)
        {
            _DM.DamageOverTime(BurnDamagePsec, TickRate, BurnTimeToAdd);
            return;
        }

        _DM._BPM._VFX.FBCall_StickyGlueStart(2);
        _DM._BPM._MH.SlowedSpeed = SlowedSpeed;
            _DM.isCollidingWithOtherGlue = true;
        
    }

    Coroutine RemovePlayerSlowSpeed;

    private void Start()
    {
        StartCoroutine(DestroyFTPC());
        isburning = false;
    }

    [Server]
    private void OnTriggerExit(Collider other)
    {
        if (isClient)
            return;

        if (other.gameObject.layer != 9)
            return;

        Debug.Log("player");
        _DM = other.gameObject.GetComponent<DamageManager>();

        if (myTeam == _DM._BPM._Team)
            if (!_DM._BPM._WM.FriendlyFire)
                return;


        if (isburning)
        {
            _DM.DamageOverTime(BurnDamagePsec, TickRate, BurnTimeToAdd);
            return;
        }


        _DM.RemoveSlowPlayer(SlowedSpeedDelay,SlowedSpeed);
        _DM.isCollidingWithOtherGlue = false;
    }

    IEnumerator DestroyFTPC()
    {
        yield return new WaitForSeconds(FTPCDestroyTime);
        if(_DM!=null)
        {
            _DM.isCollidingWithOtherGlue = false;
        }
        NetworkServer.Destroy(this.gameObject);
    }

    public void startscallingRPC()
    {
        startscalling();
    }

    public void startscalling()
    {
        transform.DOScaleX(ScaleValue, TimeToScale);
        transform.DOScaleZ(ScaleValue, TimeToScale);
    }

}
