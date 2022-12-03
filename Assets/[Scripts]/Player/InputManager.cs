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
    private PlayerInputActions.OnDroneActions onDrone;
    private PlayerController playerController;
    private PlayerLook playerLook;
    private DroneLook droneLook;
    private DroneController droneController;
    private PlayerGunController playerGunController;
    private GameObject scopeOverlay;
    private GameObject pistolCrosshair;
    private GameObject sniperModel;

    private Camera playerCam, droneCam;
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

        onDrone = playerInputActions.OnDrone;
        GameObject drone = GameObject.FindGameObjectWithTag("Drone");
        droneLook = drone.GetComponent<DroneLook>();
        droneController = drone.GetComponent<DroneController>();

        scopeOverlay = GameObject.FindGameObjectWithTag("ScopeOverlay");
        sniperModel = GameObject.FindGameObjectWithTag("SniperModel");
        scopeOverlay.SetActive(false);

        pistolCrosshair = GameObject.FindGameObjectWithTag("PistolCrosshair");

        //onGround.Jump.performed += ctx => playerController.Jump();
        onGround.SwitchWeapon.performed += ctx => playerGunController.SwitchGun();
        onGround.ChangeView.performed += ctx => GroundChangeViewPerformed();
        onDrone.ChangeView.performed += ctx => DroneChangeViewPerformed();

        playerInput.SwitchCurrentActionMap("OnGround");
        //playerInput.currentActionMap = onGround.Get();

        playerCam = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
        droneCam = GameObject.FindGameObjectWithTag("DroneCamera").GetComponent<Camera>();

        playerCam.enabled = true;
        droneCam.enabled = false;
    }

    private void DroneChangeViewPerformed()
    {
        playerCam.enabled = true;
        droneCam.enabled = false;
        onGround.Enable();
        onDrone.Disable();
        playerInput.SwitchCurrentActionMap("OnGround");
    }

    private void GroundChangeViewPerformed()
    {
        playerCam.enabled = false;
        droneCam.enabled = true;
        onGround.Disable();
        onDrone.Enable();
        playerInput.SwitchCurrentActionMap("OnDrone");
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
        droneController.Move(onDrone.Movement.ReadValue<Vector2>());
        droneController.MoveUpDown(onDrone.Elevation.ReadValue<Vector2>());
        playerController.isSprinting = onGround.Sprint.IsPressed();
        playerController.isCrouching = onGround.Crouch.IsPressed();
        if (onGround.Jump.IsPressed())
            playerController.Jump();

        if (playerGunController.currentGun.name == "Sniper" && onGround.Zoom.IsPressed())
        {
            scopeOverlay.SetActive(true);
            sniperModel.SetActive(false);
            playerCam.fieldOfView = 20f;
        }

        else
        {
            scopeOverlay.SetActive(false);
            sniperModel.SetActive(true);
            playerCam.fieldOfView = 50f;
        }
        if (playerGunController.currentGun.name == "Sniper")
        {
            pistolCrosshair.SetActive(false);
        }
        else
        {
            pistolCrosshair.SetActive(true);
        }
    }

    void Update()
    {
        if (!playerController.isSprinting)
        {
            playerGunController.currentGun.CheckInput(onGround.Fire);

        }
        playerLook.Look(onGround.Look.ReadValue<Vector2>());
        droneLook.Look(onDrone.Look.ReadValue<Vector2>());
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
