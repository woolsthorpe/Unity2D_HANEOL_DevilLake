using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Movement Stats")]
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

    [Header("Debug")] 
    public bool debugShowIsGroundedBox;
}
