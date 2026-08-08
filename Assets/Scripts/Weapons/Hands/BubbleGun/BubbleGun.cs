using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGun : MonoBehaviour
{

    [Header("Values")]
    public float tickRate;
    float shootWaitTime;

    public float BubbleReloadTime;

    public int maxBullet;
    public int BulletsL;
    public int BulletsR;

    public float Damage;

    [Header("ClassRefrence")]
    public WeaponManager _WM;

    [Header("BubbleGun")]
    public GameObject BubbleGunPrefab;


    private bool _BigshotAvailableToShootL = true;
    private bool _BigshotAvailableToShootR = true;

    Coroutine _coroutineL;
    Coroutine _coroutineR;

    Coroutine _ReloadL;
    Coroutine _ReloadR;
    public void Launch(Vector3 BallSpawnPoint, Vector3 CameraForward, int _LR)//LR=0 LeftHand; LR=1 RightHand
    {
        if(_LR==0 && _BigshotAvailableToShootL)
        {
            if(_ReloadL!=null)
                StopCoroutine(_ReloadL);

            _coroutineL = StartCoroutine(startTickingL());
        }

        if(_LR == 1 && _BigshotAvailableToShootR)
        {
            if (_ReloadR != null)
                StopCoroutine(_ReloadR);

            _coroutineR = StartCoroutine(startTickingR());
        }

    }


    IEnumerator startTickingL()
    {
        while(BulletsL >0)
        {
            _WM._BPM._VFX.CMD_VFX_HmuzzleSmokeL();
            _WM.CMD_SpawnBubble(_WM.LeftHandSpawnPoint, _WM.targetPoint);
            _WM._BPM._UIM.L_BubbleGun.fillAmount = 1 - ((float)BulletsL / (float)maxBullet);

            BulletsL--;
            yield return new WaitForSeconds(tickRate);
            if(BulletsL==0)
            {
                _BigshotAvailableToShootL = false;
            }
        }
    }

    IEnumerator startTickingR()
    {
        while (BulletsR > 0)
        {
            _WM._BPM._VFX.CMD_VFX_HmuzzleSmokeR();
            _WM.CMD_SpawnBubble(_WM.RightHandSpawnPoint, _WM.targetPoint);

            _WM._BPM._UIM.R_BubbleGun.fillAmount = 1 - ((float)BulletsR / (float)maxBullet);
            BulletsR--;
            yield return new WaitForSeconds(tickRate);
            if (BulletsR == 0)
            {
                _BigshotAvailableToShootR = false;
            }
        }
    }

    IEnumerator ReloadL()
    {
        //DOVirtual.Float(_WM._BPM._UIM.L_BubbleGun.fillAmount, 0, BubbleReloadTime, updateLeftUI);
        int bulletToload = maxBullet - BulletsL;

        while(bulletToload>0)
        {
            yield return new WaitForSeconds((BubbleReloadTime / maxBullet));
            BulletsL++;
            bulletToload--;
            _WM._BPM._UIM.L_BubbleGun.fillAmount = 1 - ((float)BulletsL / (float)maxBullet);
            _BigshotAvailableToShootL = true;
        }
    }

    IEnumerator ReloadR()
    {
        //DOVirtual.Float(1, 0, BubbleReloadTime, updateRightUI);

        int bulletToload = maxBullet - BulletsR;

        while (bulletToload > 0)
        {
            yield return new WaitForSeconds((BubbleReloadTime / maxBullet));
            BulletsR++;
            bulletToload--;
            _WM._BPM._UIM.R_BubbleGun.fillAmount = 1 - ((float)BulletsR / (float)maxBullet);
            _BigshotAvailableToShootR = true;
        }
    }

    void updateLeftUI(float value)
    {
        _WM._BPM._UIM.L_BubbleGun.fillAmount = value;
    }

    void updateRightUI(float value)
    {
        _WM._BPM._UIM.R_BubbleGun.fillAmount = value;
    }


    public void DeLaunchL()
    {
        if(_coroutineL!=null)
            StopCoroutine(_coroutineL);

        _ReloadL = StartCoroutine(ReloadL());
    }

    public void DeLaunchR()
    {
        if (_coroutineR != null)
            StopCoroutine(_coroutineR);

        _ReloadR = StartCoroutine(ReloadR());
    }
}
