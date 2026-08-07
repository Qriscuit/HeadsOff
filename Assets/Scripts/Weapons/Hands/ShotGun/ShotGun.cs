using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotGun : MonoBehaviour
{
    public WeaponManager _WM;

    public float ShotgunSpread;
    public int NumberOfPellets;

    public bool LShotReady = true;
    public bool RShotReady = true;

    public ShotGunBullet _ShotGunBullet;

    internal void LaunchBullet(Vector3 BulletSpawnPoint, Vector3 CameraForward, int _LR)//LR=0 LeftHand; LR=1 RightHand
    {
        if(_LR == 0 && LShotReady)
        {
            //LShotReady = false;
            _WM._BPM._VFX.CMD_VFX_HmuzzleSmokeL();
            for (int i = 0; i < NumberOfPellets; i++)
            {
                _WM.CMD_RaycastShotGunBullet(BulletSpawnPoint, CameraForward);
            }
        }
        else if(RShotReady)
        {
            //RShotReady = false;
            _WM._BPM._VFX.CMD_VFX_HmuzzleSmokeR();
            for (int i = 0; i < NumberOfPellets; i++)
            {
                _WM.CMD_RaycastShotGunBullet(BulletSpawnPoint, CameraForward);
            }
        }
    }
}
