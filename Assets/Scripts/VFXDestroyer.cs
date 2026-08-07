using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXDestroyer : MonoBehaviour
{
    public bool SurviveOnStart = false;
    public float lifetime = 1.2f;
    void Start()
    {
        if(!SurviveOnStart) Destroy(this.gameObject, lifetime);
    }

    public void DeleteTime(float Time)
    {
        Destroy(gameObject, Time);
    }
}
