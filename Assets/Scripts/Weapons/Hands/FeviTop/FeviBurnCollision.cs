using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeviBurnCollision : MonoBehaviour
{
    [SerializeField] FeviTopChild _FTC;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer==11)
        {
            Debug.Log("burnnnn");
        }
    }
}
