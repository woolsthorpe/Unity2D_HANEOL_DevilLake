using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAttackState : IBodyState
{
    public void Enter(Body body)
    {
        // 공격 애니메이션 재생 로직 작성 
        
        // 공격 프리팹 생성
        GameObject attackPrefab = body.InstantiateAttack(body.attackPrefab);
        attackPrefab.transform.position = body.attackPosition.position;
    }

    public void Update(Body body)
    {
        // 공격 애니메이션 끝났는지 체크 후 Idle 상태로 트랜지션
    }

    public void FixedUpdate(Body body)
    {

    }

    public void Exit(Body body)
    {

    }
}
