using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAttackState : IBodyState
{
    public void Enter(Body body)
    {
        // 공격 애니메이션 재생 로직 작성 
        body.animator.Play("Belial Body Attack");
        AnimatorStateInfo animatorStateInfo = body.animator.GetCurrentAnimatorStateInfo(0);
        // 공격 속도에 따른 애니메이션 속도 조절
        body.animator.speed = body.bodyData.attackSpeedPercentage;
        
        // 공격 실행 
        body.currentWeapon.weaponAttack.body = body;
        body.currentWeapon.weaponAttack.Attack();
    }

    public void Update(Body body)
    {
        // 공격 애니메이션 끝났는지 체크 후 Idle 상태로 트랜지션
        AnimatorStateInfo animatorStateInfo = body.animator.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("Belial Body Attack") && animatorStateInfo.normalizedTime >= 1.0f)
        {
            body.StateMachine.TransitionToState(body.StateMachine.IdleState, body);
        }
    }

    public void FixedUpdate(Body body)
    {

    }

    public void Exit(Body body)
    {
        // 애니메이션 속도 초기화
        body.animator.speed = 1.0f;
    }
}
