using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BullRush : MonoBehaviour
{
    [Header("ClassRefrence")]
    public WeaponManager _WM;

    [Header("BullRush")]
    public float BullMoveSpeed;
    public bool IsBullRunAllowed=false;
    public float BullRushCoolDown;
    public bool BRcoolDown=false;
    public BoxCollider _BullCol;
    public float StunTime = 3;
    [HideInInspector] public bool FriendlyFire
    {
        get
        {
            return _WM.FriendlyFire;
        }
    }

    private void Start()
    {
        IsBullRunAllowed = false;
        BRcoolDown = false;
    }

    public void Launch()
    {
        if (IsBullRunAllowed)
        {
            TurnOffBullRush();
        }
        else
        if (BRcoolDown == false)
        {
            IsBullRunAllowed = true;
            _BullCol.enabled = true;
            _WM.CMD_ToggleBullRushValues(true);
            _WM._BPM._AM._fna.SetTrigger("BullRush Start");
            _WM._BPM._VFX.CMD_VFX_LbullRushStart();
            _WM._BPM._VFX.VFX_DashActive(10, false);
            _WM._BPM._MH.IsMovementAllowed = false;
            BRcoolDown = true;
        }
    }

    private void Update()
    {
        if (_WM._BPM.isServer) return;
        if (IsBullRunAllowed)
        {
            _WM._BPM._MH._CC.Move(transform.up * _WM._BPM._MH.InitialGravities[0] * BullMoveSpeed * Time.deltaTime);
            _WM._BPM._MH._CC.Move(_WM._BPM.transform.forward * BullMoveSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //_WM._BPM._VFX.CMD_VFX_LBullStop();
        if (!IsBullRunAllowed)
            return;
        
        if (other.gameObject.layer == 9)
        {
            Debug.Log("code to check other network player and the others name is " + other.gameObject.name);
            TurnOffBullRush();

            DamageManager DM = other.gameObject.GetComponent<DamageManager>();

            if (_WM._BPM.isClient)
                return;

            if (!FriendlyFire)
                if (DM._BPM._Team == _WM._BPM._Team) return;
            
            DM.StunPlayer(2,0);
            DM.SingleHit(40f, _WM._BPM);
            _WM._BPM._UIM.MakeDamageCrossHairAppear();
        }

        if (other.gameObject.layer == 7 || other.gameObject.layer == 6)
        {
            TurnOffBullRush();
        }
    }

    void TurnOffBullRush()
    {
        IsBullRunAllowed = false;
        _WM._BPM._MH.IsMovementAllowed = true;

        if (_WM._BPM.isClient)
        {
            _WM.CMD_ToggleBullRushValues(false);
            _WM._BPM._VFX.VFX_LbullRushStop();
        }
        else
        {
            _WM._BPM._VFX.CLNT_VFX_LbullRushStop();
        }

        _WM._BPM._VFX.VFX_DashActiveStop();
        _WM._BPM._AM._fna.SetTrigger("BullRush Collision");
        StartCoroutine(StartCoolDown());
    }

    IEnumerator StartCoolDown()
    {
        DOVirtual.Float(0, 1, BullRushCoolDown, UpdateLegUI);
        yield return new WaitForSeconds(BullRushCoolDown);
        BRcoolDown = false;
    }

    void UpdateLegUI(float value)
    {
        _WM._BPM._UIM.BullRush.fillAmount = value;
    }
}
