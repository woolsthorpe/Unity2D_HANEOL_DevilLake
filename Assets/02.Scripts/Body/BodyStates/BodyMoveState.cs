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
        if (InputManager.Movement == Vector2.zero)
        {
            body.StateMachine.TransitionToState(body.StateMachine.IdleState, body);
        }
    }

    public void FixedUpdate(Body body)
    {
        Vector3 direction = InputManager.Movement.normalized;
        direction.z = 0;
        
        body.TurnCheck(InputManager.Movement);
        body.transform.position += Time.fixedDeltaTime * body.moveSpeed * direction;
    }

    public void Exit(Body body)
    {

    }
}
