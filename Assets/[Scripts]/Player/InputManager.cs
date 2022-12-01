using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private PlayerInput playerInput;
    private PlayerInputActions playerInputActions;
    private PlayerInputActions.OnGroundActions onGround;
    private PlayerInputActions.OnMenuActions onMenu;
    private PlayerController playerController;
    private PlayerLook playerLook;
    private PlayerGunController playerGunController;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerGunController = player.GetComponent<PlayerGunController>();
        playerInput = player.GetComponent<PlayerInput>();
        playerController = player.GetComponent<PlayerController>();
        playerLook = player.GetComponent<PlayerLook>();
        playerInputActions = new PlayerInputActions();
        onGround = playerInputActions.OnGround;
        onMenu = playerInputActions.OnMenu;

        //onGround.Jump.performed += ctx => playerController.Jump();
        onGround.SwitchWeapon.performed += ctx => playerGunController.SwitchGun();
        playerInput.SwitchCurrentActionMap("OnGround");
        //playerInput.currentActionMap = onGround.Get();
    }

    public void SwitchActionMap()
    {
        if (playerInput.currentActionMap == onGround.Get())
        {
            playerInput.SwitchCurrentActionMap("OnMenu");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("OnGround");
        }

        if (onGround.enabled) {
            onGround.Disable();
            onMenu.Enable();
        }else {
            onGround.Enable();
            onMenu.Disable();
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        playerController.Move(onGround.Movement.ReadValue<Vector2>());
        playerController.isSprinting = onGround.Sprint.IsPressed();
        playerController.isCrouching = onGround.Crouch.IsPressed();
        if (onGround.Jump.IsPressed())
            playerController.Jump();
    }

    void Update()
    {
        if (!playerController.isSprinting)
        {
            playerGunController.currentGun.CheckInput(onGround.Fire);

        }
        playerLook.Look(onGround.Look.ReadValue<Vector2>());
    }

    void LateUpdate()
    {
        
    }

    private void OnEnable()
    {
        onGround.Enable();
        
    }
    private void OnDisable()
    {
        onGround.Disable();
        
    }
}
