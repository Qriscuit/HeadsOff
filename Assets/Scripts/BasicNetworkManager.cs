using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BasicNetworkManager : NetworkManager 
{
    public Body _AT9;
    public Transform BodySpawnParent;

    public override void OnStartServer()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject M = Instantiate(_AT9.gameObject, BodySpawnParent.GetChild(i).position, Quaternion.identity);
            NetworkServer.Spawn(M);
        }
    }
}
