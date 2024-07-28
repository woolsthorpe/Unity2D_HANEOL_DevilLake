using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParasiticState : IPlayerState
{
    public void Enter(Player player)
    {
        player.sr.enabled = false;
        player.collier.enabled = false;
    }

    public void Update(Player player)
    {

    }

    public void FixedUpdate(Player player)
    {
        player.transform.position = player.currentHostTransform.position;
    }

    public void Exit(Player player)
    {
        player.sr.enabled = true;
        player.collier.enabled = true;
    }
}
