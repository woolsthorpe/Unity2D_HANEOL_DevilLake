using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class WeaponSkill : ScriptableObject
{
    [Header("Weapon Skill Info")]
    public string skillName;
    public string skillDescription;
    public GameObject skillAttackPrefab;        // 스킬 공격 프리팹
    public float skillCoolDown;                 // 스킬 쿨다운 
    public float bloodCost;                     // 혈액 소모량
    public float knockbackForce;                // 강도 ( 밀치기 )
    public float damage;                        // 공격력
    
    [HideInInspector] public Body body;         // 시전 육체 클래스

    public abstract void Use();
}
