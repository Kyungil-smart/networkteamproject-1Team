using UnityEngine;

public class MonsterIdleState : IState
{
    private MonsterContorller _monsterController;

    public MonsterIdleState(MonsterContorller monsterController)
    {
        _monsterController = monsterController;
    }
    
    public void Enter()
    {

    }

    public void Update()
    {
        _monsterController.ChangeState(_monsterController.PatrolState);
    }

    public void Exit()
    {

    }
}
