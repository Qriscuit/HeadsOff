using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ElectroBall : MonoBehaviour
{
    [Header("ClassRefrence")]
    public WeaponManager _WM;

    [Header("ElectroBall")]
    public GameObject EballPrefab; //BallPrefab
    public float ReloadTime;

    int LR;//LR=0 LeftHand; LR1=RightHand
    private bool _EBallAvailableToShootL = true;
    private bool _EBallAvailableToShootR = true;
   

    ElectroBallManager BallBeingShot;
    
    public void Launch(Vector3 BallSpawnPoint, Vector3 CameraForward, int _LR)//LR=0 LeftHand; LR=1 RightHand
    {
        if (_EBallAvailableToShootL&&_LR==0)
        {
            _WM._BPM._VFX.CMD_VFX_HmuzzleElectricL();
            _WM.CMD_SpawnEBall(BallSpawnPoint, CameraForward);
            _EBallAvailableToShootL = false;
            StartCoroutine(ReloadBallL());


        }

        if (_EBallAvailableToShootR&&_LR==1)
        {
            _WM._BPM._VFX.CMD_VFX_HmuzzleElectricR();
            _WM.CMD_SpawnEBall(BallSpawnPoint, CameraForward);
            _EBallAvailableToShootR = false;
            StartCoroutine(ReloadBallR());


        }
    }

    IEnumerator ReloadBallL()
    {
        DOVirtual.Float(0, 1, ReloadTime, updateLeftUI);
        yield return new WaitForSeconds(ReloadTime);
        _EBallAvailableToShootL = true;
    }

    void updateLeftUI(float value)
    {
        _WM._BPM._UIM.L_ElectroBall.fillAmount = value;
    }

    IEnumerator ReloadBallR()
    {
        DOVirtual.Float(0, 1, ReloadTime, updateRightUI);
        yield return new WaitForSeconds(ReloadTime);
        _EBallAvailableToShootR = true;
    }

    void updateRightUI(float value)
    {
        _WM._BPM._UIM.R_ElectroBall.fillAmount = value;
    }
}
