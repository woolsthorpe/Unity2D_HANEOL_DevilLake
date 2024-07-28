using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Body Data")]
public class BodyData : ScriptableObject
{
    public string bodyName;
    public string bodyDescription;
    // 육체 리소스, 애니메이터 등등도 필요할 듯.
    public float maxBlood;                  // 최대 혈액량
    public float extractedBloodAmount;      // 흡혈량
    public float bleedTime;                 // 출혈 시간
    public float bonusDamagePercentage;     // 추가 피해
    public float damageReductionPercentage; // 피해 감소
    public float attackSpeedPercentage;     // 공격 속도
    public float moveSpeedPercentage;       // 이동 속도
}
