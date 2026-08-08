using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : MonoBehaviour
{
    public WeaponManager _WM;
    
    public bool LShotReady = true;
    public bool RShotReady = true;

    public MachineGunBullet _MachineGunBullet;

    internal void LaunchBullet(Vector3 BulletSpawnPoint, Vector3 CameraForward, int _LR)//LR=0 LeftHand; LR=1 RightHand
    {
        if (_LR == 0 && LShotReady)
        {
            //LShotReady = false;
            _WM._BPM._VFX.CMD_VFX_HmuzzleSmokeL();
            _WM.CMD_SpawnMachineGunBullet(BulletSpawnPoint, CameraForward);
        }
        else if (RShotReady)
        {
            //RShotReady = false;
            _WM._BPM._VFX.CMD_VFX_HmuzzleSmokeR();
            _WM.CMD_SpawnMachineGunBullet(BulletSpawnPoint, CameraForward);
        }
    }
}
