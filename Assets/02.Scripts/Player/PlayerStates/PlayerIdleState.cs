using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : IPlayerState
{
    public void Enter(Player player)
    {
        
    }

    public void Update(Player player)
    {
        if (InputManager.Movement != Vector2.zero)
        {
            player.StateMachine.TransitionToState(player.StateMachine.MoveState, player);
        }
    }

    public void FixedUpdate(Player player)
    {

    }

    public void Exit(Player player)
    {

    }
}
