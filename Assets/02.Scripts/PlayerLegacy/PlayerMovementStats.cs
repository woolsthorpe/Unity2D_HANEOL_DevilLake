using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Player Movement Stats")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Move")] 
    public float moveSpeed;
    public float eyeStateMoveSpeed;

    [Header("Jump")] 
    public float jumpPower;

    [Header("Grounded/Collision Checks")] 
    public LayerMask groundLayer;
    public float groundDetectionRayLength = 0.02f;

    [Header("Gravity Scale")] 
    public float gravityScale = 1.0f;

    [Header("Debug")] 
    public bool debugShowIsGroundedBox;
}
