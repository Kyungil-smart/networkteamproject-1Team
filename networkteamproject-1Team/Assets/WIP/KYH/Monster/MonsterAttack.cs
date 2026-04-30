using System;
using Unity.Netcode;
using UnityEngine;

public class MonsterAttack : NetworkBehaviour
{
    private MonsterController _monsterController;
    private LayerMask _playerB;
    private float _timer;
    private Ray _ray;

    public override void OnNetworkSpawn()
    {
        Init();
    }

    private void Init()
    {
        _monsterController = GetComponent<MonsterController>();
        // 테스트를 위해 임시로 Player
        _playerB = LayerMask.GetMask("Player");
    }
    
    public void Attack()
    {
        if (!IsServer) return;
        
        _ray = new Ray(transform.position + _monsterController.MonsterData.offset, transform.forward);
        
        if (Physics.Raycast(_ray, out RaycastHit hit, _monsterController.MonsterData.attackRange, _playerB))
        {
            Debug.Log("충돌!" + hit.collider.gameObject.name);
            if (hit.collider.TryGetComponent<TestPlayer>(out var player))
            {
                player.TakeDamage(_monsterController.MonsterData.attackDamage);
            }
        }
        
    }

    public void LookAtTarget()
    {
        Transform target = _monsterController.MonsterAI.Target;
        if (target == null) return;
        
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(_ray.origin, _ray.direction);
    }
}
