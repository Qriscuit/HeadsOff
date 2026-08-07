using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PA_VFX : MonoBehaviour
{
    [SerializeField] Transform ColliderTransform;
    [SerializeField] VisualEffect visualEffect;
    [SerializeField] float Lasersize;
    private void Awake()
    {
        visualEffect = this.gameObject.GetComponent<VisualEffect>();
    }


    // Update is called once per frame
    void Update()
    {
        Lasersize = ColliderTransform.localScale.z * 14.4f; // for some reason 1 Z unit in transform equals 14.4 vfx laser size
        transform.eulerAngles = ColliderTransform.eulerAngles;
        visualEffect.SetFloat("LaserSize", Lasersize);
    }
}
