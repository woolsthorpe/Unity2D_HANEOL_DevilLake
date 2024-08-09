using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : IPlayerState
{
    public void Enter(Player player)
    {
        
    }

    public void Update(Player player)
    {
        if (InputManager.Movement == Vector2.zero)
        {
            player.StateMachine.TransitionToState(player.StateMachine.IdleState, player);
        }
    }

    public void FixedUpdate(Player player)
    {
        Vector3 direction = InputManager.Movement.normalized;
        direction.z = 0;
        
        player.TurnCheck(InputManager.Movement);
        player.transform.position += Time.fixedDeltaTime * player.playerData.moveSpeed * direction;
    }

    public void Exit(Player player)
    {
        
    }
}
