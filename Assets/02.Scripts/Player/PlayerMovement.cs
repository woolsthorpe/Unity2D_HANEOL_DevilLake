using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerMovementStats movementStats;
    [SerializeField] private Collider2D _bodyCollider; 
    [SerializeField] private Collider2D _feetCollider; 

    private Rigidbody2D _rb;

    public bool IsTopDownMovement { get; set; }

    // 이동 관련 
    private Vector2 _moveVelocity;
    private bool _isLookingRight;
    
    // 충돌 체크
    private RaycastHit2D _groundHit;
    private bool _isGrounded;

    private void Awake()
    {
        _isLookingRight = true;

        TryGetComponent(out _rb);
    }

    private void FixedUpdate()
    {
        CollisionCheck();

        Move(InputManager.Movement);
        Jump();
    }

    #region Move
    private void Move(Vector2 moveInput)
    {
        if (!IsTopDownMovement)
        {
            _rb.gravityScale = 1;
            
            //if (moveInput != Vector2.zero)
            {
                // 턴 체크 
                TurnCheck(moveInput);
            
                _rb.velocity = new Vector2(moveInput.x * movementStats.moveSpeed, _rb.velocity.y);
                print("횡 움직임");
            }
            //else
            {
                //_moveVelocity = Vector2.Lerp()
            }
        }
        else
        {
            _rb.gravityScale = 0;
            //if (moveInput != Vector2.zero)
            {
                TurnCheck(moveInput);
                
                _rb.velocity = new Vector2(moveInput.x, moveInput.y).normalized * movementStats.eyeStateMoveSpeed;
                print("탑다운 움직임");
            }
            //else
            {
            
            }
        }
    }
    
    private void TurnCheck(Vector2 moveInput)
    {
        if (_isLookingRight && moveInput.x < 0f)
        {
            Turn(false);
        }
        
        else if (!_isLookingRight && moveInput.x > 0f)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            _isLookingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            _isLookingRight = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }
    #endregion

    #region Jump
    private void Jump()
    {
        if (InputManager.JumpPressed && _isGrounded && !IsTopDownMovement)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, movementStats.jumpPower);
        }
    }
    #endregion

    #region Collision Check

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetCollider.bounds.center.x, _feetCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetCollider.bounds.size.x, movementStats.groundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, movementStats.groundDetectionRayLength, movementStats.groundLayer);
        if (_groundHit.collider != null)
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
        
        #region Debug Visualization

        if (movementStats.debugShowIsGroundedBox)
        {
            Color rayColor;
            if (_isGrounded)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }
            
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * movementStats.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * movementStats.groundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - movementStats.groundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }
        #endregion
    }
    
    private void CollisionCheck()
    {
        IsGrounded();
    }

    #endregion

}
