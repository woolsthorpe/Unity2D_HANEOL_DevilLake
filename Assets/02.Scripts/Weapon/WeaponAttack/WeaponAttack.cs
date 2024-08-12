using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAttack : ScriptableObject
{
    [Header("Weapon Attack Info")]
    public string attackName;
    public string attackDescription;
    public GameObject attackPrefab;             // 공격 프리팹
    public float coolDown;                      // 공격 대기시간
    public float knockbackForce;                // 강도 ( 밀치기 )
    public float damage;                        // 공격력
    
    [HideInInspector] public Body body;         // 시전 육체 클래스

    public abstract void Attack();
}
