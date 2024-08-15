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
    public static event KeyAction OnAttack;
    public static event KeyAction OnWeaponSkill;
    public static event KeyAction OnInteract;
    public static event KeyAction OnChangeWeapon;

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
        InputAsset.Player.Interact.performed += ctx => Interact();
        InputAsset.Player.Attack.performed += ctx => Attack();
        InputAsset.Player.ChangeWeapon.performed += ctx => ChangeWeapon();
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

    private void Attack()
    {
        OnAttack?.Invoke();
    }
    
    private void WeaponSkill()
    {
        OnWeaponSkill?.Invoke();
    }
    
    private void Interact()
    {
        OnInteract?.Invoke();
    }
    
    private void ChangeWeapon()
    {
        OnChangeWeapon?.Invoke();
    }
}
