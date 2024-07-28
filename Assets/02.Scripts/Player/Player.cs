using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageAble
{
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private HealthController _playerHealth;
    [SerializeField] private Animator _playerAnimator;
    private StateMachine _stateMachine;
    private PlayerCombatStats _playerCombatStats;
    public Weapon _weapon;  // 혈기
    public BodyData _body;  // 육체

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _playerMovement.IsTopDownMovement = !_playerMovement.IsTopDownMovement;
        }
        
        else if (Input.GetKeyDown(KeyCode.G))
        {
            _weapon.UseWeaponSkill();
        }
    }

    public void TakeDamage(float amount)
    {
        
    }
}
