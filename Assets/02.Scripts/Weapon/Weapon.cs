using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Weapon")]
public class Weapon : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponName;
    public string weaponDescription;
    // 공격력 등등 스탯 추가하기.
    public WeaponAttack weaponAttack;   // 기본 공격
    public WeaponSkill weaponSkill;     // 혈기술

    public void Attack()
    {
        weaponAttack.Attack();
    }
    public void UseWeaponSkill()
    {
        weaponSkill.Use();
    }
}
