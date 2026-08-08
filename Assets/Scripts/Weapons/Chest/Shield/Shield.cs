using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Shield : MonoBehaviour
{
    public WeaponManager _WM;

    public float _ShieldHealth = 50f;
    public float ShieldHealth
    {
        get
        {
            return _ShieldHealth;
        }

        set
        {
            _ShieldHealth = value;
            _WM._BPM._UIM.Shield.fillAmount = _ShieldHealth / 50;
            if (_WM.isServer) _WM.CLNT_UpdateShieldHealth(_ShieldHealth);
        }
    }

    public float ShieldRegenWait = 5f;
    public float ShieldRegenValuePerTick = 5f;
    public float ShieldRegenTickRate = 0.5f;
    
    public bool IsPersonalShieldActive = true;
    
    public ShieldBall _shieldBall;
    public ShieldBall _SpawnedBall;

    public void LaunchBall(Vector3 _Dir, Vector3 _SpawnPoint)
    {
        if(_SpawnedBall == null) _WM.CMD_SpawnShield(_SpawnPoint, _WM._BPM._Cam.transform.forward);
    }

    Coroutine RegenStarted;
    public float DecreaseHealthBy(float Value)
    {
        if (RegenStarted == null)
        {
            RegenStarted = StartCoroutine(RegenShieldHealth());
        }
        else
        {
            StopCoroutine(RegenStarted);
            RegenStarted = StartCoroutine(RegenShieldHealth());
        }
        
        if (IsPersonalShieldActive)
        {
            ShieldHealth -= Value;
            if (ShieldHealth <= 0)
            {
                IsPersonalShieldActive = false;
                _WM._BPM._AM.LaunchBattery();
                return Mathf.Abs(ShieldHealth);
            } 
            else return 0;
        } 

        return Value;
    }
    
    IEnumerator RegenShieldHealth()
    {
        yield return new WaitForSeconds(ShieldRegenWait);

        while (ShieldHealth < 50)
        {
            yield return new WaitForSeconds(ShieldRegenTickRate);
            ShieldHealth += ShieldRegenValuePerTick;
            if (ShieldHealth > 45 && !IsPersonalShieldActive)
            {
                _WM._BPM._AM.ReAttachBattery();
                IsPersonalShieldActive = true;
            }
        }
    }
}
