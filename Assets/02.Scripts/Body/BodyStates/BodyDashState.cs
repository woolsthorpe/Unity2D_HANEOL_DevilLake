using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyDashState : IBodyState
{
    public void Enter(Body body)
    {
        body.animator.Play("Belial Body Dash");

        if (body.facingRight)
        {
            body.rb.velocity = new Vector2(body.dashSpeed, 0f);
        }
        else
        {
            body.rb.velocity = new Vector2(-body.dashSpeed, 0f);
        }
    }

    public void Update(Body body)
    {
        AnimatorStateInfo stateInfo = body.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Belial Body Dash") && stateInfo.normalizedTime >= 1.0f)
        {
            body.StateMachine.TransitionToState(body.StateMachine.IdleState, body);
        }
    }

    public void FixedUpdate(Body body)
    {

    }

    public void Exit(Body body)
    {
        body.rb.velocity = new Vector2(0f, body.rb.velocity.y);
    }
}
