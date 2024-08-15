using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BodyDieState : IBodyState
{
    public void Enter(Body body)
    {
        // Die 애니메이션 재생 로직 작성 
        
        
        // 사망 처리
        body.isDie = true;
        body.interactCollider.enabled = false;
        
        // 플레이어와 육체 분리 로직 작성
        body.EndParasitic(body.parasiticPlayer);

        // 육체 삭제
        body.DestroyBody();
    }

    public void Update(Body body)
    {
        // Die 애니메이션 종료시 육체 삭제 로직 작성
        
    }

    public void FixedUpdate(Body body)
    {

    }

    public void Exit(Body body)
    {

    }
}
