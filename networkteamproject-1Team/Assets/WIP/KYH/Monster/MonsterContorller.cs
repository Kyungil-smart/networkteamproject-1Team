using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public enum StateType
{
    Idle = 0,
    Patrol = 1,
    Chase = 2,
    Attack = 3
}
/// <summary>
/// 몬스터의 전반적인 기능을 관리
/// </summary>
public class MonsterContorller : NetworkBehaviour
{
    [field:SerializeField] public MonsterData MonsterData { get; set; }
    private StateMachine _state;
    private LayerMask _layerMask;
    
    public NetworkVariable<StateType> currentState = new NetworkVariable<StateType>(
        writePerm: NetworkVariableWritePermission.Server);
    public MonsterAI MonsterAI { get; private set; }

    public MonsterIdleState IdleState { get; private set; }
    public MonsterPatrolState PatrolState { get; private set; }
    public MonsterChaseState ChaseState { get; private set; }
    public MonsterAttackState AttackState { get; private set; }


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Init();
        }
        else
        {
            MonsterAI.enabled = false;
        }
    }
    
    private void Update()
    {
        if (!IsServer) return;
        
        _state.Update();
    }
    
    private void Init()
    {
        _state = new StateMachine();
        MonsterAI = GetComponent<MonsterAI>();
        _layerMask = LayerMask.GetMask("Player");
        
        IdleState = new MonsterIdleState(this);
        PatrolState = new MonsterPatrolState(this);
        ChaseState = new MonsterChaseState(this);
        AttackState = new MonsterAttackState(this);
        
        _state.ChangeState(IdleState);
        currentState.Value = StateType.Idle;
    }

    public Transform DetectPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, MonsterData.chaseRange, _layerMask);

        Transform target = null;
        float minDistance = float.MaxValue;
        
        foreach (Collider col in colliders)
        {
            float dis = Vector3.Distance(transform.position, col.transform.position);

            if (dis < minDistance)
            {
                minDistance = dis;
                target = col.transform;
            }
        }
        return target;
    }

    public void ChangeState(StateType newState)
    {
        switch (newState)
        {
            case StateType.Idle:
                _state.ChangeState(IdleState);
                break;
            case StateType.Patrol:
                _state.ChangeState(PatrolState);
                break;
            case StateType.Chase:
                _state.ChangeState(ChaseState);
                break;
            case StateType.Attack :
                _state.ChangeState(AttackState);
                break;
        }
        
        currentState.Value = newState;
    }
}
