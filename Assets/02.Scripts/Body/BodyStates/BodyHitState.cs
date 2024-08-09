using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyHitState : IBodyState
{
    public void Enter(Body body)
    {
        // Hit 애니메이션 재생
        //body.animator.Play("Belial Body Hit");
    }

    public void Update(Body body)
    {
        AnimatorStateInfo stateInfo = body.animator.GetCurrentAnimatorStateInfo(0);
        //if (stateInfo.IsName("Belial Body Hit") && stateInfo.normalizedTime >= 1.0f)
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
