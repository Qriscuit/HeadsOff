using HeadsOffGlobals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunBullet : MonoBehaviour
{
    public Rigidbody _RB;
    public VFXDestroyer Trail;
    public Team myTeam;
    public float BulletSpeed;

    void Start()
    {
        Invoke("DeleteBulletAndKillTrailInSeconds", 5f);
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

    public void LaunchBullet(Vector3 DirectionToGo)
    {
        _RB.velocity = DirectionToGo * BulletSpeed;
    }
}
