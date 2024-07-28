using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyIdleState : IBodyState
{
    public void Enter(Body body)
    {
        body.animator.Play("Belial Body Idle");
    }

    public void Update(Body body)
    {
        if (InputManager.Movement != Vector2.zero)
        {
            body.StateMachine.TransitionToState(body.StateMachine.MoveState, body);
        }
    }

    public void FixedUpdate(Body body)
    {
        
    }

    public void Exit(Body body)
    {

    }
}
