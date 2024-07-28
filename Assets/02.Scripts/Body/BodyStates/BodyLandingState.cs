using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyLandingState : IBodyState
{
    public void Enter(Body body)
    {
        body.animator.Play("Belial Body Landing");
    }

    public void Update(Body body)
    {
        AnimatorStateInfo stateInfo = body.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Belial Body Landing") && stateInfo.normalizedTime >= 1.0f)
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
