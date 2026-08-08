using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StompLegs : MonoBehaviour
{

    [Header("ClassRefrence")]
    public WeaponManager _WM;

    [Header("StompLegs")]
    public float StompMoveSpeed;
    public float StompCoolDownTime;
    bool IsStompAllowed=true;
    bool IsStomping;

    [Header("ShockWave")]
    public GameObject _StompShockWaveParent;
    StompShockWave _StompShockWave;
    public GameObject StompVFX;
    [SerializeField] float TimeStomped;
    [SerializeField] float ScaleMultiplyer;
    [SerializeField] float MaxScale;
    [SerializeField] float MinScale;
    [HideInInspector]
    public bool FriendlyFire
    {
        get
        {
            return _WM.FriendlyFire;
        }
    }

    private void Start()
    {
        {
            IsStompAllowed = true;
            IsStomping = false;
        }
    }
    public void Launch()
    {
        if (IsStompAllowed)
        {
            if (_WM._BPM._MH._CC.isGrounded == false)
            {
                Debug.Log(_WM._BPM._MH._CC.isGrounded);
                _WM._BPM._MH.IsMovementAllowed = false;
                IsStomping = true;
                IsStompAllowed = false;
                TimeStomped = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(IsStomping)
        {
            _WM._BPM._MH._CC.Move(-transform.up * StompMoveSpeed * Time.deltaTime);
            TimeStomped += Time.deltaTime;
            if (_WM._BPM._MH._CC.isGrounded==true)
            {
                IsStomping = false;
                _WM._BPM._VFX.FB_CamShake.PlayFeedbacks();
                StartCoroutine(StompingCoolDown());
                float EndValue = TimeStomped * ScaleMultiplyer;
                Mathf.Clamp(EndValue, MinScale, MaxScale);
                //Instantiate(StompVFX, transform.position, Quaternion.identity);
                //_StompShockWave = Instantiate(_StompShockWaveParent, _WM._BPM.transform.position, Quaternion.identity);
                _WM.CMD_SpawnStomp(_WM._BPM.transform.position,EndValue);
                //_StompShockWave.StartScallingRPCCaller(EndValue);
                _WM._BPM._MH.IsMovementAllowed = true;
            }
        }
    }

    IEnumerator StompingCoolDown()
    {
        DOVirtual.Float(0, 1, StompCoolDownTime, UpdateLegUI);
        yield return new WaitForSeconds(StompCoolDownTime);
        IsStompAllowed = true;
    }



    void UpdateLegUI(float value)
    {
        _WM._BPM._UIM.Stomp.fillAmount = value;
    }

    public void HitCallBack(GameObject PlayerGotHit)
    {
        //Damage or Stuffs
        
    }

}
