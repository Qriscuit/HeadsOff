using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Rigidbody _RB;
    public VFXDestroyer Trail;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void Launch()
    {

    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 7)
        {
            DeleteBulletAndKillTrailInSeconds();
        }
    }

    void DeleteBulletAndKillTrailInSeconds()
    {
        Trail.transform.parent = null;
        Trail.DeleteTime(1.5f);

        Destroy(gameObject);
    }
}
