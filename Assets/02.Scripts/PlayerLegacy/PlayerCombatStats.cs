using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Player Combat Stats")]
public class PlayerCombatStats : ScriptableObject
{
    public float maxHealth;
    public float attackDamage;
    public float damageReduction; // 피해 감소
    public float attackSpeed;     // 공격 속도
}
