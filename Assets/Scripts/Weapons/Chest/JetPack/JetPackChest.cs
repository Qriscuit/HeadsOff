using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPackChest : MonoBehaviour
{
    [Header("ClassRefrence")]
    public WeaponManager _WM;
    public MasterInputManager _IM;

    [Header("JetPack")]
    public CharacterController _CC;
    public float JetPackMoveSpeed;
    public float UpSpeed;
    public float DownSpeed;
    public float JetPackFuelTank;
    public float JetPackFuel;
    public float FuelBurnRate;
    public float FuelFillRate;
    public float MaxHeight;
    public float CamSpinMultiplier;// for rotation of player in sky

    public ParticleSystem JetstreamL, JetstreamR;

    bool IsJetPackAllowedToFly;
    bool IsJetPackActive;//master variable which completely turn of flying
    bool MoveDown;
    bool MoveUP;

    [Space]
    public float FlyXSensitivity = 1;
    public float FlyYSensitivity = 1;

    private void Start()
    {
        if(CamSpinMultiplier==0)
        {
            CamSpinMultiplier = _WM._BPM._MH.CamPanMultiplier;
        }
    }

    private void OnEnable()
    {
        _IM._MovementDir += MovementButtonDL;
        _IM._CameraMovement += CameraMovement;
        _IM._JetPackDown += JetPackMoveDown;
        _IM._JumpButton += JetPackMoveUp;
    }

    private void OnDisable()
    {
        _IM._MovementDir -= MovementButtonDL;
        _IM._CameraMovement -= CameraMovement;
        _IM._JetPackDown -= JetPackMoveDown;
        _IM._JumpButton -= JetPackMoveUp;
    }

    Vector2 MovementDir;
    Vector2 CameraDir;
    

    void MovementButtonDL(Vector2 _MovementDir)
    {
        MovementDir = _MovementDir;
    }

    void CameraMovement(Vector2 MouseMovementDir)
    {
        CameraDir = MouseMovementDir;
    }


    public void Launch()
    {
        if(IsJetPackActive==true)
        {
            DeLaunch();
            return;
        }
        _WM._BPM._VFX.CMD_VFX_CjetPackStart();
        //JetstreamL.Play();
        //JetstreamR.Play();
        _WM._BPM._AM._fna.SetTrigger("Jetpack On");
        IsJetPackActive = true;
        IsJetPackAllowedToFly = true;
        _WM._BPM._MH.IsMovementAllowed = false;
    }

    public void JetPackMoveDown(bool Button)
    {
        MoveDown = Button;
    }

    public void JetPackMoveUp(bool Button)
    {
        MoveUP = Button;
    }


    private void Update()
    {
        if(IsJetPackActive && JetPackFuel > 0)
        {
            JetPackFuel -= FuelFillRate * Time.deltaTime;
            Debug.Log("Losing fuel by the second");

            if (IsJetPackAllowedToFly)
            {
                if (MoveUP)
                {
                    if (/*_CC.transform.position.y < MaxHeight && */JetPackFuel > 0)
                    {
                        _CC.Move(transform.up * UpSpeed * Time.deltaTime);   
                    }
                }
                if (MoveDown)
                {
                    _CC.Move(-transform.up * DownSpeed * Time.deltaTime);
                }
                if(JetPackFuel<0)
                {
                    DeLaunch();
                }
                _WM._BPM._UIM.JetPack.fillAmount = JetPackFuel / JetPackFuelTank;
            }
            
            if (MovementDir.y > 0)//W  
            {
                _CC.Move(transform.forward * JetPackMoveSpeed * Time.deltaTime);
            }
            if (MovementDir.x < 0)//A
            {
                _CC.Move(-transform.right * JetPackMoveSpeed * Time.deltaTime);
            }
            if (MovementDir.y < 0)//S
            {
                _CC.Move(-transform.forward * JetPackMoveSpeed * Time.deltaTime);
            }
            if (MovementDir.x > 0)//D
            {
                _CC.Move(transform.right * JetPackMoveSpeed * Time.deltaTime);
            }
        }
        else if(JetPackFuel<JetPackFuelTank)
        {
            JetPackFuel += Time.deltaTime * FuelFillRate;
            _WM._BPM._UIM.JetPack.fillAmount = JetPackFuel / JetPackFuelTank;
        }
    }

    public void DeLaunch()
    {
        if (_WM.isServer)
            return;

        _WM._BPM._VFX.CMD_VFX_CjetPackStop();
        //JetstreamL.Stop();
        //JetstreamR.Stop();
        _WM._BPM._AM._fna.SetTrigger("Jetpack Off");
        IsJetPackAllowedToFly = false;
        IsJetPackActive = false;
        _WM._BPM._MH.IsMovementAllowed = true;
    }

    void PlayerSpin()
    {
        
        float MouseX = CameraDir.x;
        float MouseY = CameraDir.y;
        Vector2 MouseInput = new Vector2(MouseX, MouseY);
        //_CC.transform.eulerAngles += new Vector3(transform.eulerAngles.x * FlyXSensitivity, MouseX * _WM._BPM._MH.CamSpinMultiplier * FlyYSensitivity, transform.eulerAngles.z);
    }
}
