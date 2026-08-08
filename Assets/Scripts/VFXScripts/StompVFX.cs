using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompVFX : MonoBehaviour
{
    private void Awake()
    {
        Destroy(this.gameObject, 2f);   
    }
}
