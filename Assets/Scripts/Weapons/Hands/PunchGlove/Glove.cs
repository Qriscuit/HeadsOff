using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glove : MonoBehaviour
{
    /*
    //damage is being checked locally
    public PG_BaseLaunchPoint _PG;
    public DamageManager _DM;
    private void Awake()
    {
        _PG = GetComponentInParent<PG_BaseLaunchPoint>();
    }

    private void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.layer != 9) return;
        _DM = other.GetComponent<DamageManager>();

        if (_DM._BPM._Team == _PG.MyTeam) return;
        _DM.SingleHit(_PG.Damage);
    }*/

}
