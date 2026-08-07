using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class MasterInputManager : NetworkBehaviour
{
    PlayerInputActionMap input;

    public delegate void LeftArmButton(bool LeftArmButton);
    public event LeftArmButton _LeftArmButton;
    public event LeftArmButton _LeftArmButtonStarted;

    public delegate void ADSButton(bool ADSButton);
    public event ADSButton _ADSButton;
    public event ADSButton _ADSButtonStarted;

    public delegate void RightArmButton(bool RightArmButton);
    public event RightArmButton _RightArmButton;
    public event RightArmButton _RightArmButtonStarted;
    
    public delegate void ChestAbility(bool ChestAbilityButton);
    public event ChestAbility _ChestAbility;
    public event ChestAbility _ChestAbilityStarted;

    public delegate void LegAbility(bool LegAbilityButton);
    public event LegAbility _LegAbility;
    public event LegAbility _LegAbilityStarted;

    public delegate void HeadDeAt_tachment(bool HeadDeAt_tachment);
    public event HeadDeAt_tachment _HeadDeAt_tachment;
    public event HeadDeAt_tachment _HeadDeAt_tachmentStarted;

    public delegate void MovementDir(Vector2 MovementDir);
    public event MovementDir _MovementDir;
    public event MovementDir _MovementDirStarted;

    public delegate void JumpButton(bool JumpButton);
    public event JumpButton _JumpButton;
    public event JumpButton _JumpButtonStarted;

    public delegate void Head_PickUp(bool Head_PickUpButton);
    public event Head_PickUp _Head_PickUp;
    public event Head_PickUp _Head_PickUpStarted;

    public delegate void BodyPartSwap(Vector2 BodyPartSwap);
    public event BodyPartSwap _BodyPartSwap;
    public event BodyPartSwap _BodyPartSwapStarted;

    public delegate void HeadThrow(bool HeadThrowButton);
    public event HeadThrow _HeadThrow;
    public event HeadThrow _HeadThrowStarted;

    public delegate void HandSwap(bool HandSwapButton);
    public event HandSwap _HandSwap;
    public event HandSwap _HandSwapStarted;

    public delegate void PrintingChest(Vector2 PrintingChestButton);
    public event PrintingChest _PrintingChest;
    public event PrintingChest _PrintingChestStarted;

    public delegate void CameraMovement(Vector2 CameraMoveDir);
    public event CameraMovement _CameraMovement;
    //public event CameraMovement _CameraMovementStarted;

    public delegate void JetPackDown(bool JetpackDownButton);
    public event JetPackDown _JetPackDown;
    public event JetPackDown _JetPackDownStarted;

    private void Awake()
    {
        input = new PlayerInputActionMap();
    }
    private void Start()
    {
        //Debug.Log("My isLocalPlayer = " + isLocalPlayer + " and my connectionId is = " + netId);

        //if (!isLocalPlayer)
        //{
        //    Debug.Log("My isLocalPlayer = " + isLocalPlayer + " and my connectionId is = " + netId);
        //    return;
        //}

        //LeftArm
        input.Player.LeftArm.started += ctx =>
        {
            _LeftArmButtonStarted?.Invoke(true);
        };
        input.Player.LeftArm.performed += ctx =>
        {
            _LeftArmButton?.Invoke(true);
        };
        input.Player.LeftArm.canceled += ctx => 
        {
            _LeftArmButton?.Invoke(false);
        };

        //ADS
        input.Player.ADS.started += ctx =>
        {
            _ADSButtonStarted?.Invoke(true);
        };
        input.Player.ADS.performed += ctx =>
        {
            _ADSButton?.Invoke(true);
        };
        input.Player.ADS.canceled += ctx =>
        {
            _ADSButton?.Invoke(false);
        };

        //rightArm
        input.Player.RightArm.started += ctx =>
        {
            _RightArmButtonStarted?.Invoke(true);
        };
        input.Player.RightArm.performed += ctx =>
        {
            _RightArmButton?.Invoke(true);
        };
        input.Player.RightArm.canceled += ctx =>
        {
            _RightArmButton?.Invoke(false);
        };

        //ChestAbility
        input.Player.ChestAbility.started += ctx =>
        {
            _ChestAbilityStarted?.Invoke(true);
        };
        input.Player.ChestAbility.performed += ctx =>
        {
            _ChestAbility?.Invoke(true);
        };
        input.Player.ChestAbility.canceled += ctx =>
        {
            _ChestAbility?.Invoke(false);
        };

        //LegAbility
        input.Player.LegAbility.started += ctx =>
        {
            _LegAbilityStarted?.Invoke(true);
        };
        input.Player.LegAbility.performed += ctx =>
        {
            _LegAbility?.Invoke(true);
        };
        input.Player.LegAbility.canceled += ctx =>
        {
            _LegAbility?.Invoke(false);
        };

        //Head Attach And Detach
        input.Player.HeadDeAt_tachment.started += ctx =>
        {
            _HeadDeAt_tachmentStarted?.Invoke(true);
        };
        input.Player.HeadDeAt_tachment.performed += ctx =>
        {
            _HeadDeAt_tachment?.Invoke(true);
        };
        input.Player.HeadDeAt_tachment.canceled += ctx =>
        {
            _HeadDeAt_tachment?.Invoke(false);
        };

        //Movement
        input.Player.Movement.performed += ctx =>
        {
            _MovementDir?.Invoke(ctx.ReadValue<Vector2>());
        };
        input.Player.Movement.canceled += ctx =>
        {
            _MovementDir?.Invoke(ctx.ReadValue<Vector2>());
        };

        //jump
        input.Player.Jump.started += ctx =>
        {
            _JumpButtonStarted?.Invoke(true);
        };
        input.Player.Jump.performed += ctx =>
        {
            _JumpButton?.Invoke(true);
        };
        input.Player.Jump.canceled += ctx =>
        {
            _JumpButton?.Invoke(false);
        };

        //Head PickUP
        input.Player.Head_PickUp.started += ctx =>
        {
            _Head_PickUpStarted?.Invoke(true);
        };
        input.Player.Head_PickUp.performed += ctx =>
        {
            _Head_PickUp?.Invoke(true);
        };
        input.Player.Head_PickUp.canceled += ctx =>
        {
            _Head_PickUp?.Invoke(false);
        };

        //BodyPartSwap
        input.Player.BodyPartSwap.started += ctx =>
        {
            _BodyPartSwapStarted?.Invoke(ctx.ReadValue<Vector2>());
        };
        input.Player.BodyPartSwap.performed += ctx =>
        {
            _BodyPartSwap?.Invoke(ctx.ReadValue<Vector2>());
        };
        input.Player.BodyPartSwap.canceled += ctx =>
        {
            _BodyPartSwap?.Invoke(ctx.ReadValue<Vector2>());
        };

        //HeadThrow
        input.Player.HeadThrow.started += ctx =>
        {
            _HeadThrowStarted?.Invoke(true);
        };
        input.Player.HeadThrow.performed += ctx =>
        {
            _HeadThrow?.Invoke(true);
        };
        input.Player.HeadThrow.canceled += ctx =>
        {
            _HeadThrowStarted?.Invoke(false);
        };

        //HandSwap
        input.Player.HandSwap.started += ctx =>
        {
            _HandSwapStarted?.Invoke(true);
        };
        input.Player.HandSwap.performed += ctx =>
        {
            _HandSwap?.Invoke(true);
        };
        input.Player.HandSwap.canceled += ctx =>
        {
            _HandSwap?.Invoke(false);
        };

        //PrintingChest
        input.Player.PrintingChest.started += ctx =>
        {
            _PrintingChestStarted?.Invoke(ctx.ReadValue<Vector2>());
        };
        input.Player.PrintingChest.performed += ctx =>
        {
            _PrintingChest?.Invoke(ctx.ReadValue<Vector2>());
        };
        input.Player.PrintingChest.canceled += ctx =>
        {
            _PrintingChest?.Invoke(ctx.ReadValue<Vector2>());
        };

        //camera
        input.Player.Camera.started += ctx =>
        {
            _CameraMovement?.Invoke(ctx.ReadValue<Vector2>());
        };
        input.Player.Camera.performed += ctx =>
        {
            _CameraMovement?.Invoke(ctx.ReadValue<Vector2>());
        };
        input.Player.Camera.canceled += ctx =>
        {
            _CameraMovement?.Invoke(ctx.ReadValue<Vector2>()*Time.deltaTime);
        };

        //jetpack
        input.Player.Jetpack.started += ctx =>
        {
            _JetPackDownStarted?.Invoke(true);
        };
        input.Player.Jetpack.performed += ctx =>
        {
            _JetPackDown?.Invoke(true);
        };
        input.Player.Jetpack.canceled += ctx =>
        {
            _JetPackDown?.Invoke(false);
        };
    }

    private void OnEnable()
    {
        if (isServer) return;
        input.Player.Enable();
    }

    private void OnDisable()
    {
        if (isServer) return;
        input.Player.Disable();
    }

}
