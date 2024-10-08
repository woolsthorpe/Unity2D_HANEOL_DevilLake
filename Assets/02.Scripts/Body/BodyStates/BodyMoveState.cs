using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyMoveState : IBodyState
{
    public void Enter(Body body)
    {
        body.animator.Play("Belial Body Move");
    }

    public void Update(Body body)
    {
        if (InputManager.Movement.x == 0f)
        {
            body.StateMachine.TransitionToState(body.StateMachine.IdleState, body);
        }
    }

    public void FixedUpdate(Body body)
    {
        // 이동 방향 노말벡터 계산
        Vector3 direction = new Vector3(InputManager.Movement.x, 0f).normalized;
        direction.z = 0;
        
        // 이동 속도 계산
        float moveSpeed = body.parasiticPlayer.playerData.moveSpeed * body.bodyData.moveSpeedPercentage;
        
        body.TurnCheck(InputManager.Movement);
        body.transform.position += Time.fixedDeltaTime * moveSpeed * direction;
    }

    public void Exit(Body body)
    {

    }
}
