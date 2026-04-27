using Unity.Netcode;
using UnityEngine;

public enum TeamType { None, A, B }

// A / B 팀 공통 로직 담당 추상 클래스
public abstract class TeamBase : NetworkBehaviour
{
    const int LAYER_A = 10;
    const int LAYER_B = 11;

    // 스폰 직후 팀 결정
    public NetworkVariable<TeamType> Team = new NetworkVariable<TeamType>(TeamType.None,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        Team.OnValueChanged += OnTeamChanged;
    }

    public override void OnNetworkDespawn()
    {
        Team.OnValueChanged -= OnTeamChanged;
    }

    void OnTeamChanged(TeamType prev, TeamType next) => HandleTeamAssigned(next);
    protected abstract void OnTeamSetup(TeamType team);

    void HandleTeamAssigned(TeamType team)
    {
        // 공통 처리
        if (IsOwner)
        {
            ApplyCullingMask(team);
            LocalManager.Instance.IamB = (team == TeamType.B);
        }

        // 자식 처리 (IsOwner는 내부에서 판단)
        OnTeamSetup(team);
    }

    void ApplyCullingMask(TeamType team)
    {
        Camera cam = Camera.main;
        if (team == TeamType.B)
        {
            cam.cullingMask |=  1 << LAYER_B;
            cam.cullingMask &= ~(1 << LAYER_A);
        }
        else
        {
            cam.cullingMask |=  1 << LAYER_A;
            cam.cullingMask &= ~(1 << LAYER_B);
        }
        Debug.Log($"[{GetType().Name}] 팀 {team} 시야 적용");
    }

    [ClientRpc]
    public void ForceTeleportClientRpc(Vector3 position, Quaternion rotation, ClientRpcParams rpcParams = default)
    {
        if (TryGetComponent(out CharacterController cc))
            cc.enabled = false;

        transform.position = position;
        transform.rotation = rotation;

        if (TryGetComponent(out CharacterController cc2))
            cc2.enabled = true;
    }
}
