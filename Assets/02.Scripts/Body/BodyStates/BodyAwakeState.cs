using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyAwakeState : IBodyState
{
    public void Enter(Body body)
    {
        body.animator.SetBool("IsEnable", true);
    }

    public void Update(Body body)
    {
        bool testCode = true;
        if (testCode)
        {
            body.StateMachine.TransitionToState(body.StateMachine.IdleState, body);
        }
        else
        {
            AnimatorStateInfo stateInfo = body.animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Belial Body Awake") && stateInfo.normalizedTime >= 1.0f)
            {
                body.StateMachine.TransitionToState(body.StateMachine.IdleState, body);
            }
        }
    }

    public void FixedUpdate(Body body)
    {

    }

    public void Exit(Body body)
    {

    }
}
