using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Body : MonoBehaviour, IInteractable, IDamageable, IHealable
{
    public BodyStateMachine StateMachine { get; private set; } = new BodyStateMachine();
    
    [Header("Body Info")]
    public BodyData bodyData;
    
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Animator animator;
    
    [Header("Other")]
    public Collider2D bodyCollider;
    public Collider2D interactCollider;
    public Transform groundCheckTransform;
    
    [HideInInspector] public bool isGround;
    [HideInInspector] public bool facingRight;
    [HideInInspector] public Player parasiticPlayer;
    [HideInInspector] public bool isDie;
    [HideInInspector] public Sprite bodyDropSprite;           // 적 몬스터의 사망 애니메이션의 마지막 Sprite
    public BoxCollider2D bodyDropCollider;     // 육체 드롭 스프라이트에 딱 맞는 콜라이더 

    [Header("Runtime Info")]
    public float currentBodyHealth;     // 현재 체력
    public List<Weapon> weapons;        // 가지고 있는 혈기 리스트
    public Weapon currentWeapon;        // 현재 사용하고 있는 혈기
    public Weapon nextWeapon;           // 전환시 등잘할 다음 혈기 

    private float _dashCoolDownTimer;
    private int _curWeaponNum = 1;
    private float _changeWeaponTimer;
    private bool _canChangeWeapon;

    private void Start()
    {
        StateMachine.Initialize(this);
        
        if (!TryGetComponent(out animator)) Debug.LogError($"{animator.GetType()} 찾지 못함.");
        if (!TryGetComponent(out rb)) Debug.LogError($"{rb.GetType()} 찾지 못함.");
        if (!TryGetComponent(out sr)) Debug.LogError($"{sr.GetType()} 찾지 못함.");
    }
    
    public void Initialize()
    {
        // 육체 초기 이미지 적용
        sr.sprite = bodyDropSprite;
        Debug.Log(bodyDropSprite);
        Debug.Log(sr.sprite);
        
        // 육체 초기 이미지에 맞는 콜라이더만 활성화
        bodyCollider.enabled = false;
        // 스프라이트의 경계 크기를 사용하여 BoxCollider2D의 크기를 조정
        Bounds spriteBounds = sr.sprite.bounds;
        bodyDropCollider.size = spriteBounds.size;
        bodyDropCollider.offset = Vector2.zero;
        bodyDropCollider.enabled = true;
    }

    private void OnEnable()
    {
        // 입력 이벤트 구독
        InputManager.OnJump += OnJump;
        InputManager.OnDash += OnDash;
        InputManager.OnAttack += OnAttack;  // 어택이 곧 혈기공격 
        InputManager.OnWeaponSkill += OnWeaponSkill;
        InputManager.OnChangeWeapon += OnChangeWeapon;
    }

    private void OnDisable()
    {
        InputManager.OnJump -= OnJump;
        InputManager.OnDash -= OnDash;
        InputManager.OnAttack -= OnAttack;
        InputManager.OnWeaponSkill -= OnWeaponSkill;
        InputManager.OnChangeWeapon -= OnChangeWeapon;
    }

    private void Update()
    {
        StateMachine.UpdateState(this);
        Debug.DrawRay(groundCheckTransform.position, Vector3.down * 0.05f, Color.magenta);

        // Fall 상태 체크
        if (!isGround && 
            (StateMachine.CurrentState == StateMachine.IdleState ||
             StateMachine.CurrentState == StateMachine.MoveState ||
             StateMachine.CurrentState == StateMachine.JumpState))
        {
            if (rb.velocity.y < 0)
            {
                StateMachine.TransitionToState(StateMachine.FallState, this);
            }
        }
        
        // 대쉬 쿨다운
        _dashCoolDownTimer += Time.deltaTime;
        
        // 혈기 변경 딜레이
        _changeWeaponTimer += Time.deltaTime;
        if (!_canChangeWeapon && _changeWeaponTimer >= bodyData.changeWeaponDelay)
        {
            _changeWeaponTimer = 0f;
            _canChangeWeapon = true;
        }
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdateState(this);

        IsGrounded(); // 바닥 체크 
    }
    
    public void Interact(Player player)
    {
        // 해당 육체가 죽었으면 안되고
        if (isDie)
        {
            return;
        }

        // 해당 육체가 이미 기생한 육체이면 안됨
        if (player.currentHostBody == this)
        {
            return;
        }
        
        StartParasitic(player);
    }

    public void OnChangeWeapon()
    {
        if (_canChangeWeapon)
        {
            // 다음 혈기로 혈기 변경 
            currentWeapon = nextWeapon;
        
            // 다음 혈기 지정 로직 
            _curWeaponNum++;
            if (_curWeaponNum >= weapons.Count)
            {
                _curWeaponNum = 0;
            }
            nextWeapon = weapons[_curWeaponNum];

            // 딜레이 적용
            _canChangeWeapon = false;
        }
    }

    public void StartParasitic(Player player)
    {
        if (isDie)
        {
            return;
        }

        if (player.currentHostBody != null)
        {
            // 현재 갖고 있는 육체가 있었다면 해당 육체 사망 처리
            player.currentHostBody.StateMachine.TransitionToState(player.currentHostBody.StateMachine.DieState, player.currentHostBody);
        }
        
        // 육체 기생 로직
        parasiticPlayer = player;
        player.currentHostBody = this;    // 육체 정보 전달 
        player.StateMachine.TransitionToState(player.StateMachine.ParasiticState, player);
        StateMachine.TransitionToState(StateMachine.AwakeState, this);
        
        // 육체 능력치 적용(초기화) 로직
        _dashCoolDownTimer = parasiticPlayer.playerData.dashCoolDown;   // 대쉬 쿨다운 초기화
        currentBodyHealth = bodyData.maxBodyHealth;                     // 체력 초기화
        
        // 혈기 무작위 획득 로직
        weapons = new List<Weapon>();   // 보유한 혈기 리스트 초기화
        List<Weapon> copiedList = new List<Weapon>(bodyData.getableWeapons);    // 획득 가능한 혈기 리스트 깊은 복사 ( 참조 X )
        int randNum = Random.Range(1, copiedList.Count + 1);        // 무작위 갯수 
        for (int i = 0; i < randNum; i++)
        {
            int weaponNum = Random.Range(0, copiedList.Count);
            // 혈기 무작위 종류 획득 로직 작성
            weapons.Add(copiedList[weaponNum]);
            
            // 중복 제거
            copiedList.RemoveAt(weaponNum);
        }
        // 현재 혈기 지정
        currentWeapon = weapons[0];
        
        // 다음 혈기 지정
        if (weapons.Count == 1)
        {
            nextWeapon = weapons[0];
        }
        else
        {
            nextWeapon = weapons[1];
        }
        
        // 출혈 코루틴 재생 
        StartCoroutine(Bleed());
        
        // 체력 UI연동
        HUDController.instance.ChangeHpBar(currentBodyHealth, bodyData.maxBodyHealth);
    }

    public void EndParasitic(Player player)
    {
        // 육체 기생 해제 로직
        parasiticPlayer = null;
        player.currentHostBody = null;    // 육체 정보 해제 
        player.StateMachine.TransitionToState(player.StateMachine.IdleState, player);
    }

    public void OnJump()
    {
        // 트랜지션 가능한 상태 체크
        if (isGround && 
            (StateMachine.CurrentState == StateMachine.IdleState ||
            StateMachine.CurrentState == StateMachine.MoveState))
        {
            StateMachine.TransitionToState(StateMachine.JumpState, this);
        }
    }

    public void OnDash()
    {
        // 트랜지션 가능한 상태 체크
        if (StateMachine.CurrentState == StateMachine.IdleState ||
            StateMachine.CurrentState == StateMachine.MoveState ||
            StateMachine.CurrentState == StateMachine.JumpState ||
            StateMachine.CurrentState == StateMachine.FallState)
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
        // 피해 면역일 땐 대미지를 입히지 않음
        if (parasiticPlayer.isDamageImmunity)
        {
            return;
        }
        
        // 본진일 땐 대미지를 입히지 않음
        if (parasiticPlayer.isLakeScene)
        {
            return;
        }
        
        // 대미지 적용시 피해 면역 적용
        parasiticPlayer.isDamageImmunity = true;
        
        // 상태 트랜지션
        StateMachine.TransitionToState(StateMachine.HitState, this);
        
        // 넉백
        rb.AddForce(hitDirection * knockbackForce, ForceMode2D.Impulse);   
        
        // 대미지 피해 계산
        float damage;
        if (damageReduction)
        {
            damage = amount - amount * bodyData.damageReductionPercentage;
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
            tryNum++;
            if (tryNum >= 10000)
            {
                yield break;
            }
            
            // 본진일 땐 대미지를 입히지 않음
            if (parasiticPlayer.isLakeScene)
            {
                yield return null;
                continue;
            }

            // 다음 1 출혈까지 걸리는 시간 계산 
            yield return waitForSeconds;

            // 1 출혈 피해 적용
            currentBodyHealth -= 1.0f;
            if (currentBodyHealth <= 0f)
            {
                // 사망
                StateMachine.TransitionToState(StateMachine.DieState, this);
                yield break;
            }
            // 체력 UI연동
            HUDController.instance.ChangeHpBar(currentBodyHealth, bodyData.maxBodyHealth);
        }
    }

    public void DestroyBody()
    {
        Destroy(gameObject);
    }
}
