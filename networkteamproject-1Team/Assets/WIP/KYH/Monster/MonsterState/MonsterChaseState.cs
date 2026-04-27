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
        float dis = Vector3.Distance(_monsterController.transform.position, _monsterController.MonsterAI.Target.position);

        if (dis > _monsterController.MonsterData.attackRange)
        {
            _monsterController.MonsterAI.ChasePlayer(_monsterController.MonsterAI.Target);
        }
        else
        {
            _monsterController.MonsterAI.Agent.ResetPath();
            // 클라이언트RCP 공격처리
            
        }
    }

    public void Exit()
    {
    }
}
