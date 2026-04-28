using UnityEngine;

public class MonsterAttackState : IState
{
    private MonsterController _monsterController;

    public MonsterAttackState(MonsterController monsterController)
    {
        _monsterController = monsterController;
    }
    
    public void Enter()
    {
        _monsterController.MonsterAI.Agent.ResetPath();
    }

    public void Update()
    {
        if (_monsterController.MonsterAI.Target == null)
        {
            _monsterController.ChangeState(StateType.Patrol);
            return;
        }
        
        if (_monsterController.DistanceToPlayer() > _monsterController.MonsterData.attackRange)
        {
            _monsterController.ChangeState(StateType.Chase);
        }
    }

    public void Exit()
    {
        
    }
}
