using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCollectionCollider : MonoBehaviour
{
    [HideInInspector] public BasePlayerManager _BPM;
    [HideInInspector] public SphereCollider CollectionCollider;

    private void Awake()
    {
        _BPM = GetComponentInParent<BasePlayerManager>();
        CollectionCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 && other.gameObject.tag != "Internal") //TODO remeber this number mus be changed everytime the project changes kyuki "body" layer ka number alag alag ho jayega
        {
            _BPM._MM.BodiesCloseBy.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6) //TODO remeber this number mus be changed everytime the project changes kyuki "body" layer ka number alag alag ho jayega
        {
            _BPM._MM.BodiesCloseBy.Remove(other.gameObject);
        }
    }
}
