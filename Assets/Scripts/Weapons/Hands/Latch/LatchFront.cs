using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LatchFront : MonoBehaviour
{
    Rigidbody RB;

    SphereCollider _collider;

    public BasePlayerManager _BPM;
    public LayerMask Mask;

    public float speed = 1f;

    [HideInInspector] public Vector3 SurfaceNormal;

    public float TimeToGoBeforeReturning;

    Coroutine DidNotFindWall;
    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
        DidNotFindWall = StartCoroutine(WentAsFarAsPossible());
        _BPM._MH.IsMovementAllowed = false;
        StartCoroutine(AboutToDestroy());
        Destroy(this.gameObject, 10f);
    }

    IEnumerator AboutToDestroy()
    {
        yield return new WaitForSeconds(9.5f);
        _BPM._MH.IsMovementAllowed = true;
      // .. _BPM._LC.gameObject.SetActive(false);
        _BPM._Cam.enabled = true;
     //  _BPM._LC.CamActivated = false;
    }

    IEnumerator WentAsFarAsPossible()
    {
        yield return new WaitForSeconds(TimeToGoBeforeReturning);

        ComeBack();
    }

    void ComeBack()
    {
        RB.velocity = Vector3.zero;

        transform.DOMove(_BPM.transform.position, 0.5f).OnComplete(() => 
        {
            Destroy(this.gameObject);
        });
    }

    Vector3 _Dir;
    public void Launch(Vector3 Dir)
    {
        _Dir = Dir;

        RB.velocity = _Dir * speed; 
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 10)
        {
            RB.velocity = Vector3.zero;
            RB.isKinematic = true;
            _collider.isTrigger = true;

            SurfaceNormal = other.GetContact(0).normal;

            transform.eulerAngles = SurfaceNormal;
            
            ReelIn();

            StopCoroutine(DidNotFindWall);
        }
    }

    private void OnDestroy()
    {
        
    }

    private void ReelIn()
    {
        _BPM.transform.DOMove(transform.position, 0.7f).OnComplete(() => 
        {
            _BPM.transform.forward = SurfaceNormal;
            _BPM._MH.IsMovementAllowed = false;
           // _BPM._LC.gameObject.SetActive(true);
           // _BPM._LC.transform.forward = SurfaceNormal;

            //_BPM._CH.enabled = false;
            _BPM._Cam.enabled = false;

            //_BPM._MH.IsMovementAllowed = false;
            //_BPM._CH._MC.enabled = false;
            //_BPM._LC._Cam.enabled = true;
         //   _BPM._LC.CamActivated = true;
        } );
    }
}
