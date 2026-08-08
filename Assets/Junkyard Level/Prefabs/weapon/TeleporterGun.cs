using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterGun : MonoBehaviour
{
    public enum TeleporterName
    {
        TeleporterA, TeleporterB
    }

    public TeleporterName _teleporterName;
    public TeleporterGun teleportOtherEnd;
    public bool isActive=false;
    private void Awake()
    {
        isActive = true;
    }
    private void OnTriggerEnter(Collider player)
    {
        Debug.Log(teleportOtherEnd.isActive);
        if (teleportOtherEnd.isActive)
        {
            
            if (player.gameObject.layer == 14)
            {
                Debug.Log("teleported");

                if (_teleporterName == TeleporterName.TeleporterA)
                {
                    player.transform.parent.gameObject.transform.position = teleportOtherEnd.transform.TransformPoint(new Vector3(5, 0, 0));

                }
                else
                {
                    player.transform.parent.gameObject.transform.position = teleportOtherEnd.transform.TransformPoint(new Vector3(-5, 0, 0));
                }
            }

        }

    }
}
