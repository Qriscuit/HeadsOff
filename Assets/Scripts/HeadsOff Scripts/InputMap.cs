// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/InputMap.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMap : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMap()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMap"",
    ""maps"": [
        {
            ""name"": ""Cube"",
            ""id"": ""9635361e-7746-4114-bbf2-95c86d2cc97b"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""edbcea7e-be78-4c8c-a64b-4b492479bd4d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""ae4c7598-aaf9-4044-a7f5-f1d53af87f77"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MouseSpin"",
                    ""type"": ""Button"",
                    ""id"": ""65bd0e2d-f72c-44a3-b973-b0bf0c18e28c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""MovementVector"",
                    ""id"": ""a52683ba-1fdc-4d2a-9ba7-6d94ce21bbae"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1890dc38-6a5e-4bae-ad4e-0d2049de1efa"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""402a7c04-17fd-472c-b843-c39fd4a4d224"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""300fb350-0584-4530-a30d-0ed40eb20502"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""7a705f91-de40-47b2-a6c0-666928f8e75e"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""85c6113c-ebc3-4f13-b6e1-9403ee57bde1"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""49efe153-d1ad-4465-b51f-3c4c599ba6b6"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseSpin"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Cube
        m_Cube = asset.FindActionMap("Cube", throwIfNotFound: true);
        m_Cube_Movement = m_Cube.FindAction("Movement", throwIfNotFound: true);
        m_Cube_Jump = m_Cube.FindAction("Jump", throwIfNotFound: true);
        m_Cube_MouseSpin = m_Cube.FindAction("MouseSpin", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Cube
    private readonly InputActionMap m_Cube;
    private ICubeActions m_CubeActionsCallbackInterface;
    private readonly InputAction m_Cube_Movement;
    private readonly InputAction m_Cube_Jump;
    private readonly InputAction m_Cube_MouseSpin;
    public struct CubeActions
    {
        private @InputMap m_Wrapper;
        public CubeActions(@InputMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Cube_Movement;
        public InputAction @Jump => m_Wrapper.m_Cube_Jump;
        public InputAction @MouseSpin => m_Wrapper.m_Cube_MouseSpin;
        public InputActionMap Get() { return m_Wrapper.m_Cube; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CubeActions set) { return set.Get(); }
        public void SetCallbacks(ICubeActions instance)
        {
            if (m_Wrapper.m_CubeActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_CubeActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_CubeActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_CubeActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_CubeActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_CubeActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_CubeActionsCallbackInterface.OnJump;
                @MouseSpin.started -= m_Wrapper.m_CubeActionsCallbackInterface.OnMouseSpin;
                @MouseSpin.performed -= m_Wrapper.m_CubeActionsCallbackInterface.OnMouseSpin;
                @MouseSpin.canceled -= m_Wrapper.m_CubeActionsCallbackInterface.OnMouseSpin;
            }
            m_Wrapper.m_CubeActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @MouseSpin.started += instance.OnMouseSpin;
                @MouseSpin.performed += instance.OnMouseSpin;
                @MouseSpin.canceled += instance.OnMouseSpin;
            }
        }
    }
    public CubeActions @Cube => new CubeActions(this);
    public interface ICubeActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnMouseSpin(InputAction.CallbackContext context);
    }
}
