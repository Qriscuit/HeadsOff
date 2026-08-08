using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineDamageCollision : MonoBehaviour
{
    [HideInInspector] public Mine _Mine;
    public SphereCollider _SphereCollider;

    List<DamageManager> Managers = new List<DamageManager>();

    private void Awake()
    {
        _Mine = GetComponentInParent<Mine>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (_Mine.isClient) return;
        if (!_Mine.IsExploding) return;
        if (other.gameObject.layer != 9) return;

        //if (MainGameManager.Inst.IsOtherOfMyTeam(other.gameObject.transform.parent.gameObject)) return;
        Debug.Log("Hit Registered");

        DamageManager _DM = other.GetComponent<DamageManager>();

        if (!Managers.Contains(_DM))
        {
            if (_Mine._Team == _DM._BPM._Team)
                if (!_DM._BPM._WM.FriendlyFire)
                    return;
            
            Debug.Log("In Hit");

            _DM.SingleHit(_Mine.Damage, _Mine._BPM);
            _Mine._BPM._UIM.MakeDamageCrossHairAppear();

            Managers.Add(_DM);
        }
    }
}
