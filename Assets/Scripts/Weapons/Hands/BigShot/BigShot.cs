using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BigShot : MonoBehaviour
{

    [Header("ClassRefrence")]
    public WeaponManager _WM;

    [Header("BigShot")]
    public GameObject BigShotPrefab;
    [HideInInspector] public BigShotManager BallBeingShot;
    public float BigShotReloadTime;

    private bool _BigshotAvailableToShootL=true;
    private bool _BigshotAvailableToShootR=true;

    //public ParticleSystem VFX_MuzzleRingL, VFX_MuzzleRingR;

    public void Launch(Vector3 BallSpawnPoint, Vector3 CameraForward, int _LR)//LR=0 LeftHand; LR=1 RightHand
    {
        
        if (_BigshotAvailableToShootL && _LR==0)
        {
            //VFX_MuzzleRingL.Play();
            _WM._BPM._VFX.CMD_VFX_HmuzzleSmokeL();
            //BallBeingShot = Instantiate(BigShotPrefab, transform.TransformPoint(BallSpawnPoint), Quaternion.identity).GetComponent<BigShotManager>();

            _WM.CMD_SpawnBigShot(BallSpawnPoint, CameraForward);
            
            _BigshotAvailableToShootL = false;
            StartCoroutine(ReloadBallL());

        }
        if (_BigshotAvailableToShootR&& _LR==1)
        {
            //VFX_MuzzleRingR.Play();
            _WM._BPM._VFX.CMD_VFX_HmuzzleSmokeR();
            //BallBeingShot = Instantiate(BigShotPrefab, transform.TransformPoint(BallSpawnPoint), Quaternion.identity).GetComponent<BigShotManager>();

            _WM.CMD_SpawnBigShot(BallSpawnPoint, CameraForward);

            _BigshotAvailableToShootR = false;
            StartCoroutine(ReloadBallR());


        }
    }
    IEnumerator ReloadBallL()
    {
        DOVirtual.Float(0, 1, BigShotReloadTime,updateLeftUI);
        yield return new WaitForSeconds(BigShotReloadTime);
        _BigshotAvailableToShootL = true;
    }

    void updateLeftUI(float value)
    {
        _WM._BPM._UIM.L_BigShot.fillAmount = value;
    }


    IEnumerator ReloadBallR()
    {
        DOVirtual.Float(0, 1, BigShotReloadTime, updateRightUI);
        yield return new WaitForSeconds(BigShotReloadTime);
        _BigshotAvailableToShootR = true;
    }

    void updateRightUI(float value)
    {
        _WM._BPM._UIM.R_BigShot.fillAmount = value;
    }
}
