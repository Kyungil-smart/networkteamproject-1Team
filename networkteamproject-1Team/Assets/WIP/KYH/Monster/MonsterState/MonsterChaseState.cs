using UnityEngine;

public class MonsterChaseState : IState
{
    private MonsterContorller _monsterController;

    public MonsterChaseState(MonsterContorller monsterController)
    {
        _monsterController = monsterController;
    }
    
    public void Enter()
    {
        _monsterController.MonsterAI.IsDetected = true;
    }

    public void Update()
    {
        if (_monsterController.MonsterAI.Target == null)
        {
            _monsterController.ChangeState(StateType.Patrol);
            return;
        }

        if (_monsterController.DistanceToPlayer() <= _monsterController.MonsterData.attackRange)
        {
            _monsterController.ChangeState(StateType.Attack);
        }
    }

    public void Exit()
    {
        _monsterController.MonsterAI.IsDetected = false;
    }
}
