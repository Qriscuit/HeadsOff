using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PunchGlove : MonoBehaviour
{
    public WeaponManager _WM;
    public GameObject _SpawnedBasePrefab;
    [SerializeField] float reloadTime;
    [SerializeField] bool isAllowedL;
    [SerializeField] bool isAllowedR;

    private void Awake()
    {
        reloadTime = 2f;
        isAllowedL = true;
        isAllowedR = true;
    }
    public void LaunchGlove(Vector3 _Dir, Vector3 SpawnPoint, int LR)//LR=0 LeftHand; LR=1 RightHand
    {
        if(LR==0 && isAllowedL)
        {
            //shotballL
            _WM._BPM._VFX.CMD_VFX_HmuzzleRingL();
            _WM.CMD_SpawnPunch(SpawnPoint, _Dir);
            isAllowedL = false;
            StartCoroutine(ReloadBallL());
            _WM._BPM._PAM.Punch.Play();
        }
        else if (LR == 1 && isAllowedR)
        {
            //shotballR
            _WM._BPM._VFX.CMD_VFX_HmuzzleRingR();
            _WM.CMD_SpawnPunch(SpawnPoint, _Dir);
            isAllowedR = false;
            StartCoroutine(ReloadBallR());
            _WM._BPM._PAM.Punch.Play();
        }

    }

    IEnumerator ReloadBallL()
    {
        DOVirtual.Float(0, 1, reloadTime, updateLeftUI);
        yield return new WaitForSeconds(reloadTime);
        isAllowedL = true;
    }
    void updateLeftUI(float value)
    {
        _WM._BPM._UIM.L_PunchGlove.fillAmount = value;
    }


    IEnumerator ReloadBallR()
    {
        DOVirtual.Float(0, 1, reloadTime, updateRightUI);
        yield return new WaitForSeconds(reloadTime);
        isAllowedR = true;
    }

    void updateRightUI(float value)
    {
        _WM._BPM._UIM.R_PunchGlove.fillAmount = value;
    }
}
