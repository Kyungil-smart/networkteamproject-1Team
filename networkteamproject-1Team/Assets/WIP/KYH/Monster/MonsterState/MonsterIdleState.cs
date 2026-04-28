using System.Collections;
using System.Collections.Generic;
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
        _monsterController.ChangeState(StateType.Patrol);
    }

    public void Exit()
    {

    }
}
