using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Collision : MonoBehaviour
{
    public ElectroBallManager _EBM;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==9)
        {
            //DamageManager DM = other.gameObject.GetComponent<DamageManager>();
           // DM.DamageOverTime(_EBM.DamageOverTimeDamage, _EBM.TickRate, _EBM.DamageOverTimeTime);
         //   DM._BPM._WM.TurnOffWeapons(_EBM.WeaponLockTime);
        }
    }

}
