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
    public float aliveTime;         // 육체없이 살아있을 수 있는 시간
    public float damageImmunityTime;// 무적 시간
}
