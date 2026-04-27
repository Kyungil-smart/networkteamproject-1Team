using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : NetworkBehaviour
{
    public Transform Target { get; set; }
    public NavMeshAgent Agent { get; set; }

    private void Awake()
    {
        Init();
    }
    
    private void Init()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    public void ChasePlayer(Transform target)
    {
        Agent.SetDestination(target.position);
    }
}
