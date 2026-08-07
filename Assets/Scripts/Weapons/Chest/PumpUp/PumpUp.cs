using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpUp : MonoBehaviour
{
    public WeaponManager _WM;
    public GameObject PumpUpVFX;
    public float DamageAbsorptionPercentage;
    public float DamageMultiplierOnActivated;

    public float ScaleValue;
    public float ScaleTime;
    public float DeleteAfterScaleTime;
    [SerializeField] Vector3 SpawnOffset;
    /// <summary>
    /// How long the pump ups ability runs for
    /// </summary>
    public float PumpUpTime;

    public float PumpUpDamage;
    Coroutine Ability;

    [SerializeField]bool isPumpingAllowed;

    float _CurrentPumpLevel;
    public float CurrentPumpLevel
    {
        get
        {
            return _CurrentPumpLevel;
        }

        set
        {
            _CurrentPumpLevel = value;

            if (_CurrentPumpLevel >= 100)
            {
                PumpedAndReady = true;
                _CurrentPumpLevel = 100;
            }

            _WM._BPM._UIM.PumpUp.fillAmount = _CurrentPumpLevel / 100;

            if (_WM.isServer) _WM.CLNT_UpdateCurrentPumpLevel(_CurrentPumpLevel);
            
        }
    }

    public bool PumpedAndReady = false;
    public bool AbilityInUse = false;      //To Be Placed in the upper class when the scriptableObjct thing comes in place
    public bool FriendlyFire;
    private float VFXDeleteAfterThisTime = 0.5f;

    public float NegateDamage(float Damage)
    {
        float NewDamage = Damage - (Damage * (DamageAbsorptionPercentage / 100));

        CurrentPumpLevel += 35;


        //PlayVFX();


        return NewDamage;
    }

    void PlayVFX()
    {

    }

    public void Launch()
    {
        if (!isPumpingAllowed) return;
        if (!PumpedAndReady) return;

        PumpedAndReady = false;

        

        CurrentPumpLevel = 0;
        _WM.CMD_SpawnPumpUp(_WM._BPM.transform.TransformPoint(SpawnOffset),ScaleValue, ScaleTime, PumpUpDamage, VFXDeleteAfterThisTime);
        _WM._BPM._AM._fna.SetTrigger("PumpUpExplosion");
        _WM._BPM._MH.IsMovementAllowed = false;
        isPumpingAllowed = false;
        StartCoroutine(DeActivatePumpUp());
    }

    IEnumerator DeActivatePumpUp()
    {
        yield return new WaitForSeconds(ScaleTime+1);
        _WM._BPM._MH.IsMovementAllowed = true;
        isPumpingAllowed = true;
    }

    //public void AbilityStarted()
    //{
    //    PumpedAndReady = true;
    //    AbilityInUse = true;

    //    Ability = StartCoroutine(PumpUpAbilityCountDown());
    //}

    //IEnumerator PumpUpAbilityCountDown()
    //{
    //    yield return new WaitForSeconds(PumpUpTime);

    //    AbilityEnded();
    //}

    //void AbilityEnded()
    //{
    //    PumpedAndReady = false;
    //    AbilityInUse = false;

    //    CurrentPumpLevel = 0;
    //}
}
