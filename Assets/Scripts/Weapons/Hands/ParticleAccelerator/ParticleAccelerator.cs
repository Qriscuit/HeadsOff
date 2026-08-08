using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using HeadsOffGlobals;
using System;
using DG.Tweening;

public class ParticleAccelerator : NetworkBehaviour
{ 
    [Header("ClassRefrence")]
    public WeaponManager _WM;
    public Team myTeam
    {
        get
        {
            return _WM._BPM._Team;
        }
    }

    [Header("ParticleAccelerator")]
    public float MaxDistance;
    public LayerMask _LayersToReflect;

    public GameObject CylinderCollider;

    private Ray rayL;
    private Ray rayR;

    private RaycastHit hitL;
    private RaycastHit hitR;

    private Vector3 AngleOfReflectionL;
    private Vector3 AngleOfReflectionR;

    public int MaxReflections = 1;
    public Vector3 ScallingFactor;
    public float damage = 10;

    public PA_Collision[] _PA_CollisionL;
    public PA_Collision[] _PA_CollisionR;


    public bool IsPAallowedL=true;
    public bool IsPAallowedR = true;

    [SerializeField] float PA_cooldownTime;
    [SerializeField] float PA_DestroyTime;

    //recharge Var
    bool isRechargingL;
    bool isRechargingR;

    float TimeRechargedL;
    float TimeRechargedR;

    [SyncVar(hook = nameof(LHRefUpdate))] public int nReflectionsL;
    void LHRefUpdate(int oldValue, int NewValue)
    {
        if (NewValue == 0)
            _WM._BPM._NNBody._LHParticleAcceleratorAccentsAnimator.SetTrigger("Release");
        if (NewValue == 1)
            _WM._BPM._NNBody._LHParticleAcceleratorAccentsAnimator.Play("PA_LH_First Charge");
        if (NewValue == 2)                                                
            _WM._BPM._NNBody._LHParticleAcceleratorAccentsAnimator.Play("PA_LH_Second Charge");
        if (NewValue == 3)                                                
            _WM._BPM._NNBody._LHParticleAcceleratorAccentsAnimator.Play("PA_LH_Third Charge");
    }

    [SyncVar(hook = nameof(RHRefUpdate))] public int nReflectionsR;
    void RHRefUpdate(int oldValue, int NewValue)
    {
        if (NewValue == 0)
            _WM._BPM._NNBody._RHParticleAcceleratorAccentsAnimator.SetTrigger("Release");
        if (NewValue == 1)  
            _WM._BPM._NNBody._RHParticleAcceleratorAccentsAnimator.Play("PA_RH_First Charge");
        if (NewValue == 2)  
            _WM._BPM._NNBody._RHParticleAcceleratorAccentsAnimator.Play("PA_RH_Second Charge");
        if (NewValue == 3)  
            _WM._BPM._NNBody._RHParticleAcceleratorAccentsAnimator.Play("PA_RH_Third Charge");
    }

    #region Recharge

    [Command]// on button down
    public void Recharge(int LR)
    {
        if(LR==0 && IsPAallowedL)
        {
            isRechargingL = true;
            TimeRechargedL = 0f;
            nReflectionsL = 0;
            StartCoroutine(StartRechargingL());
            _WM._BPM._PAM.ParticleAccelerator.Play();
        }
        else if(LR==1 && IsPAallowedL)
        {
            isRechargingR = true;
            TimeRechargedR = 0f;
            nReflectionsR = 0;
            StartCoroutine(StartRechargingR());
            _WM._BPM._PAM.ParticleAccelerator.Play();
        }

    }

    [Server]
    IEnumerator StartRechargingL()
    {
        while (isRechargingL)
        {
            yield return new WaitForEndOfFrame();
            TimeRechargedL += Time.deltaTime;

            nReflectionsL = (int)TimeRechargedL;

            if (nReflectionsL >= MaxReflections)
                isRechargingL = false;
        }

    }
    [Server]
    IEnumerator StartRechargingR()
    {
        while (isRechargingR)
        {
            yield return new WaitForEndOfFrame();
            TimeRechargedR += Time.deltaTime;

            nReflectionsR = (int)TimeRechargedR;

            if (nReflectionsR >= MaxReflections)
                isRechargingR = false;
        }
    }

    #endregion

    [Command]//on buton up
    public void Launch(Vector3 _Dir, Vector3 SpawnPoint, int LR)
    {
        if(LR==0)
        {
            StopCoroutine(StartRechargingL());
            isRechargingL = false;
            if (nReflectionsL < 1)
                return;


            _PA_CollisionL = new PA_Collision[nReflectionsL];
            CLNT_SetrefL(nReflectionsL);

            nReflectionsL = Mathf.Clamp(nReflectionsL, 1, nReflectionsL);
            Vector3 direction = _Dir - SpawnPoint;
            rayL = new Ray(SpawnPoint, direction.normalized);



            for (int i = 0; i < nReflectionsL; i++)
            {
                Debug.Log("going inside the loop");
                if (i == 0)
                {
                    if (Physics.Raycast(rayL.origin, rayL.direction, out hitL, MaxDistance, _LayersToReflect))
                    {
                        AngleOfReflectionL = Vector3.Reflect(rayL.direction, hitL.normal);
                        rayL = new Ray(hitL.point, AngleOfReflectionL);

                        GameObject PArays = Instantiate(CylinderCollider, SpawnPoint, Quaternion.identity);
                        NetworkServer.Spawn(PArays);

                        CLNT_scallingL(PArays, i, hitL.point, hitL.distance);
                        scallingL(PArays, i, hitL.point, hitL.distance);
                    }
                }
                else
                {
                    if (Physics.Raycast(rayL.origin, rayL.direction, out hitL, MaxDistance, _LayersToReflect))
                    {

                        GameObject PArays = Instantiate(CylinderCollider, rayL.origin, Quaternion.identity);
                        NetworkServer.Spawn(PArays);

                        CLNT_scallingL(PArays, i, hitL.point, hitL.distance);
                        scallingL(PArays, i, hitL.point, hitL.distance);

                    }
                }
            }

            IsPAallowedL = false;
            StartCoroutine(PAcooldownL());
            StartCoroutine(DestroyPAL());
            nReflectionsL = 0;
        }
        else if(LR==1)
        {
            StopCoroutine(StartRechargingR());
            isRechargingR = false;
            if (nReflectionsR < 1)
                return;


            _PA_CollisionR = new PA_Collision[nReflectionsR];
            CLNT_SetrefR(nReflectionsR);

            nReflectionsR = Mathf.Clamp(nReflectionsR, 1, nReflectionsR);

            Vector3 direction = _Dir - SpawnPoint;
            rayR = new Ray(SpawnPoint, direction.normalized);



            for (int i = 0; i < nReflectionsR; i++)
            {
                Debug.Log("going inside the loop");
                if (i == 0)
                {
                    if (Physics.Raycast(rayR.origin, rayR.direction, out hitR, MaxDistance, _LayersToReflect))
                    {
                        AngleOfReflectionR = Vector3.Reflect(rayR.direction, hitR.normal);
                        rayR = new Ray(hitR.point, AngleOfReflectionR);

                        GameObject PArays = Instantiate(CylinderCollider, SpawnPoint, Quaternion.identity);
                        NetworkServer.Spawn(PArays);

                        CLNT_scallingR(PArays, i, hitR.point, hitR.distance);
                        scallingR(PArays, i, hitR.point, hitR.distance);
                    }
                }
                else
                {
                    if (Physics.Raycast(rayR.origin, rayR.direction, out hitR, MaxDistance, _LayersToReflect))
                    {

                        GameObject PArays = Instantiate(CylinderCollider, rayR.origin, Quaternion.identity);
                        NetworkServer.Spawn(PArays);

                        CLNT_scallingR(PArays, i, hitR.point, hitR.distance);
                        scallingR(PArays, i, hitR.point, hitR.distance);

                    }
                }
            }

            IsPAallowedR = false;
            StartCoroutine(PAcooldownR());
            StartCoroutine(DestroyPAR());
            nReflectionsR = 0;
        }

    }

    [ClientRpc]
    void CLNT_scallingL(GameObject PArays,int i,Vector3 point, float distance)
    {
        _PA_CollisionL[i] = PArays.GetComponentInChildren<PA_Collision>(); ;
        _PA_CollisionL[i]._PA = this;
        _PA_CollisionL[i].transform.parent.transform.LookAt(point);
        while (_PA_CollisionL[i].transform.parent.transform.localScale.z <= distance / 2)
        {
            _PA_CollisionL[i].transform.parent.transform.localScale += ScallingFactor;
        }
    }

    [ClientRpc]
    void CLNT_scallingR(GameObject PArays, int i, Vector3 point, float distance)
    {
        _PA_CollisionR[i] = PArays.GetComponentInChildren<PA_Collision>(); ;
        _PA_CollisionR[i]._PA = this;
        _PA_CollisionR[i].transform.parent.transform.LookAt(point);
        while (_PA_CollisionR[i].transform.parent.transform.localScale.z <= distance / 2)
        {
            _PA_CollisionR[i].transform.parent.transform.localScale += ScallingFactor;
        }
    }

    void scallingL(GameObject PArays, int i, Vector3 point, float distance)
    {
        _PA_CollisionL[i] = PArays.GetComponentInChildren<PA_Collision>(); ;
        _PA_CollisionL[i]._PA = this;
        _PA_CollisionL[i].transform.parent.transform.LookAt(point);
        while (_PA_CollisionL[i].transform.parent.transform.localScale.z <= distance / 2)
        {
            _PA_CollisionL[i].transform.parent.transform.localScale += ScallingFactor;
        }
    }

    void scallingR(GameObject PArays, int i, Vector3 point, float distance)
    {
        _PA_CollisionR[i] = PArays.GetComponentInChildren<PA_Collision>(); ;
        _PA_CollisionR[i]._PA = this;
        _PA_CollisionR[i].transform.parent.transform.LookAt(point);
        while (_PA_CollisionR[i].transform.parent.transform.localScale.z <= distance / 2)
        {
            _PA_CollisionR[i].transform.parent.transform.localScale += ScallingFactor;
        }
    }

    [ClientRpc]
    void CLNT_SetrefL(int nlist)
    {
        _PA_CollisionL = new PA_Collision[nlist];
    }
    [ClientRpc]
    void CLNT_SetrefR(int nlist)
    {
        _PA_CollisionR = new PA_Collision[nlist];
    }

    IEnumerator DestroyPAL()
    {
        yield return new WaitForSeconds(PA_DestroyTime);
        for(int i =0; i<_PA_CollisionL.Length;i++)
        {
            if(_PA_CollisionL[i]!=null)
                NetworkServer.Destroy(_PA_CollisionL[i].transform.parent.parent.gameObject); 
        }

    }

    IEnumerator DestroyPAR()
    {
        yield return new WaitForSeconds(PA_DestroyTime);
        for (int i = 0; i < _PA_CollisionR.Length; i++)
        {
            if (_PA_CollisionR[i] != null)
                NetworkServer.Destroy(_PA_CollisionR[i].transform.parent.parent.gameObject);
        }

    }

    IEnumerator PAcooldownL()
    {
        DOVirtual.Float(0, 1, PA_cooldownTime, updateLeftUI);
        yield return new WaitForSeconds(PA_cooldownTime);
        IsPAallowedL = true;
    }
    void updateLeftUI(float value)
    {
        _WM._BPM._UIM.L_PA.fillAmount = value;
    }

    IEnumerator PAcooldownR()
    {
        DOVirtual.Float(0, 1, PA_cooldownTime, updateRightUI);
        yield return new WaitForSeconds(PA_cooldownTime);
        IsPAallowedL = true;
    }
    void updateRightUI(float value)
    {
        _WM._BPM._UIM.L_PA.fillAmount = value;
    }


    [Command]
    public void CMD_CollisionCallBack(GameObject other)
    {

    }


}
