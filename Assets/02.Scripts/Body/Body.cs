using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Body : MonoBehaviour, IInteractable, IDamageable, IHealable
{
    public BodyStateMachine StateMachine { get; private set; } = new BodyStateMachine();
    
    [Header("Body Info")]
    public BodyData bodyData;
    
    [Header("Attack Settings")]
    public GameObject attackPrefab;
    public Transform attackPosition;    // 공격 이펙트 생성 위치
    
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    
    [HideInInspector] public Transform groundCheckTransform;
    [HideInInspector] public bool isGround;
    [HideInInspector] public bool facingRight;
    [HideInInspector] public Player parasiticPlayer;

    [Header("Runtime Info")]
    public float currentBodyHealth;    // 현재 체력
    public Weapon currentWeapon;   // 현재 사용하고 있는 혈기

    private float _dashCoolDownTimer;

    private void Start()
    {
        StateMachine.Initialize(this);
        Initialize();
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

        // Fall 상태 체크
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
        
        // 대쉬 쿨다운
        _dashCoolDownTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdateState(this);

        IsGrounded(); // 바닥 체크 
    }

    public void Initialize()
    {
        if (!TryGetComponent(out animator)) Debug.LogError($"{animator.GetType()} 찾지 못함.");
        if (!TryGetComponent(out rb)) Debug.LogError($"{rb.GetType()} 찾지 못함.");
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
        player.StateMachine.TransitionToState(player.StateMachine.ParasiticState, player);
        StateMachine.TransitionToState(StateMachine.AwakeState, this);
        
        // 육체 능력치 적용(초기화) 로직
        _dashCoolDownTimer = parasiticPlayer.playerData.dashCoolDown;   // 대쉬 쿨다운 초기화
        currentBodyHealth = bodyData.maxBodyHealth;                     // 체력 초기화
        
        // 출혈 코루틴 재생 
        StartCoroutine(Bleed());
    }

    public void EndParasitic(Player player)
    {
        // 육체 기생 해제 로직
        parasiticPlayer = null;
        player.currentHostTransform = null;    // 위치 정보 해제
        player.StateMachine.TransitionToState(player.StateMachine.IdleState, player);
    }

    public void OnJump()
    {
        // 트랜지션 가능한 상태 체크
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
        // 트랜지션 가능한 상태 체크
        if (StateMachine.CurrentState != StateMachine.DisableState &&
            StateMachine.CurrentState != StateMachine.AwakeState)
        {
            // 대쉬 쿨다운 체크
            if (_dashCoolDownTimer >= parasiticPlayer.playerData.dashCoolDown)
            {
                _dashCoolDownTimer = 0f;
                StateMachine.TransitionToState(StateMachine.DashState, this);
            }
        }
    }
    
    public void OnAttack()
    {
        // 트랜지션 가능한 상태 체크
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

    public void TakeDamage(float amount, bool damageReduction = true, Vector2 hitDirection = new Vector2(), float knockbackForce = 0f)
    {
        // 상태 트랜지션
        StateMachine.TransitionToState(StateMachine.HitState, this);
        
        rb.AddForce(hitDirection * knockbackForce, ForceMode2D.Impulse);    // 넉백
        
        // 대미지 피해 계산
        float damage;
        if (damageReduction)
        {
            damage = amount * bodyData.damageReductionPercentage;
        }
        else
        {
            damage = amount;
        }
        
        currentBodyHealth -= damage;
       
      
        if (currentBodyHealth <= 0f)
        {
            // 사망
            StateMachine.TransitionToState(StateMachine.DieState, this);
        }

        //UI연동
        HUDController.instance.ChangeHpBar(currentBodyHealth, bodyData.maxBodyHealth);
    }

    public void Heal(float amount)
    {
        currentBodyHealth += amount;
        if (currentBodyHealth > bodyData.maxBodyHealth) // 최대체력 보다 체력이 많으면
        {
            currentBodyHealth = bodyData.maxBodyHealth;
        }
    }

    public void SetRandomBodyStats()
    {
        // 육체 능력치 랜덤 설정
        
    }

    private IEnumerator Bleed()
    {
        int tryNum = 0;
        
        // 출혈 1 당 걸리는 시간 계산
        float secondsPerBleed;
        secondsPerBleed = 1.0f / bodyData.bleedTime;
        WaitForSeconds waitForSeconds = new WaitForSeconds(secondsPerBleed);

        // 육체 살아있는 동안 지속 출혈
        while (StateMachine.CurrentState != StateMachine.DieState)
        {
            // 무한 반복 예외 처리
            if (tryNum >= 10000)
            {
                break;
            }

            // 다음 1 출혈까지 걸리는 시간 계산 
            yield return waitForSeconds;

            // 1 출혈 피해 적용
            TakeDamage(1f, false);
        }
    }
}
