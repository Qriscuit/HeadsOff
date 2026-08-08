using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening;

public class ShieldBall : NetworkBehaviour
{
    public Rigidbody _RB;
    public SphereCollider _SphereCollider;
    public MeshRenderer BallRenderer;
    
    public float LaunchSpeedMultiplier;

    public GameObject EntireShield;

    Vector3 dir;
    
    private void Awake()
    {
        _RB = GetComponent<Rigidbody>();
        _SphereCollider = GetComponent<SphereCollider>();
    }

    IEnumerator Destroy()
    {
        Debug.Log("about destory in 10 secs");
        yield return new WaitForSeconds(10);
        NetworkServer.Destroy(gameObject);
    }

    public void LaunchBall(Vector3 _Dir)
    {
        dir = _Dir;    
        transform.forward = dir;
        _RB.velocity = _Dir * LaunchSpeedMultiplier * 100;
    }

    //[Server]
    private void OnCollisionEnter(Collision collision)
    {
        if (isClient) return;
        if (collision.gameObject.layer != 7) return;
        if (collision.gameObject.tag == "Shield") return;

        Debug.Log("Hit something " + collision.gameObject.name +" "+ collision.gameObject.layer);

        BallRenderer.enabled = false;
        _RB.isKinematic = true;

        Debug.Log(collision.GetContact(0).normal);

        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
        //transform.up = collision.GetContact(0).normal;

        EntireShield.SetActive(true);
        EntireShield.transform.DOScale(Vector3.one, 0.5f);

        StartCoroutine(Destroy());

        CLNT_SpawnShield(transform.position);
    }

    [ClientRpc]
    void CLNT_SpawnShield(Vector3 Position)
    {
        transform.position = Position;
        _SphereCollider.enabled = false;
        
        BallRenderer.enabled = false;
        _RB.isKinematic = true;

        EntireShield.SetActive(true);
        EntireShield.transform.DOScale(Vector3.one, 0.5f);
    }
}
