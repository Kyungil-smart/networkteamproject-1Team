using UnityEngine;

// PlayerA 프리팹에 부착
// 프리팹 구조:
//   PlayerA  (Layer: Player)
//   ├─ A     (Layer: Average)    A 시점: 보통 모델
//   ├─ B     (Layer: Beautiful)  B 시점: 괴물 모델
//   └─ ...
public class TeamA : TeamBase
{
    [SerializeField] GameObject _normalModel;
    [SerializeField] GameObject _monsterModel;
    protected override void OnTeamSetup(TeamType team) // 팀 배정 시 모든 클라이언트에서 호출됨
    {
        // B팀 클라이언트에서 PlayerA를 괴물로 보이게 처리
        // 앞 순서로 생성된 PlayerA 프리팹은 델리게이트로 처리 (플레이어 B 설정이 늦었을때 적용)
        if (LocalManager.Instance.IamB)
            ApplyMonsterAvatar();
        else
            LocalManager.Instance.OnIamBSet += ApplyMonsterAvatar;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        LocalManager.Instance.OnIamBSet -= ApplyMonsterAvatar;
    }

    // 루트 Animator의 Avatar를 괴물 모델 Animator에서 읽어 교체
    void ApplyMonsterAvatar()
    {
        _normalModel.gameObject.SetActive(false);
        _monsterModel.gameObject.SetActive(true);

        var rootAnimator = GetComponent<Animator>();
        var monsterAnimator = _monsterModel.GetComponent<Animator>();
        rootAnimator.avatar = monsterAnimator.avatar;
    }
}
