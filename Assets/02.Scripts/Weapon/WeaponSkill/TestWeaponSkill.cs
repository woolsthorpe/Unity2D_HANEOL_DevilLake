using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/WeaponSkill/TestWeaponSkill")]
public class TestWeaponSkill : WeaponSkill
{
    public override void Use()
    {
        // 혈액 소모 로직
        body.currentBodyHealth -= bloodCost;
        if (body.currentBodyHealth <= 0f)
        {
            // 사망
            body.StateMachine.TransitionToState(body.StateMachine.DieState, body);
        }
        
        // 스킬 공격 프리팹 생성
        body.InstantiateAttack(body.currentWeapon.weaponSkill.skillAttackPrefab, body.transform);
        
        // 공격 정보 전달
        Attack attack = skillAttackPrefab.GetComponent<Attack>();
        attack.attacker = body.gameObject;                                                                  // 공격자 전달
        attack.anim.speed = body.bodyData.attackSpeedPercentage;                                            // 공격 속도
        attack.damage = body.currentWeapon.weaponSkill.damage * body.bodyData.bonusDamagePercentage;        // 대미지 계산
    }
}
