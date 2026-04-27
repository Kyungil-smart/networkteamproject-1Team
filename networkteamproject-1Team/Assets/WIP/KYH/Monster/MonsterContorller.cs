using Unity.Netcode;
using UnityEngine;

/// <summary>
/// 몬스터의 전반적인 기능을 관리
/// </summary>
public class MonsterContorller : NetworkBehaviour
{
    private MonsterData _monsterData;

    private StateMachine _state;

    public MonsterIdleState IdleState { get; private set; }
    public MonsterPatrolState PatrolState { get; private set; }
    public MonsterChaseState ChaseState { get; private set; }
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _state = new StateMachine();
        
        IdleState = new MonsterIdleState(this);
        PatrolState = new MonsterPatrolState(this);
        ChaseState = new MonsterChaseState(this);
        
        _state.ChangeState(IdleState);
    }

    private void Update()
    {
        _state.Update();
    }

    public void ChangeState(IState newState)
    {
        _state.ChangeState(newState);
    }
}
