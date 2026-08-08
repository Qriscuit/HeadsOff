using System.Collections;
using UnityEngine;
using Mirror;
using HeadsOffGlobals;

public class Mine : NetworkBehaviour
{
    public SphereCollider ProximitySensor;
    public SphereCollider ExplosionCircle;

    public float Damage;
    public ParticleSystem Explosion;

    public float LaunchStrengthMultiplier;
    public float ProximitySize;
    public float ExplosionSize;
    public float ExplosionSpeedMultiplier;

    public bool _IsNadeArmed = false;
    public bool IsNadeArmed = false;
    public bool IsExploding = false;

    public Material BLUE;
    public Material RED;
    public Material grey;

    public Rigidbody RB;

    public MeshRenderer Blinker;

    public BasePlayerManager _BPM;
    public Team _Team;

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();

        Blinker.material = grey;

        if (isServer) ProtocolNightfallCall = StartCoroutine(ProtocolNightfall());
    }

    Coroutine BlinkingRoutine;

    IEnumerator BlinkingBlue()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            Blinker.material =  BLUE;
            yield return new WaitForSeconds(0.1f);
            Blinker.material = grey;
        }
    }

    IEnumerator BlinkingRed()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            Blinker.material = RED;
            yield return new WaitForSeconds(0.1f);
            Blinker.material = grey;
        }
    }

    public void LaunchMineFromHand(Vector3 _Dir)
    {
        RB.AddForce(_Dir * LaunchStrengthMultiplier);
    }

    //[Server]
    public void OnTriggerEnter(Collider other)
    {
        if (isClient) return;
        if (!_IsNadeArmed) return;

        if(other.gameObject.layer == 9) //LAYER HARDCODE: 9 - DamageCollider
        {
            //if (MainGameManager.Inst.IsOtherOfMyTeam(other.gameObject.transform.parent.gameObject)) return;

            DamageManager _DM = other.GetComponent<DamageManager>();
            if (_Team == _DM._BPM._Team)
                if (!_DM._BPM._WM.FriendlyFire)
                    return;

            CLNT_Explode();
            Explode();
            StopCoroutine(BlinkingRoutine);
            Blinker.material = RED;
        }
    }

    //[Server]
    private void OnCollisionEnter(Collision collision)
    {
        if (isClient) return;
        if (collision.gameObject.layer == 7) //LAYER HARDCODE: 7 - WALLS
        {
            CLNT_ActivateMine(collision.GetContact(0).normal, _Team);
            ActivateMine(collision.GetContact(0).normal);
        }
    }

    [ClientRpc]
    public void CLNT_ActivateMine(Vector3 Normal, Team _Team)
    {
        RB.velocity = Vector3.zero;
        RB.useGravity = false;
        RB.isKinematic = true;
        ProximitySensor.isTrigger = true;

        transform.up = Normal;

        ProximitySensor.enabled = true;
        IsNadeArmed = true;
        _IsNadeArmed = true;
        Debug.Log("Is local bpm Active ? " + (Room.LocalBPM != null));
        BlinkingRoutine = (Room.LocalBPM == null) ? StartCoroutine(BlinkingRed()) : (Room.LocalBPM._Team == _Team) ? StartCoroutine(BlinkingBlue()) : StartCoroutine(BlinkingRed());
    }
    public void ActivateMine(Vector3 Normal)
    {
        RB.velocity = Vector3.zero;
        RB.useGravity = false;
        RB.isKinematic = true;
        ProximitySensor.isTrigger = true;

        transform.up = Normal;

        ProximitySensor.enabled = true;
        IsNadeArmed = true;
        _IsNadeArmed = true;
        BlinkingRoutine = StartCoroutine(BlinkingRed());
    }

    public float explsnCalc = 0;
    private void Update()
    {
        if (IsNadeArmed)
        {
            explsnCalc += Time.deltaTime * ExplosionSpeedMultiplier;

            ProximitySensor.radius = Mathf.Lerp(ProximitySensor.radius, ProximitySize, explsnCalc);

            if (ProximitySensor.radius >= ProximitySize)
            {
                //Debug.Log("Armed and ready to fuck things up");

                IsNadeArmed = false;
                explsnCalc = 0;
            }
        }

        if (IsExploding)
        {
            explsnCalc += Time.deltaTime * ExplosionSpeedMultiplier;

            ExplosionCircle.radius = Mathf.Lerp(ExplosionCircle.radius, ExplosionSize, explsnCalc);

            if (ExplosionCircle.radius >= ExplosionSize)
            {
                IsExploding = false;
                explsnCalc = 0;
            }
        }
    }

    [ClientRpc]
    private void CLNT_Explode()
    {
        IsExploding = true;
        Debug.Log("About to go in coroutine for cmd deletion");
        Explosion.gameObject.SetActive(true);

        ProximitySensor.enabled = false;
        //ExplosionCircle.enabled = true;
        this.GetComponent<MeshRenderer>().enabled = false;
        //StartCoroutine(DestroyMine());
    }
    private void Explode()
    {
        IsExploding = true;
        Explosion.gameObject.SetActive(true);

        ProximitySensor.enabled = false;
        ExplosionCircle.enabled = true;
        this.GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(DestroyMine());
    }

    IEnumerator DestroyMine()
    {
        Debug.Log("In the Coutoutine of death");
        yield return new WaitForSeconds(1f);
        Delete();
    }

    void Delete()
    {
        Debug.Log("Deleting mine");
        NetworkServer.Destroy(gameObject);
    }

    Coroutine ProtocolNightfallCall;
    IEnumerator ProtocolNightfall()
    {
        yield return new WaitForSeconds(15);
        Explode();
        CLNT_Explode();
    }

    private NM_GC room;
    private NM_GC Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NM_GC;
        }
    }
}
