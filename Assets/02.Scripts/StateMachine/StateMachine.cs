using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private Dictionary<string, IState> _states;     // 상태 딕셔너리
    private IState _currentState;                   // 현재 상태

    public void AddState(string key)
    {
        // 상태 추가 로직 작성
        Debug.Log("상태 추가!");
    }

    public void ChangeState(string key)
    {
        // 상태 변경 로직 작성
        Debug.Log("상태 변경!");
    }
}
