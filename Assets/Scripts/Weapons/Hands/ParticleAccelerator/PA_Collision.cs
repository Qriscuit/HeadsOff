using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeadsOffGlobals;

public class PA_Collision : MonoBehaviour
{
    public ParticleAccelerator _PA;
    

    private void OnTriggerEnter(Collider other)
    {
        if (_PA.isClient)
            return;

        if(other.gameObject.layer == 9)
        {
            DamageManager DM = other.gameObject.GetComponent<DamageManager>();
            if (_PA.myTeam == DM._BPM._Team)
                if (!_PA._WM.FriendlyFire)
                    return;

            DM.SingleHit(_PA.damage, _PA._WM._BPM);
            _PA._WM._BPM._UIM.MakeDamageCrossHairAppear();
        }
        
    }
}
