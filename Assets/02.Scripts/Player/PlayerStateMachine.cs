public class PlayerStateMachine
{
    public IPlayerState CurrentState { get; private set; }

    public PlayerIdleState IdleState { get; private set; } = new PlayerIdleState();
    public PlayerMoveState MoveState { get; private set; } = new PlayerMoveState();
    public PlayerParasiticState ParasiticState { get; private set; } = new PlayerParasiticState();
    public PlayerDieState DieState { get; private set; } = new PlayerDieState();

    public void Initialize(Player player)
    {
        TransitionToState(IdleState, player);
    }

    public void TransitionToState(IPlayerState newState, Player player)
    {
        CurrentState?.Exit(player);
        CurrentState = newState;
        CurrentState.Enter(player);
    }

    public void UpdateState(Player player)
    {
        CurrentState?.Update(player);
    }
    
    public void FixedUpdateState(Player player)
    {
        CurrentState?.FixedUpdate(player);
    }
}