using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Mirror;
using HeadsOffGlobals;

public class FeviTopParent : NetworkBehaviour
{
    public WeaponManager _WM;
    public Team myTeam;
    public GameObject FeviSplashP;
    public FeviTopChild FeviTopChildPrefab;
    public float TimeToSpawnGlue;
    public Rigidbody _FTRrigidbody;
    public ParticleSystem FeviPuddle;
    public float FTPDestroyTime;

    public bool SpawnGlueAllowed;

    private void Start()
    {
        SpawnGlueAllowed = true;
        _FTRrigidbody = GetComponent<Rigidbody>();
        StartCoroutine(FeviTopDestroy());
    }

    [Server]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            _WM.CLNT_setSpawnFEVIref();
            if (SpawnGlueAllowed)
            {
                Debug.Log("Fevitop hit the ground on server");
                CLNT_TurnOnGlueVFX();
                StartCoroutine(SpawnGlue());
            }
        }
    }

    [ClientRpc]
    public void CLNT_TurnOnGlueVFX()
    {
        Debug.Log("Fevitop client fevipuddle vfx play");
        FeviPuddle.Play();
    }

    public void Launch(Vector3 Pos, float FTmoveSpeed, WeaponManager WM)
    {
        _FTRrigidbody.velocity = (Vector3.down +Pos) * FTmoveSpeed ;

        _WM = WM;
    }

    IEnumerator SpawnGlue()
    {
        SpawnGlueAllowed = false;
        yield return new WaitForSeconds(TimeToSpawnGlue);
        SpawnGlueAllowed = true;
        //Debug.Log("spawning glue");
        GameObject _FTPC = Instantiate(FeviTopChildPrefab.gameObject, this.transform.position,Quaternion.identity);
        NetworkServer.Spawn(_FTPC);
        _FTPC.GetComponent<FeviTopChild>().startscallingRPC();
        _FTPC.GetComponent<FeviTopChild>().myTeam = myTeam;
    }

    IEnumerator FeviTopDestroy()
    {
        yield return new WaitForSeconds(FTPDestroyTime);
        CMD_SpawnFeviVFX(this.transform.position);
        NetworkServer.Destroy(this.gameObject);
    }

    public void CMD_SpawnFeviVFX(Vector3 positionToSpawn)
    {
        if (isClient)
            return;

        GameObject FeviVFX = Instantiate(FeviSplashP, positionToSpawn, Quaternion.identity);
        NetworkServer.Spawn(FeviVFX);
    }
}

