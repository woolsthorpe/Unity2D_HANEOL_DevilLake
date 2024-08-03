using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Body : MonoBehaviour, IInteractable, IDamageable, IHealable
{
    public BodyData bodyData;
    public BodyStateMachine StateMachine { get; private set; } = new BodyStateMachine();
    public Rigidbody2D rb;
    public Animator animator;
    public GameObject attackPrefab;
    public Transform attackPosition;    // 공격 이펙트 생성 위치

    [Header("Stats")] // 육체 기생할 때 플레이어 스탯과 육체 증가량 스탯을 계산하여 나온 현재 육체의 스탯
    public float moveSpeed;
    public float jumpPower;
    public float dashSpeed;

    [HideInInspector] public Transform groundCheckTransform;
    [HideInInspector] public bool isGround;
    [HideInInspector] public bool facingRight;
    [HideInInspector] public Player parasiticPlayer;

    public float currentHealth;    // 현재 체력
    public Weapon currentWeapon;   // 현재 혈기


    private void Start()
    {
        StateMachine.Initialize(this);
    }

    private void OnEnable()
    {
        // 입력 이벤트 구독
        InputManager.OnJump += OnJump;
        InputManager.OnDash += OnDash;
        InputManager.OnAttack += OnAttack;
        // 혈기 일반공격 이벤트 로직 작성 후 구독하기.
        InputManager.OnWeaponSkill += OnWeaponSkill;
    }

    private void OnDisable()
    {
        InputManager.OnJump -= OnJump;
        InputManager.OnDash -= OnDash;
        InputManager.OnAttack -= OnAttack;
        // 혈기 일반공격 이벤트 로직 작성 후 구독 해제하기.
        InputManager.OnWeaponSkill -= OnWeaponSkill;
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
        StartParasitic(player);
    }

    public void StartParasitic(Player player)
    {
        // 육체 기생 로직
        parasiticPlayer = player;
        player.currentHostTransform = transform;    // 위치 정보 참조
        // 육체 능력치 적용 로직 작성
        player.StateMachine.TransitionToState(player.StateMachine.ParasiticState, player);
        StateMachine.TransitionToState(StateMachine.AwakeState, this);
    }

    public void EndParasitic(Player player)
    {
        parasiticPlayer = null;
        player.currentHostTransform = null;    // 위치 정보 해제
        // 육체 능력치 적용 해제 로직 작성
        player.StateMachine.TransitionToState(player.StateMachine.IdleState, player);
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
    
    public void OnAttack()
    {
        // 비활성화, 어웨이크, 사망, 공격, 히트, 착지, 혈기 공격, 혈기 스킬 상태가 아닐 때 상태 전환 
        // == 무브, 점프, 폴, 아이들 일때만 전환 가능 
        // ( 이외 대쉬 점프 등도 if문 반전 사용하기. )
        if (StateMachine.CurrentState == StateMachine.IdleState ||
            StateMachine.CurrentState == StateMachine.MoveState ||
            StateMachine.CurrentState == StateMachine.JumpState ||
            StateMachine.CurrentState == StateMachine.FallState)
        {
            StateMachine.TransitionToState(StateMachine.AttackState, this);
        }
    }

    public void OnWeaponAttack()
    {
        
    }

    public void OnWeaponSkill()
    {
        
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

    public GameObject InstantiateAttack(GameObject prefab)
    {
        return Instantiate(prefab);
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

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            // 사망
            StateMachine.TransitionToState(StateMachine.DieState, this);
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > bodyData.maxBlood) // 최대체력 보다 체력이 많으면
        {
            currentHealth = bodyData.maxBlood;
        }
    }
}
