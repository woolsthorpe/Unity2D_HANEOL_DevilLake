using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public PlayerStateMachine StateMachine { get; private set; } = new PlayerStateMachine();
    
    [Header("Player Info")]
    public PlayerData playerData;               // 플레이어 데이터
    public CapsuleCollider2D collier;           // 플레이어 콜라이더
    public CircleCollider2D interactionRange;   // 상호작용 범위
    
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody2D rb;

    [HideInInspector] public Transform currentHostTransform;      // 현재 기생하고 있는 숙주의 위치 정보
    [HideInInspector] public IInteractable currentInteractable;   // 현재 가능한 상호작용 
    
    private bool _facingRight = true;

    private void Start()
    {
        StateMachine.Initialize(this);
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
    }

    private void OnDisable()
    {
        InputManager.OnInteract -= OnInteract;
    }
    
    private void Update()
    {
        StateMachine.UpdateState(this);
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdateState(this);
    }

    // 상호작용 
    public void OnInteract()
    {
        Debug.Log("Test");
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
}
