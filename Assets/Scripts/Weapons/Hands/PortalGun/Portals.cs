using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Portals : NetworkBehaviour
{
    public enum TeleporterName
    {
        TeleporterA, TeleporterB
    }

    public TeleporterName _teleporterName;
    public Portals teleportOtherEnd;
    public bool isActive = false;
    private void Awake()
    {
        isActive = true;
    }

    
    private void OnTriggerEnter(Collider player)
    {
       
        if(teleportOtherEnd!=null)
            if (teleportOtherEnd.isActive)
            {
                if (player.gameObject.layer == 9)
                {
                    Debug.Log("colliding with player");
                    if (_teleporterName == TeleporterName.TeleporterA)
                    {
                        //player.transform.parent.gameObject.transform.forward = teleportOtherEnd.transform.forward;


                        player.transform.parent.gameObject.transform.position = teleportOtherEnd.transform.TransformPoint(new Vector3(0, 0, 5));
                        player.transform.parent.gameObject.transform.rotation = Quaternion.Euler(player.transform.parent.gameObject.transform.eulerAngles.x, player.transform.parent.gameObject.transform.eulerAngles.y + 180, player.transform.parent.gameObject.transform.eulerAngles.z);
                    }
                    else
                    {
                        //player.transform.parent.gameObject.transform.forward = teleportOtherEnd.transform.forward;
                        player.transform.parent.gameObject.transform.position = teleportOtherEnd.transform.TransformPoint(new Vector3(0, 0, 5));
                        player.transform.parent.gameObject.transform.rotation = Quaternion.Euler(player.transform.parent.gameObject.transform.eulerAngles.x, player.transform.parent.gameObject.transform.eulerAngles.y - 180, player.transform.parent.gameObject.transform.eulerAngles.z);
                    }

                }

            }
    }
}
