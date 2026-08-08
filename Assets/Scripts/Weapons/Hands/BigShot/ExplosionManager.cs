using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExplosionManager : MonoBehaviour
{
    public GameObject ExplosionEffectPrefab;
    
    GameObject ExplosionEffect;
    public SphereCollider _SphereCollider;
    [HideInInspector] public bool ExplosionStarted = false;

    public float SizeToGrowUpTo;

    [Header("Class References")]
    public BigShotManager _BSM;
    DamageManager DM;
    
    public void StartExplosion()
    {
        ExplosionEffect = Instantiate(ExplosionEffectPrefab, transform.position, Quaternion.Euler( _BSM.ExplosionPoint));
        _BSM.StartExplosion();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (_BSM.isClient)
            return;

        Debug.Log("servercollision");
        if (other.gameObject.layer==9 && other.transform.CompareTag("DamageCollider"))
        {
       
            Debug.Log("collision check");

            float scale = transform.localScale.x;

            if (scale<SizeToGrowUpTo)
            {
                scale = 0f;
            }

            float TotalDamage = _BSM.BallDamage-scale;
            
            DM = other.GetComponent<DamageManager>();

            if (_BSM.myTeam == DM._BPM._Team)
                if (!DM._BPM._WM.FriendlyFire)
                    return;

            Debug.Log("inside friendly fire");
            DM.SingleHit(TotalDamage, _BSM._BPM);
            Debug.Log(TotalDamage);
            
            Vector3 directionVector = _BSM.gameObject.transform.position - other.transform.position;
            float AngleOfHit = Vector3.SignedAngle(directionVector, other.transform.forward, Vector3.up);

            _BSM._BPM._UIM.MakeDamageCrossHairAppear();
            //PlayerHit._AM.Stagger(AngleOfHit);
        }
    }
}
