using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Movement Settings")] 
    public float moveSpeed;         // 이동 속도
    public float jumpPower;         // 점프 힘
    public float dashSpeed;         // 대쉬 속도
    public float dashCoolDown;      // 대쉬 쿨다운
}
