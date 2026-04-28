using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 몬스터 AI Navmesh 행동을 수행
/// </summary>
public class MonsterAI : NetworkBehaviour
{
    private MonsterController _monsterController;
    public Transform Target { get; set; }
    public NavMeshAgent Agent { get; set; }
    public bool IsDetected { private get; set; }

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (IsDetected) ChasePlayer(Target);
        Target = _monsterController.DetectPlayer();
    }
    
    private void Init()
    {
        _monsterController = GetComponent<MonsterController>();
        Agent = GetComponent<NavMeshAgent>();
    }

    private void ChasePlayer(Transform target)
    {
        Agent.SetDestination(target.position);
    }
}
