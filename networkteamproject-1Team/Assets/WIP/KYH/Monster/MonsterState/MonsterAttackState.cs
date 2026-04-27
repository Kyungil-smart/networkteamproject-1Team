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
        
    }

    public void Update()
    {

    }

    public void Exit()
    {
        
    }
}
