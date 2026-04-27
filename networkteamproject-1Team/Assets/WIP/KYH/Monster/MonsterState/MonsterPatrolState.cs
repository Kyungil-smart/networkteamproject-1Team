using UnityEngine;

public class MonsterPatrolState : IState
{
    private MonsterContorller _monsterController;

    public MonsterPatrolState(MonsterContorller monsterController)
    {
        _monsterController = monsterController;
    }
    
    public void Enter()
    {
    }

    public void Update()
    {
        _monsterController.MonsterAI.Target = _monsterController.DetectPlayer();

        if (_monsterController.MonsterAI.Target != null)
        {
            _monsterController.ChangeState(StateType.Chase);
        }
    }

    public void Exit()
    {
    }
    
}
