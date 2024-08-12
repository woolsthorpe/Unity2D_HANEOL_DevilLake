using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Body Data")]
public class BodyData : ScriptableObject
{
    public string bodyName;
    public string bodyDescription;
    public float maxBodyHealth;             // 최대 혈액량 ( 체력 )
    public float extractedBloodAmount;      // 추가 흡혈량
    public float bleedTime;                 // 출혈 시간
    public float bonusDamagePercentage;     // 추가 피해 (%)
    public float damageReductionPercentage; // 피해 감소 (%)
    public float attackSpeedPercentage;     // 공격 속도 (%)
    public float moveSpeedPercentage;       // 이동 속도 (%)
    public List<Weapon> getableWeapons;     // 얻을 수 있는 혈기 리스트
}
