using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationEvent : MonoBehaviour
{
    public Attack attack;

    public void OnStartAttack()
    {
        attack.StartAttack();
    }

    public void OnEndAttack()
    {
        attack.EndAttack();
    }
}
