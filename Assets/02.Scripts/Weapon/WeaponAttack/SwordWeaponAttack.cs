using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/WeaponAttack/SwordWeaponAttack")]
public class SwordWeaponAttack : WeaponAttack
{
    public override void Attack()
    {
        // 공격 프리팹 생성
        GameObject attackObject = Instantiate(body.currentWeapon.weaponAttack.attackPrefab, body.transform);
        
        // 공격 정보 전달
        Attack attack = attackObject.GetComponent<Attack>();
        attack.attacker = body.gameObject;                                                                  // 공격자 전달
        attack.anim.speed = body.bodyData.attackSpeedPercentage;                                            // 공격 속도
        attack.damage = body.currentWeapon.weaponAttack.damage * body.bodyData.bonusDamagePercentage;       // 대미지 계산
        attack.knockbackForce = body.currentWeapon.weaponAttack.knockbackForce;                             // 강도 
    }
}
