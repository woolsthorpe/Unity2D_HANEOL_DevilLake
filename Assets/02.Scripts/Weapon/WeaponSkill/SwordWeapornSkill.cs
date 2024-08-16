using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapornSkill : MonoBehaviour
{
    public float damage = 5;
    public float speed;
    public float returnSpeed;
    private Rigidbody2D rigid;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Shot(int lookDirection,Transform bodyTransform)
    {
        StartCoroutine(UseSkill(lookDirection,bodyTransform));
    }
    IEnumerator UseSkill(int direction, Transform bodyTransform)
    {
        rigid.AddForce(Vector2.right*direction*speed,ForceMode2D.Impulse);

        yield return new WaitForSeconds(1);
       
      
        this.transform.position = Vector3.MoveTowards(transform.position,bodyTransform.position,returnSpeed*Time.deltaTime);
        
    }


}
