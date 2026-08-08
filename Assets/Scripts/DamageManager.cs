using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class DamageManager : MonoBehaviour
{
    public BasePlayerManager _BPM;

    public BoxCollider HeadCollider;
    public BoxCollider BodyCollider;

    public Transform DamageNumbersParent;
    public MMFeedback DamagePopUp;

    public BasePlayerManager LastDamageBy;

    private void Awake()
    {
        _BPM = GetComponentInParent<BasePlayerManager>();
    }

    public void ConnectToBody(bool Connected)
    {
        //if the parameter comes False it means that the player just detached himself from the body

        if (Connected)
        {
            BodyCollider.enabled = true;
            HeadCollider.enabled = false;
        }
        else
        {
            BodyCollider.enabled = false;
            HeadCollider.enabled = true;
        }
    }

    public void OnEnable()
    {
        _BPM._bodyAttached += ConnectToBody;
    }

    public void OnDisable()
    {
        _BPM._bodyAttached -= ConnectToBody;
    }

    //single hit
    public void SingleHit(float Damage, BasePlayerManager DamageBy)
    {
        _BPM._VFX.FBCall_CamShakeStart(2);
        _BPM._VFX.FBCall_GeneralDamageFlashStart(2);
        DamageBy._VFX.CLNT_PlayHittingEnemy();
        LastDamageBy = DamageBy;
        _BPM.HealthReduce(Damage);
    }


    //damage overtime

    Coroutine DmgOverTime;
    public void DamageOverTime(float BaseDamage, float TickRate, float Time)
    {
        _BPM._VFX.FBCall_FlameDamageStart();//feedback
        DmgOverTime = StartCoroutine(Coroutine_DamageOverTime(BaseDamage, TickRate, Time));
    }

    IEnumerator Coroutine_DamageOverTime(float BaseDamage, float TickRate, float Time)
    {
        float TimeElapsed = 0;

        while (TimeElapsed <= Time)
        {
            _BPM.HealthReduce(BaseDamage);
            TimeElapsed += TickRate;

            if (TimeElapsed >= Time)
            {
                Debug.Log("DAMAGE OVER TIME ENDDEDDDEDDE");
                _BPM._VFX.FBCall_FlameDamageStop();
            }
            yield return new WaitForSeconds(TickRate);
        }
    }


    //stun player
    public void StunPlayer(float Time,int DamageType)//0 for bullrush // 1 for electroball
    {
        if (_BPM.isClient) return;

        if (DamageType == 0)
            _BPM._VFX.FBCall_BullrushStunStart(2);

        if (DamageType == 1)
            _BPM._VFX.FBCall_ElectroballStunStart();


        StartCoroutine(_stunPlayer(Time));
    }

    IEnumerator _stunPlayer(float Time)
    {
        _BPM._MH.IsMovementAllowed = false;
        _BPM._WM.TurnOffWeapons(Time);
        yield return new WaitForSeconds(Time);
        _BPM._MH.IsMovementAllowed = true;

        _BPM._VFX.FBCall_BullrushStunStop();
        _BPM._VFX.FBCall_ElectroballStunStop();
    }



    // this is removing the slowed speed of the player due to the glue. the slow speed value is set directly in the bPM
    public bool isCollidingWithOtherGlue;
    bool isCoroutineRunning;
    public void RemoveSlowPlayer(float Time, float SlowValue)
    {
        if(isCoroutineRunning)
        {
            StopCoroutine(_RemoveslowPlayer(Time));
            StartCoroutine(_RemoveslowPlayer(Time));
        }
        else
        {
            StartCoroutine(_RemoveslowPlayer(Time));
        }
    }

    IEnumerator _RemoveslowPlayer(float Time)
    {
        isCoroutineRunning = true;
        yield return new WaitForSeconds(Time);

        if (!isCollidingWithOtherGlue)
        _BPM._MH.SlowedSpeed = 1;
        _BPM._VFX.FBCall_StickyGlueStop();
        isCoroutineRunning = false;
    }

    private void Update()
    {
        if(_BPM._MH.SlowedSpeed < 1)
        {
            StartCoroutine(_HardCodeRemoveslowPlayer(5));
        }
    }

    IEnumerator _HardCodeRemoveslowPlayer(float Time)
    {
        isCoroutineRunning = true;
        yield return new WaitForSeconds(Time);

        if (!isCollidingWithOtherGlue)
            _BPM._MH.SlowedSpeed = 1;
        _BPM._VFX.FBCall_StickyGlueStop();
        isCoroutineRunning = false;
    }
}
