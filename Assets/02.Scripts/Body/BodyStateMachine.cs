using UnityEngine;

public class BodyStateMachine
{
    public IBodyState CurrentState { get; private set; }

    public BodyDisableState DisableState { get; private set; } = new BodyDisableState();
    public BodyAwakeState AwakeState { get; private set; } = new BodyAwakeState();
    public BodyIdleState IdleState { get; private set; } = new BodyIdleState();
    public BodyMoveState MoveState { get; private set; } = new BodyMoveState();
    public BodyJumpState JumpState { get; private set; } = new BodyJumpState();
    public BodyFallState FallState { get; private set; } = new BodyFallState();
    public BodyLandingState LandingState { get; private set; } = new BodyLandingState();
    public BodyDashState DashState { get; private set; } = new BodyDashState();
    public BodyAttackState AttackState { get; private set; } = new BodyAttackState();
    public BodyDieState DieState { get; private set; } = new BodyDieState();

    public void Initialize(Body body)
    {
        TransitionToState(DisableState, body);
    }

    public void TransitionToState(IBodyState newState, Body body)
    {
        CurrentState?.Exit(body);
        CurrentState = newState;
        Debug.Log(CurrentState);
        CurrentState.Enter(body);
    }

    public void UpdateState(Body body)
    {
        CurrentState?.Update(body);
    }
    
    public void FixedUpdateState(Body body)
    {
        CurrentState?.FixedUpdate(body);
    }
}
