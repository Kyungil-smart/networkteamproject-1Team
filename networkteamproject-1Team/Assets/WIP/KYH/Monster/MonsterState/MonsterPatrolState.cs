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
    }

    public void Exit()
    {
    }
}
