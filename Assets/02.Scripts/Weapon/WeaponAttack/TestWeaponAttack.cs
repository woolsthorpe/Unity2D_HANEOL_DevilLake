using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/WeaponAttack/TestWeaponAttack")]
public class TestWeaponAttack : WeaponAttack
{
    public override void Attack()
    {
        // 공격 프리팹 생성
        body.InstantiateAttack(body.currentWeapon.weaponAttack.attackPrefab, body.transform);
        
        // 공격 정보 전달
        Attack attack = attackPrefab.GetComponent<Attack>();
        attack.attacker = body.gameObject;                                                                  // 공격자 전달
        attack.anim.speed = body.bodyData.attackSpeedPercentage;                                            // 공격 속도
        attack.damage = body.currentWeapon.weaponAttack.damage * body.bodyData.bonusDamagePercentage;       // 대미지 계산
    }
}
