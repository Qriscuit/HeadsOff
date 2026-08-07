using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using DG.Tweening;
using HeadsOffGlobals;
public class PumpUpManager : NetworkBehaviour
{
    [HideInInspector] public PumpUp _pumpup;
    [SerializeField] float _PumpUpDamage;
    DamageManager _DM;
    public Team MyTeam;
    float ScaleTime;

    public void StartScalling(float ScaleValue, float _ScaleTime, float PumpUpDamage, float AfterScaleFinishDestroyTime)
    {
        ScaleTime = _ScaleTime;
        _PumpUpDamage = PumpUpDamage;
        transform.DOScale(ScaleValue, ScaleTime).OnComplete(()=>DestroyCaller(AfterScaleFinishDestroyTime));
    }

    void DestroyCaller(float DestroyTime)
    {
        StartCoroutine(destroyThis(DestroyTime));
    }
    IEnumerator destroyThis(float Value)
    {
        yield return new WaitForSeconds(Value);
        NetworkServer.Destroy(this.gameObject);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (isClient) return;

        if (other.gameObject.layer != 9) return;

        _DM = other.GetComponent<DamageManager>();


        if (_DM._BPM._Team == MyTeam)
            if(!_pumpup.FriendlyFire)
                return;

        _DM.SingleHit(_PumpUpDamage, _pumpup._WM._BPM);

        //Send Hit Info back to main player so that tehy can flash damage image 
        _pumpup._WM._BPM._UIM.MakeDamageCrossHairAppear();

        //Also place for damage numbers to appear when player 

    }
}
