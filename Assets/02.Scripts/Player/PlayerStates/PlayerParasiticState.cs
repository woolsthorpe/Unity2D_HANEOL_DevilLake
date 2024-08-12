using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParasiticState : IPlayerState
{
    public void Enter(Player player)
    {
        player.sr.enabled = false;
        player.collier.enabled = false;
        player.aliveTimeTimer = 0f;         // 육체없이 있던 시간 초기화
    }

    public void Update(Player player)
    {

    }

    public void FixedUpdate(Player player)
    {
        player.transform.position = player.currentHostBody.gameObject.transform.position;
    }

    public void Exit(Player player)
    {
        player.sr.enabled = true;
        player.collier.enabled = true;
    }
}
