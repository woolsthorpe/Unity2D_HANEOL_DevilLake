using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    private float _maxHealth;
    private float _currentHealth;

    public void TakeDamage(float amount)
    {
        // 대미지 피해 로직 작성
        Debug.Log($"{amount} 만큼 대미지 피해!");
    }

    public void Heal(float amount)
    {
        // 체력 회복 로직 작성
        Debug.Log($"{amount} 만큼 체력 회복!");
    }
}
