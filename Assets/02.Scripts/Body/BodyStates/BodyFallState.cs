using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyFallState : IBodyState
{
    public void Enter(Body body)
    {
        body.animator.Play("Belial Body Fall");
    }

    public void Update(Body body)
    {
        if (body.isGround)
        {
            body.StateMachine.TransitionToState(body.StateMachine.LandingState, body);
        }
    }

    public void FixedUpdate(Body body)
    {
        Vector3 direction = new Vector3(InputManager.Movement.x, 0f).normalized;
        direction.z = 0;
        
        body.TurnCheck(InputManager.Movement);
        body.transform.position += Time.fixedDeltaTime * body.moveSpeed * direction;
    }

    public void Exit(Body body)
    {

    }
}
