using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindRange : MonoBehaviour
{
    public int findtype;

    public FSpearsc fss;
    

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" && fss.AttackCooldown >= 8f)
        {
            if (findtype == 1) {
                fss.ChargeAttack(1);
            } else if (findtype == 2)
                fss.ChargeAttack(2);
        }

        if (other.gameObject.name == "Player" && findtype == 3) {
            fss.isBack = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" && findtype == 3)
        {
            fss.isBack = false;
        }
    }
}
