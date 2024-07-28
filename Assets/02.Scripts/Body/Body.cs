using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Body : MonoBehaviour, IInteractable
{
    public BodyData bodyData;
    public BodyStateMachine StateMachine { get; private set; } = new BodyStateMachine();
    public Rigidbody2D rb;
    public Animator animator;

    public Transform groundCheckTransform;
    public bool isGround;

    [Header("Stats")] // 육체 기생할 때 플레이어 스탯과 육체 증가량 스탯을 계산하여 나온 현재 육체의 스탯
    public float moveSpeed;
    public float jumpPower;
    public float dashSpeed;

    [FormerlySerializedAs("_facingRight")] public bool facingRight;

    private void Start()
    {
        StateMachine.Initialize(this);
    }

    private void OnEnable()
    {
        // 입력 이벤트 구독
        InputManager.OnJump += OnJump;
        InputManager.OnDash += OnDash;
    }

    private void OnDisable()
    {
        InputManager.OnJump -= OnJump;
        InputManager.OnDash -= OnDash;
    }

    private void Update()
    {
        StateMachine.UpdateState(this);
        Debug.DrawRay(groundCheckTransform.position, Vector3.down * 0.05f, Color.magenta);

        if (!isGround && 
            StateMachine.CurrentState != StateMachine.DisableState &&
            StateMachine.CurrentState != StateMachine.DashState &&
            StateMachine.CurrentState != StateMachine.LandingState)
        {
            if (rb.velocity.y < 0)
            {
                StateMachine.TransitionToState(StateMachine.FallState, this);
            }
        }
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdateState(this);

        IsGrounded(); // 바닥 체크 
    }
    
    public void Interact(Player player)
    {
        // 육체 기생 로직 작성
        player.currentHostTransform = transform;    // 위치 정보 참조
        // 육체 능력치 적용 로직 작성
        player.StateMachine.TransitionToState(player.StateMachine.ParasiticState, player);
        StateMachine.TransitionToState(StateMachine.AwakeState, this);
    }

    public void OnJump()
    {
        if (isGround && 
            StateMachine.CurrentState != StateMachine.DisableState && 
            StateMachine.CurrentState != StateMachine.AwakeState &&
            StateMachine.CurrentState != StateMachine.LandingState)
        {
            StateMachine.TransitionToState(StateMachine.JumpState, this);
        }
    }

    public void OnDash()
    {
        if (StateMachine.CurrentState != StateMachine.DisableState &&
            StateMachine.CurrentState != StateMachine.AwakeState)
        {
            StateMachine.TransitionToState(StateMachine.DashState, this);
        }
    }

    public void IsGrounded()
    {
        if (rb.velocity.y > 0)
        {
            isGround = false;
        }
        else
        {
            LayerMask layerMask = LayerMask.GetMask("Ground");
            isGround = Physics2D.Raycast(groundCheckTransform.position, Vector3.down, 0.05f, layerMask);
        }
    }
    
    public void TurnCheck(Vector2 moveInput)
    {
        if (facingRight && moveInput.x < 0f)
        {
            Turn();
        }
        
        else if (!facingRight && moveInput.x > 0f)
        {
            Turn();
        }
    }

    private void Turn()
    {
        facingRight = !facingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1f;
        transform.localScale = newScale;
    }
}
