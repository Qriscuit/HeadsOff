using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BubbleGunExplosion : MonoBehaviour
{
    public GameObject ExplosionEffectPrefab;

    GameObject ExplosionEffect;
    public SphereCollider _SphereCollider;
    [HideInInspector] public bool ExplosionStarted = false;

    public float SizeToGrowUpTo;

    [Header("Class References")]
    public BubbleCollision _BSM;
    DamageManager _DM;

    public void StartExplosion()
    {
        ExplosionEffect = Instantiate(ExplosionEffectPrefab, transform.position, Quaternion.Euler(_BSM.ExplosionPoint));
        _BSM.StartExplosion();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_BSM.isClient)
            return;
        Debug.Log("server collision");

        if (other.gameObject.layer == 9 && other.transform.CompareTag("DamageCollider"))
        {

            float TotalDamage = _BSM.BallDamage;

            _DM = other.GetComponent<DamageManager>();

            if (_BSM.myTeam == _DM._BPM._Team)
                if (!_DM._BPM._WM.FriendlyFire)
                    return;

            _DM.SingleHit(TotalDamage, _BSM._BPM);
            _BSM._BPM._UIM.MakeDamageCrossHairAppear();

            Debug.Log(TotalDamage);

            Vector3 directionVector = _BSM.gameObject.transform.position - other.transform.position;
            float AngleOfHit = Vector3.SignedAngle(directionVector, other.transform.forward, Vector3.up);
            //PlayerHit._AM.Stagger(AngleOfHit);
        }
        else
        {
       
        }
    }
}
