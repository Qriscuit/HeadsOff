
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGun : MonoBehaviour
{
    public LayerMask PortalGunLayerMask;
    public WeaponManager _WM;
    [SerializeField] int PortalCount;

    public Portals TelePorter_A;
    public Portals TelePorter_B;

    public GameObject TeleGameObjA;
    public GameObject TeleGameObjB;

    public Vector3 HitNormalData;
    public void Launch(Vector3 _dir, Vector3 ShootPos)
    {
        Vector3 Direction = _dir - ShootPos;
        RaycastHit hit;
        if (Physics.Raycast(ShootPos, Direction.normalized, out hit, 1000, PortalGunLayerMask))
        {
            HitNormalData = hit.normal;

            Debug.Log("hit.normal is " + hit.normal +
                " angle is " + Vector3.Angle(hit.normal, Vector3.up));

            if (Vector3.Angle(hit.normal, Vector3.up) < 75) return;
            _WM._BPM._PAM.Portal.Play();
            if (PortalCount == 0)
            {
                //TelePorter_A = PhotonNetwork.Instantiate("Prefabs/Hands/PortalGun/teleporterA", hit.point + hit.normal + new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Portals>();//add value in vector 3 for offset 
                _WM.CMD_SpawnPortal(hit.point + hit.normal*2, PortalCount, hit.normal);
            }
            else
            if (PortalCount == 1)
            {
                //TelePorter_B = PhotonNetwork.Instantiate("Prefabs/Hands/PortalGun/teleporterB", hit.point + hit.normal + new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Portals>();
                _WM.CMD_SpawnPortal(hit.point + hit.normal*2, PortalCount, hit.normal);
            }
            else
            {
                _WM.CMD_SpawnPortal(hit.point + hit.normal + new Vector3(0, 0, 0), PortalCount, hit.normal);
                PortalCount = 0;
            }

            Debug.Log("the portal raycast detected a collider " + hit.collider.gameObject.name);
        }
        else
        {
            Debug.Log("the portal raycast did not detect a collider ");
        }
    }

    public void SetValuePortalA()
    {
        //TelePorter_A.transform.eulerAngles += HitNormalData;
        //TelePorter_A.transform.forward = HitNormalData;
        PortalCount=1;
    }

    public void SetValuePortalB()
    {
        //TelePorter_B.transform.eulerAngles += HitNormalData;
        //TelePorter_B.transform.forward = HitNormalData;
        TelePorter_B.teleportOtherEnd = TelePorter_A;
        TelePorter_A.teleportOtherEnd = TelePorter_B;
        PortalCount=2;
    }

    // Refference vector 1 = transform.up
    // main vector 1 = hit.normal
    // Refference vector 2 = transform.forward
}
