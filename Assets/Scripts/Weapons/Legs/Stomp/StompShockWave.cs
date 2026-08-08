using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HeadsOffGlobals;

public class StompShockWave : NetworkBehaviour
{
    public StompLegs _StompLegs;

    public List<Transform> _childTranform;
    [SerializeField] float ShockWaveTime;

    DamageManager PlayerHit;

    public Team _team;

    private void Start()
    {
        StartCoroutine(DestroyShockWave());
    }


    IEnumerator DestroyShockWave()
    {
        yield return new WaitForSeconds(ShockWaveTime);
        NetworkServer.Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isClient) return;
        if(other.gameObject.layer == 9)
        {
            PlayerHit = other.GetComponent<DamageManager>();
            
            if (!_StompLegs.FriendlyFire)
                if (_team == PlayerHit._BPM._Team) return;

            PlayerHit.SingleHit(20,_StompLegs._WM._BPM);
            _StompLegs._WM._BPM._UIM.MakeDamageCrossHairAppear();
        }
    }

    public void ServerStartScallingRPCCaller(float Endvalue)
    {
        StartScalling(Endvalue);
    }

    void StartScalling(float Endvalue)
    {

        transform.DOScaleX(Endvalue, ShockWaveTime);
        transform.DOScaleZ(Endvalue, ShockWaveTime);
    }

    private void Update()
    {
        foreach(Transform child in transform)
        {
            Vector3 scale = new Vector3(transform.localScale.x, transform.localScale.x, transform.localScale.z);
            child.localScale = scale;

        }
    }
}
