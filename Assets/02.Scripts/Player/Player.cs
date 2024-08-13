using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; } = new PlayerStateMachine();
    
    [Header("Player Info")]
    public PlayerData playerData;               // 플레이어 데이터
    public CapsuleCollider2D collier;           // 플레이어 콜라이더
    public CircleCollider2D interactionRange;   // 상호작용 범위
    public Body initialBody = null;             // 초기 육체 
    
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody2D rb;

    [Header("Runtime Info")] 
    public Body currentHostBody;                  // 현재 기생하고 있는 숙주
    public IInteractable currentInteractable;     // 현재 가능한 상호작용 
    public float aliveTimeTimer;                  // 플레이어 육체없이 있던 시간 
    
    public bool isDamageImmunity;                 // 현재 피해 면역인지 
    public bool isLakeScene;                      // 본진 씬인지 확인 
    
    private bool _facingRight = true;
    private float _damageImmunityTimer;


    private void Start()
    {
        StateMachine.Initialize(this);

        // 초기 육체가 있다면 기생 
        if (initialBody)
        {
            initialBody.StartParasitic(this);
        }
    }

    private void Initialize()
    {
        if (!TryGetComponent(out sr)) Debug.LogError($"{sr.GetType()} 찾지 못함.");
        if (!TryGetComponent(out animator)) Debug.LogError($"{animator.GetType()} 찾지 못함.");
        if (!TryGetComponent(out rb)) Debug.LogError($"{rb.GetType()} 찾지 못함.");
    }
    
    private void OnEnable()
    {
        // 입력 이벤트 구독
        InputManager.OnInteract += OnInteract;
        
        // 씬 매니저 이벤트 구독
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        InputManager.OnInteract -= OnInteract;
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void Update()
    {
        StateMachine.UpdateState(this);
        
        // 피해 면역 시간 로직
        if (isDamageImmunity)
        {
            // 피해 면역 
            if (currentHostBody)
            {
                currentHostBody.sr.color = new Color(1f, 1f, 1f, 0.5f);
            }
            else
            {
                sr.color = new Color(1f, 1f, 1f, 0.5f);
            }
            
            _damageImmunityTimer += Time.deltaTime;
            if (_damageImmunityTimer >= playerData.damageImmunityTime)
            {
                // 해제
                isDamageImmunity = false;
                _damageImmunityTimer = 0f;
                if (currentHostBody)
                {
                    currentHostBody.sr.color = new Color(1f, 1f, 1f, 1f);
                }
                else
                {
                    sr.color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }
        
        // 게임오버 로직 
        // 본진이 아닐 때 로직 추가하기.
            if (StateMachine.CurrentState != StateMachine.DieState)
            {
                if (StateMachine.CurrentState != StateMachine.ParasiticState)   // 기생 중이 아닐 때, 
                {
                    aliveTimeTimer += Time.deltaTime;
                    if (aliveTimeTimer >= playerData.aliveTime)
                    {
                        // timer에 deltaTime을 더하여 aliveTime 보다 커지면 플레이어 사망
                        StateMachine.TransitionToState(StateMachine.DieState, this);
                    }
                }
            }
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdateState(this);
    }

    // 상호작용 
    public void OnInteract()
    {
        currentInteractable?.Interact(this);
    }

    public void TurnCheck(Vector2 moveInput)
    {
        if (_facingRight && moveInput.x < 0f)
        {
            Turn();
        }
        
        else if (!_facingRight && moveInput.x > 0f)
        {
            Turn();
        }
    }

    private void Turn()
    {
        _facingRight = !_facingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1f;
        transform.localScale = newScale;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            Debug.Log("상호작용 가능!");
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            Debug.Log("상호작용 불가능!");
            currentInteractable = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isLakeScene = scene.name == "Lake";
    }
}
