using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HeadsOffGlobals;

public class FlameThrower : MonoBehaviour
{
    [Header("Class Refrences")]
    public GameObject CollisionParticle;
    public WeaponManager _WM;

    public int LR;
    public ParticleCollision _PCL;
    public ParticleCollision _PCR;

    [Header("Variables")]
    public float FuelTank;

    [SerializeField] float _CurrentFuelTankL;
    [SerializeField] float _CurrentFuelTankR;

    public float FuelBurnRate;
    public float FuelRefillRate;

    public bool isFiringL;
    public bool isFiringR;
    private void Awake()
    {
        _CurrentFuelTankL = FuelTank;
        _CurrentFuelTankR = FuelTank;
    }
    public void Start()
    {

        isFiringL=false;
        isFiringR=false;
    }
    public void Launch(Vector3 ShotPos, Vector3 CameraForward, int _LR)
    {

        
        LR = _LR;

        if(LR==0)
        {
            if (_PCL != null)
            {
                _WM.DestroyFlame(LR);
                //_PCL = Instantiate(CollisionParticle, transform.TransformPoint(ShotPos), Quaternion.identity).GetComponent<ParticleCollision>();
                _WM.CMD_SpawnFire(ShotPos,_LR);
            }
            else
            {
                //_PCL = Instantiate(CollisionParticle, transform.TransformPoint(ShotPos), Quaternion.identity).GetComponent<ParticleCollision>();
                _WM.CMD_SpawnFire(ShotPos,_LR);
            }
        }

        if (LR == 1)
        {
            if (_PCR != null)
            {
                _WM.DestroyFlame(LR);
                //_PCR = Instantiate(CollisionParticle, transform.TransformPoint(ShotPos), Quaternion.identity).GetComponent<ParticleCollision>();
                _WM.CMD_SpawnFire(ShotPos, _LR);
                //_PCR.transform.SetParent(this.gameObject.transform);
                //_PCR._FlameThrower = this;
            }
            else
            {
                //_PCR = Instantiate(CollisionParticle, transform.TransformPoint(ShotPos), Quaternion.identity).GetComponent<ParticleCollision>();
                _WM.CMD_SpawnFire(ShotPos, _LR);
                //_PCR.transform.SetParent(this.gameObject.transform);
                //_PCR._FlameThrower = this;
            }

          //  if (_CurrentFuelTankR > 0)
          //  {
          //      isFiringR = true;
          //      _PCR.TurnOnFlameRPC();
          //  }
          //
        }
        
    }

    
    public void DeLaunchL()
    {
        isFiringL = false;
        Debug.Log("DelaunchL");
        if (!_WM.isServer && _PCL !=null) _PCL.CMD_TurnOffFlame();
    }

    public void DeLaunchR()
    {
        isFiringR = false;
        Debug.Log("DelaunchR");
        if(!_WM.isServer && _PCR != null) _PCR.CMD_TurnOffFlame();
    }


    private void Update()
    {
        if (isFiringL)
        {
            _CurrentFuelTankL -= FuelBurnRate * Time.deltaTime;
            _WM._BPM._UIM.L_FlameThrower.fillAmount = _CurrentFuelTankL / FuelTank;
            if (_CurrentFuelTankL < 0)
            {
                DeLaunchL();
            }
        }
        else if (_CurrentFuelTankL < FuelTank)
        {
            _CurrentFuelTankL += FuelRefillRate * Time.deltaTime;
            _WM._BPM._UIM.L_FlameThrower.fillAmount = _CurrentFuelTankL / FuelTank;
        }


        if (isFiringR)
        {
            _CurrentFuelTankR -= FuelBurnRate * Time.deltaTime;
            _WM._BPM._UIM.R_FlameThrower.fillAmount = _CurrentFuelTankR / FuelTank;
            if (_CurrentFuelTankR < 0)
            {
                DeLaunchR();
            }
        }
        else
            if (_CurrentFuelTankR < FuelTank)
        {
            
            _CurrentFuelTankR += FuelRefillRate * Time.deltaTime;
            _WM._BPM._UIM.R_FlameThrower.fillAmount = _CurrentFuelTankR / FuelTank;
        }
    }

    public void LaunchPCL()
    {
        
        _PCL.transform.SetParent(this.gameObject.transform);
        _PCL._FlameThrower = this;
    

            if (_CurrentFuelTankL > 0)
            {
                isFiringL = true;
                _PCL.TurnOnFlame();
            }
    }

    public void LaunchPCR()
    {
        _PCR.transform.SetParent(this.gameObject.transform);
        _PCR._FlameThrower = this;


        if (_CurrentFuelTankR > 0)
        {
            isFiringR = true;
            _PCR.TurnOnFlame();
        }
    }
}
