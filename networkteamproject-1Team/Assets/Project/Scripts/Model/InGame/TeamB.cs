using Battle;
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

        // 내가B면 A팀(괴물)을 빨간색으로, B팀(사람)을 흰색으로 표시
        if (LocalManager.Instance.IamB)
        {
            nameText.color = Color.white;
        }
        else
        {
            nameText.color = Color.red;
        }
    }
}
