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
    }

    public void Update()
    {
    }

    public void Exit()
    {
    }
}
