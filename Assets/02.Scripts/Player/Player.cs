using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public PlayerData playerData;               // 플레이어 데이터
    public PlayerStateMachine StateMachine { get; private set; } = new PlayerStateMachine();
    public SpriteRenderer sr;
    public CapsuleCollider2D collier;           // 플레이어 콜라이더
    public CircleCollider2D interactionRange;   // 상호작용 범위
    public Animator animator;
    public Rigidbody2D rb;
    public Body body;                           // 육체
    public Weapon weapon;                       // 혈기 

    [HideInInspector] public Transform currentHostTransform;      // 현재 기생하고 있는 숙주의 위치 정보
    [HideInInspector] public IInteractable currentInteractable;   // 현재 가능한 상호작용 
    
    private bool _facingRight = true;

    private void Start()
    {
        StateMachine.Initialize(this);
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
