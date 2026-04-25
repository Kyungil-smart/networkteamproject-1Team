using Unity.Netcode;
using UnityEngine;

// Player 프리팹에 부착
// 자신의 팀(TeamType)을 NetworkVariable로 보관하며 서버가 스폰 직후 설정한다
// 로컬 오너는 Team 변경 시 카메라 Culling Mask를 자동으로 전환한다
public class PlayerRole : NetworkBehaviour
{
    public NetworkVariable<TeamType> Team = new NetworkVariable<TeamType>(TeamType.None,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    // A/B 시각 레이어
    const int LayerAverage = 10;
    const int LayerBeautiful = 11;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        Team.OnValueChanged += OnTeamChanged;
    }
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        Team.OnValueChanged -= OnTeamChanged;
    }

    void OnTeamChanged(TeamType prev, TeamType next)
    {
        if (!IsOwner) return;
        ApplyTeamVision(next);
    }
    void ApplyTeamVision(TeamType team)
    {
        bool isTeamB = team == TeamType.B;
        Camera cam = Camera.main;
        if (isTeamB)
        {
            cam.cullingMask |= 1 << LayerBeautiful;
            cam.cullingMask &= ~(1 << LayerAverage);
        }
        else
        {
            cam.cullingMask |= 1 << LayerAverage;
            cam.cullingMask &= ~(1 << LayerBeautiful);
        }
        Debug.Log($"[PlayerRole] 팀 {(isTeamB ? "B → Beautiful" : "A → Average")} 시야 적용");
    }
}

