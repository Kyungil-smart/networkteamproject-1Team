using Battle;
using System;
using UnityEngine;

// PlayerB 프리팹에 부착
// B는 모두에게 사람으로 보임
public class TeamB : TeamBase
{
    protected override void OnTeamSetup()
    {
        BattleManager.Instance.OnGameStart += UpdateNameText;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        BattleManager.Instance.OnGameStart -= UpdateNameText;
    }
    private void UpdateNameText()
    {
        UpdateNameText(PlayerName.Value.ToString());
    }
    protected override void UpdateNameText(string newName)
    {
        nameText.text = newName;

        // 내가B면 TeamB 컴포넌트 부착된 대상을 볼 때 빨간색으로 이름표시(서로 알수 있게)
        if (LocalManager.Instance.IamB)
        {
            nameText.color = Color.red;
        }
        else
        {
            nameText.color = Color.white; // 그 외엔 기본 색상
        }
    }
}
