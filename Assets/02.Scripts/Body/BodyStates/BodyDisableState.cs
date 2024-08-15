using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyDisableState : IBodyState
{
    public void Enter(Body body)
    {
        // 애니메이터 비활성화
        body.animator.enabled = false;
        
        // 상호작용 콜라이더 활성화
        body.interactCollider.enabled = true;
    }

    public void Update(Body body)
    {

    }

    public void FixedUpdate(Body body)
    {

    }

    public void Exit(Body body)
    {
        body.animator.enabled = true;
        body.bodyCollider.enabled = true;
        body.bodyDropCollider.enabled = false;
        
        // 상호작용 콜라이더 비활성화
        body.interactCollider.enabled = false;
    }
}
