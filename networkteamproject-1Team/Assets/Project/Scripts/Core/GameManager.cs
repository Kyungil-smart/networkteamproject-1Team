using Unity.Netcode;
using UnityEngine;

// 게임 씬에 배치하는 싱글톤
// 현재 게임 페이즈를 관리하고 승패 판정 진입점을 제공한다
// (DontDestroyOnLoad 아님 - 게임 씬 전용)
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum Phase { Waiting, InGame, GameOver }

    public Phase CurrentPhase { get; private set; } = Phase.Waiting;

    void Awake()
    {
        Instance = this;
    }

    // 서버만 호출
    public void StartGame()
    {
        if (!IsServer) return;
        CurrentPhase = Phase.InGame;
        Debug.Log("[GameManager] 게임 시작");
    }

    public void EndGame(TeamType winner)
    {
        if (!IsServer) return;
        CurrentPhase = Phase.GameOver;
        Debug.Log($"[GameManager] 게임 종료 - 승리 팀: {winner}");
        EndGameClientRpc(winner);
    }

    [ClientRpc]
    void EndGameClientRpc(TeamType winner)
    {
        Debug.Log($"[GameManager] 승리 팀: {winner}");
        // TODO: 게임 오버 UI 표시
    }

    // 현재 생존한 A / B 인원 조회 유틸
    public (int aCount, int bCount) GetTeamCounts()
    {
        int a = 0, b = 0;
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject == null) continue;
            if (!client.PlayerObject.TryGetComponent(out PlayerRole role)) continue;
            if (role.Team.Value == TeamType.A) a++;
            else b++;
        }
        return (a, b);
    }
}
