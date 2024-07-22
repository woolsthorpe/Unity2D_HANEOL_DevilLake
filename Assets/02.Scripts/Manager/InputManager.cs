using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool JumpPressed;
    public static bool AttackPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _attackAction;

    private void Awake()
    {
        print(TryGetComponent(out PlayerInput));

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _attackAction = PlayerInput.actions["Attack"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        JumpPressed = _jumpAction.IsPressed();

        AttackPressed = _attackAction.IsPressed();
    }
}
