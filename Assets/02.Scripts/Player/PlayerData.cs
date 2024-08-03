using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Player Data")]
public class PlayerData : ScriptableObject
{
    public float eyeMoveSpeed;

    public float gravityScale;      // 중력 배수
}
