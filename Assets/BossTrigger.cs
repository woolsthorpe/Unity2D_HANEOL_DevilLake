using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private LavaObject LavaObj;
    [SerializeField] private GameObject BossObj;

    [SerializeField] private bool isTriggerPlayer=false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !isTriggerPlayer)
        {
            Debug.Log("플레이어 보스방 진입");
            isTriggerPlayer = true;
            RecognizePlayer();
            StartCoroutine(RecognizePlayer());
        }
    }

    IEnumerator RecognizePlayer()
    {
       
        HUDController.instance.OnBlackBoard();
        LavaObj.LavaRisehUp();
        yield return new WaitForSeconds(1f);
        //플레이어 상태 정지
        BossObj.GetComponent<Dandelsc>().Appear_Boss();
        // ui 활성화
        
    }
}
