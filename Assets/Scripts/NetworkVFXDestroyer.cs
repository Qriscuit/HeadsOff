using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class NetworkVFXDestroyer : NetworkBehaviour
{
    [SerializeField] float Time;

    void Start()
    {
        StartCoroutine(Destroythis());
    }

    IEnumerator Destroythis()
    {
        yield return new WaitForSeconds(Time);
        NetworkServer.Destroy(this.gameObject);
    }
}
