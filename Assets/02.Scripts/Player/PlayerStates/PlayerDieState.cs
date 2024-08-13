using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDieState : IPlayerState
{
    public void Enter(Player player)
    {
        // 게임 오버 로직 작성 
        Debug.Log("플레이어 사망... 게임오버");
        player.gameObject.SetActive(false);
        
        // 본진으로 이동
        ScenceTransition.instance.GoToMain();
    }

    public void Update(Player player)
    {

    }

    public void FixedUpdate(Player player)
    {

    }

    public void Exit(Player player)
    {

    }
}
