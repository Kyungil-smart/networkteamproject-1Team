using UnityEngine;

public class MonsterAttackState : IState
{
    private MonsterController _monsterController;
    private float _timer;

    public MonsterAttackState(MonsterController monsterController)
    {
        _monsterController = monsterController;
    }
    
    public void Enter()
    {
        _monsterController.MonsterAI.Agent.ResetPath();
        _timer = 0f;
        _monsterController.MonsterData.isAttacking = true;
    }

    public void Update()
    {
        _timer += Time.deltaTime;
        
        if (_monsterController.MonsterAI.Target == null)
        {
            _monsterController.ChangeState(StateType.Patrol);
            return;
        }
        
        if (_monsterController.DistanceToPlayer() > _monsterController.MonsterData.attackRange)
        {
            _monsterController.ChangeState(StateType.Chase);
            return;
        }
        
        _monsterController.MonsterAttack.LookAtTarget();
        
        if (_timer >= _monsterController.MonsterData.attackCooldown)
        {
            _monsterController.TriggerAttackClientRpc();
            _timer = 0;
        }
    }

    public void Exit()
    {
        _monsterController.AttackCor();
    }
}
