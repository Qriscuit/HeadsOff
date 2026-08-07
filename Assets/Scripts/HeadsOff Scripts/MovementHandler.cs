using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class MovementHandler : NetworkBehaviour
{ 
    BasePlayerManager _BPM;
    [Header("Camera")]
    public float RotationY = 0;
    float LastRotationY = 0;
    public float RotationSensitivity;
    public bool ShouldRotate = true;
    [Space]
    public Vector2 _MovementInput;
    public Vector3 MovementVector;
    public Vector3 AppliedMovement;

    [SyncVar] public bool IsMovementAllowed = true;
    [SyncVar] public bool IsPlayerSpinAllowed = true;

    public bool IsMoveButtonPressed;
    public float CamPanMultiplier = 1f;
    public float CamTiltMultiplier = 1f;

    public float SpeedOnGround = 10f;
    public float SpeedInAir = 10f;
    [SyncVar]public float SlowedSpeed = 1f;

    float GroundedGravity = -0.05f;

    public Dictionary<int, float> InitialJumpVelocities = new Dictionary<int, float>();
    public Dictionary<int, float> InitialGravities = new Dictionary<int, float>();

    [Space]

    public float CurrentAmountOfFuel = 15;
    public float HeadFuelDepletionSpeed = 1f;
    public float HeadFuelRegenSpeed = 10f;
    public float HeadThrustSpeed = 10f;
    public float HeadGravity = -10f;
    public bool AllowedToThrust = false;

    [Space] 

    public bool IsJumpPressed = false;
    public bool IsJumping = false;
    //public bool IsJumping
    //{
    //    get { return _IsJumping; }
    //    set
    //    {
    //        if (value == false)
    //            _BPM._AM.SetLanded(true);
    //        _IsJumping = value;
    //    }
    //}

    public List<Vector2> BodyJumpValues = new List<Vector2>();
    public int JumpCount;
    public int HeadJumpCount;

    public CharacterController _CC;

    public delegate void IfIsGroundedTrue();
    public event IfIsGroundedTrue _IfIsGroundedFalse;

    

    #region inputs

    public Vector3 MovementDir;
    public Vector2 CameraDir;

    void MovementButton(Vector2 _MovementDir)
    {
        MovementDir = _MovementDir;
    }

    void JumpButton(bool JumpButtonPressState)
    {
        IsJumpPressed = JumpButtonPressState;
        if (AllowedToThrust == true && JumpButtonPressState == false)
        {
            _BPM._VFX.CMD_VFX_HeadFlightStop();
            AllowedToThrust = false;
        }

        if (_BPM.BodyInPossession != null)
        {
            if (IsJumpPressed && IsMovementAllowed)
            {
                Jump();
            }
        }
        else
        {
            if (IsJumpPressed && IsMovementAllowed)
            {
                HeadJump();
            }
        }
    }

    void CameraMovement(Vector2 MouseMovementDir)
    {
        CameraDir = MouseMovementDir;
    }

    private void OnEnable()
    {
        _BPM._IM._MovementDir += MovementButton;
        _BPM._IM._JumpButton += JumpButton;
        _BPM._IM._CameraMovement += CameraMovement;
    }

    private void OnDisable()
    {
        _BPM._IM._MovementDir -= MovementButton;
        _BPM._IM._JumpButton -= JumpButton;
        _BPM._IM._CameraMovement -= CameraMovement;
    }

    #endregion

    void Awake()
    {
        _BPM = transform.GetComponent<BasePlayerManager>();

        _CC = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;

        RotationY = transform.eulerAngles.y;
        LastRotationY = RotationY;

        SetUpJumpVariables();
    }

    private void SetUpJumpVariables()
    {
        float TimeToApex =  BodyJumpValues[0].x/ 2;
        float Gravity = (-2 * BodyJumpValues[0].y) / Mathf.Pow(TimeToApex, 2);
        float FirstJumpVelocity = (2 * BodyJumpValues[0].y) / TimeToApex;
        float TimeToApexTwo = BodyJumpValues[1].x / 2;
        float SecondGravity = (-2 * (BodyJumpValues[1].y * 2) / Mathf.Pow((TimeToApexTwo * 1.25f), 2));
        float SecondJumpVelocity = (2 * (BodyJumpValues[1].y * 2) / (TimeToApexTwo * 1.25f));
        
        InitialJumpVelocities.Add(1, FirstJumpVelocity);
        InitialJumpVelocities.Add(2, SecondJumpVelocity);

        InitialGravities.Add(0, Gravity);
        InitialGravities.Add(1, Gravity);
        InitialGravities.Add(2, SecondGravity);
    }
    

    public Vector3 InputFromPlayer = Vector3.zero;
    public Vector3 ValuesForAnimator = Vector3.zero;


    private void Update()
    {
        if (Input.GetKey(KeyCode.Alpha5))
        {
            RotationSensitivity -= Time.deltaTime * 2;
        }

        if (Input.GetKey(KeyCode.Alpha6))
        {
            RotationSensitivity += Time.deltaTime * 2;
        }
    }


    void FixedUpdate()
    {
        if (IsMovementAllowed)
        {
            InputFromPlayer = Vector3.zero;
            ValuesForAnimator = Vector3.zero;
            IsMoveButtonPressed = false;

            if (MovementDir.y > 0)//W
            {
                InputFromPlayer += transform.forward;
                ValuesForAnimator += new Vector3(0, 0, 1);
                IsMoveButtonPressed = true;
            }
            if (MovementDir.x < 0)//A
            {
                InputFromPlayer -= transform.right;
                ValuesForAnimator += new Vector3(-1, 0, 0);
                IsMoveButtonPressed = true;
            }
            if (MovementDir.y < 0)//S
            {
                InputFromPlayer -= transform.forward;
                ValuesForAnimator += new Vector3(0, 0, -1);
                IsMoveButtonPressed = true;
            }
            if (MovementDir.x > 0)//D
            {
                InputFromPlayer += transform.right;
                ValuesForAnimator += new Vector3(1, 0, 0);
                IsMoveButtonPressed = true;
            }

            if (isServer) MovementVector = Vector3.zero;
            if (!isLocalPlayer) MovementVector = Vector3.zero;

            if(isValOverRiding)
            {
                IsMoveButtonPressed = true;
                InputFromPlayer += transform.forward*OverRideSpeed;
            }

            if (isLocalPlayer)
            {
                if (_BPM.BodyInPossession != null)
                    BodyMovement();
                else
                    HeadMovement();
            }
        }
    }

    [Header("OverRide Movement")]
     bool isValOverRiding;
     Vector3 OverRideDirection;
    float OverRideSpeed = 1f;
    
    public void overRideMoveVal(Vector3 OverRideDirection, float OverRideSpeed)
    {
        this.OverRideSpeed = OverRideSpeed;
        this.OverRideDirection = OverRideDirection;
        isValOverRiding = true;
        AppliedMovement.y = 1.2f;
        MovementVector.y = 1.2f;
        IsJumping = true;
    }

    public void stopOverRideMove()
    {
        OverRideSpeed = 1f;
        isValOverRiding = false;
    }
    
    void BodyMovement()
    {
        if(IsMoveButtonPressed)
        MovementVector = new Vector3((InputFromPlayer).x, MovementVector.y, (InputFromPlayer).z);
        else MovementVector = new Vector3(0, MovementVector.y, 0);

       // PlayerSpin();

        AppliedMovement.x = MovementVector.x;
        AppliedMovement.z = MovementVector.z;
        
        if (IsJumping)
        {
            //Debug.Log(AppliedMovement * SpeedInAir * Time.deltaTime * SlowedSpeed);
            _CC.Move(AppliedMovement * SpeedInAir * Time.deltaTime * SlowedSpeed);
        }
        else
        {
           // Debug.Log(AppliedMovement * SpeedOnGround * Time.deltaTime * SlowedSpeed);
            _CC.Move(AppliedMovement * SpeedOnGround * Time.deltaTime * SlowedSpeed);
        }
        

        if (AppliedMovement.y < 0 && IsJumping) _BPM._AM.SetFallingTrue();
        if (!IsJumping) _BPM._AM.SetLanded(true);
        if (!IsJumping) _BPM._AM.CMD_setlANDED(true);
        HandleGravity();
        HandleJumpVariables();
    }

    void HeadMovement()
    {
        if (IsMoveButtonPressed) MovementVector = new Vector3((InputFromPlayer).x, MovementVector.y, (InputFromPlayer).z);
        else MovementVector = new Vector3(0, MovementVector.y, 0);

       // PlayerSpin();

        AppliedMovement.x = MovementVector.x;
        AppliedMovement.z = MovementVector.z;

        _CC.Move(AppliedMovement * SpeedInAir * Time.deltaTime * SlowedSpeed );

        HandleGravityForHead();

        if (AllowedToThrust == true)
        {
            TurnOnThrusters();
        }
        else
        {
            CurrentAmountOfFuel += HeadFuelRegenSpeed * Time.deltaTime;
           
            CurrentAmountOfFuel = Mathf.Clamp(CurrentAmountOfFuel, 0f, 20f);
        }
           
        HandleHeadJumpVariables();
    }

    void HandleGravity()
    {
        if (_CC.isGrounded)
        {   
            AppliedMovement.y = GroundedGravity;
        }
        else
        {
            float PreviousYVel = MovementVector.y;
            MovementVector.y = PreviousYVel + (InitialGravities[JumpCount] * Time.deltaTime);
            AppliedMovement.y = (MovementVector.y + PreviousYVel) * 0.5f;
        }
    }

    void HandleGravityForHead()
    {
        if (_CC.isGrounded)
        {
            AppliedMovement.y = GroundedGravity;
        }
        else
        {
            float PreviousYVel = MovementVector.y;
            MovementVector.y = PreviousYVel + (HeadGravity * Time.deltaTime);
            AppliedMovement.y = (MovementVector.y + PreviousYVel) * 0.5f;
        }
    }

    void Jump()
    {
        if (JumpCount < 2)
        {
            if (!_BPM._NNBody._Animator.GetBool("Jumping")) _BPM._AM.SetJumpingTrue();
            else _BPM._AM._fna.SetTrigger("DoubleJump");

            _BPM._AM.SetLanded(false);

            IsJumping = true;
            JumpCount++;
            MovementVector.y = InitialJumpVelocities[JumpCount];
            AppliedMovement.y = InitialJumpVelocities[JumpCount];
            _BPM._PAM.Jump.Play();
        }
    }

    void HeadJump()
    {
        if (HeadJumpCount < 1)
        {
            IsJumping = true;
            HeadJumpCount++;
            MovementVector.y = InitialJumpVelocities[2];
            AppliedMovement.y = InitialJumpVelocities[2];

            //Debug.Log("InitialJumpVelocities[2] : " + InitialJumpVelocities[2]);
            //Debug.Log("CorrectedMovementVector : " + CorrectedMovementVector);
            //Debug.Log("AppliedMovement.y : " + AppliedMovement.y);
        }
        else
        {
            AllowedToThrust = true;
        }
    }

    private void TurnOnThrusters()
    {
        if (IsJumpPressed && CurrentAmountOfFuel > 0)
        {
            _BPM._VFX.CMD_VFX_HeadFlight();
            MovementVector.y += HeadThrustSpeed * Time.deltaTime;
            AppliedMovement.y += HeadThrustSpeed * Time.deltaTime;

            MovementVector.y = Mathf.Clamp(MovementVector.y, -2f, 2f);
            AppliedMovement.y = Mathf.Clamp(AppliedMovement.y, -2f, 1f);

            CurrentAmountOfFuel -= HeadFuelDepletionSpeed * Time.deltaTime;
        }
        else
        {
            _BPM._VFX.CMD_VFX_HeadFlightStop();
        }
    }

    void HandleJumpVariables()
    {
        if (!IsJumpPressed && IsJumping && _CC.isGrounded)
        {
            IsJumping = false;
            JumpCount = 0;
        }
    }

    void HandleHeadJumpVariables()
    {
        if(!IsJumpPressed&&IsJumping&&_CC.isGrounded)
        {
            IsJumping = false;
            HeadJumpCount = 0;
        }
    }

    private void LateUpdate()
    {
        if (IsPlayerSpinAllowed) PlayerSpin();
    }

    void OnJumpUpdate(InputAction.CallbackContext ctx)
    {
        IsJumpPressed = ctx.ReadValueAsButton();
        Debug.Log("Jump => " + IsJumpPressed);
    }

    void OnMovementUpdate(InputAction.CallbackContext ctx)
    {
        _MovementInput = ctx.ReadValue<Vector2>();
    }

    float camXAngle;
    float CorrectedCamXAngle;
    void PlayerSpin()
    {
        if (isServer || !isLocalPlayer)
            return;

        if (!ShouldRotate) 
            return;
 
        RotationY += CameraDir.x * Time.deltaTime * RotationSensitivity;

        //if (RotationY > 360)
        //    RotationY = 0;

        //if (RotationY < -360)
        //    RotationY = 0;


        Debug.Log("Rotation Y = " + RotationY);
        RotationY = Mathf.Lerp(LastRotationY, RotationY, 0.9f);

        LastRotationY = RotationY;
        
        Quaternion ROT = Quaternion.Euler(transform.eulerAngles.x, RotationY, transform.eulerAngles.z);

        transform.rotation = Quaternion.Lerp(transform.rotation, ROT, 0.95f);

        /*
        if (!invertY)
            MouseY = -MouseY;

        Transform cameralookat = _BPM._BodyInPossession == null ? _BPM._CMHead.LookAt : _BPM.ADSHeld ? _BPM._CMBodyADS.LookAt : _BPM._CMBody.LookAt;
        //cameralookat.eulerAngles += new Vector3(MouseY * CamSpinMultiplier, 0, 0);

        camXAngle += MouseY * CamTiltMultiplier;
        camXAngle = Mathf.Clamp(camXAngle, camAngleBottomMax, camAngleTopMax);
        CorrectedCamXAngle = (camXAngle < 0) ? 360 + camXAngle : camXAngle;
        

        cameralookat.eulerAngles = new Vector3(CorrectedCamXAngle, cameralookat.eulerAngles.y, cameralookat.eulerAngles.z);

        //if (cameralookat.eulerAngles.x>180 && cameralookat.eulerAngles.x < 345)
        //{
        //    cameralookat.eulerAngles = new Vector3(345f, cameralookat.eulerAngles.y, cameralookat.eulerAngles.z);
        //}

        //if(cameralookat.eulerAngles.x<180 && cameralookat.eulerAngles.x > 45)
        //{
        //    cameralookat.eulerAngles = new Vector3(45, cameralookat.eulerAngles.y, cameralookat.eulerAngles.z);
        //}
        //cameralookat.eulerAngles = new Vector3(Mathf.Clamp(cameralookat.eulerAngles.x,-40,40), cameralookat.eulerAngles.y, cameralookat.eulerAngles.z);

        transform.eulerAngles += new Vector3(transform.eulerAngles.x, MouseX * CamPanMultiplier * Time.deltaTime, transform.eulerAngles.z); 
        
        //Debug.Log(Camera.transform.eulerAngles.y);

        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, Camera.transform.eulerAngles.y, transform.eulerAngles.z);
        //transform.localRotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.localEulerAngles.y, Camera.transform.localEulerAngles.y, ref horiangularVelocity, smoothTime), 0);
        //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0), CamSpinMultiplier * Time.deltaTime);
        */
    }
}
