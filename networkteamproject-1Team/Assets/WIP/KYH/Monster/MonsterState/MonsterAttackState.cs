using UnityEngine;

public class MonsterAttackState : IState
{
    private MonsterContorller _monsterController;

    public MonsterAttackState(MonsterContorller monsterController)
    {
        _monsterController = monsterController;
    }
    
    public void Enter()
    {
        _monsterController.MonsterAI.Agent.ResetPath();
    }

    public void Update()
    {
        if (_monsterController.DistanceToPlayer() > _monsterController.MonsterData.attackRange)
        {
            _monsterController.ChangeState(StateType.Chase);
        }
    }

    public void Exit()
    {
        
    }
}
