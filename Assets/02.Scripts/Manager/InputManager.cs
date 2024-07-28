using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static GameControls InputAsset;

    public static Vector2 Movement;
    public static bool JumpPressed;
    public static bool AttackPressed;
    public static bool InteractPressed;
    
    // 델리게이트 선언
    public delegate void KeyAction();
    
    // 입력 이벤트 선언
    public static event KeyAction OnJump;
    public static event KeyAction OnDash;

    private void Awake()
    {
        InputAsset = new GameControls();
    }

    private void Update()
    {
        Movement = InputAsset.Player.Move.ReadValue<Vector2>();
        JumpPressed = InputAsset.Player.Jump.IsPressed();
        AttackPressed = InputAsset.Player.Attack.IsPressed();
        InteractPressed = InputAsset.Player.Interact.IsPressed();

        InputAsset.Player.Jump.performed += ctx => Jump();
        InputAsset.Player.Dash.performed += ctx => Dash();
    }
    
    private void OnEnable()
    {
        InputAsset.Enable();
    }

    private void OnDisable()
    {
        InputAsset.Disable();
    }

    private void Jump()
    {
        OnJump?.Invoke();
    }

    private void Dash()
    {
        OnDash?.Invoke();
    }
}
