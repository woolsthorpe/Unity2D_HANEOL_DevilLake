using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyJumpState : IBodyState
{
    public void Enter(Body body)
    {
        body.animator.Play("Belial Body Jump");
        
        // 점프 힘 로직 
        body.rb.AddForce(Vector2.up * body.jumpPower);
    }

    public void Update(Body body)
    {

    }

    public void FixedUpdate(Body body)
    {
        Vector3 direction = new Vector3(InputManager.Movement.x, 0f).normalized;
        direction.z = 0;
        
        body.TurnCheck(InputManager.Movement);
        body.transform.position += Time.fixedDeltaTime * body.moveSpeed * direction;
    }

    public void Exit(Body body)
    {

    }
}
