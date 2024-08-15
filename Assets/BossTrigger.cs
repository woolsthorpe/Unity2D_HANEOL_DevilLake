using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private LavaObject LavaObj;
    [SerializeField] private GameObject BossObj;

    [SerializeField]
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("플레이어 보스방 진입");
            RecognizePlayer();
        }
    }

    private void RecognizePlayer()
    {
        // LavaObj.LavaRisehUp();
        BossObj.GetComponent<Dandelsc>().Appear_Boss();
    }
}
