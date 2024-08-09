using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAttackState : IBodyState
{
    public void Enter(Body body)
    {
        // 공격 애니메이션 재생 로직 작성 
        body.animator.Play("Belial Body Attack");
        
        // 공격 프리팹 생성
        GameObject attackPrefab = body.InstantiateAttack(body.attackPrefab);
        attackPrefab.transform.position = body.attackPosition.position;
        
        // 공격 정보 전달
        Attack attack = attackPrefab.GetComponent<Attack>();
        attack.attacker = body.gameObject;
        //attack.damage = body. --------------------- ( 대미지 계산법 몰라서 안 써 놓음 )
        attack.damage = 5f; // 임시
    }

    public void Update(Body body)
    {
        // 공격 애니메이션 끝났는지 체크 후 Idle 상태로 트랜지션
        AnimatorStateInfo stateInfo = body.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Belial Body Attack") && stateInfo.normalizedTime >= 1.0f)
        {
            body.StateMachine.TransitionToState(body.StateMachine.IdleState, body);
        }
    }

    public void FixedUpdate(Body body)
    {

    }

    public void Exit(Body body)
    {

    }
}
