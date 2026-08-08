using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Latch : MonoBehaviour
{
    BasePlayerManager _BPM;

    public GameObject _Latcher;

    public LatchFront SpawnedLatcher;
    //public LatchCamera _LatchCam;

    public LayerMask Mask;
    
    private void Awake()
    {
        _BPM = transform.parent.parent.GetComponent<BasePlayerManager>();
    }
    
    public void Launch(Vector3 _dir, Vector3 SpawnPoint)
    {
        if(SpawnedLatcher == null)
        {
            SpawnedLatcher = Instantiate(_Latcher, transform.TransformPoint(SpawnPoint), Quaternion.identity).GetComponent<LatchFront>();
            SpawnedLatcher.gameObject.transform.forward = _dir;
            SpawnedLatcher._BPM = _BPM;
            SpawnedLatcher.Launch(_dir);
        }
        else
        {/*
                _BPM._MH.IsMovementAllowed = true;
            _BPM._LC.enabled = false;
            _BPM._Cam.enabled = true;

            Destroy(SpawnedLatcher);*/
        }
    }
}
