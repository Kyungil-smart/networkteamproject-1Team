using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Battle;
using Cysharp.Threading.Tasks;

// 플레이어 스폰과 팀 배정 담당
// 팀 데이터는 각 TeamBase.Team (NetworkVariable) 에 보관
// 스폰된 플레이어 목록을 ActivePlayers 로 관리
public class TeamManager : NetworkBehaviour
{
    [SerializeField] GameObject _playerPrefabA;  // TeamA 부착 프리팹
    [SerializeField] GameObject _playerPrefabB;  // TeamB 부착 프리팹
    [SerializeField] Transform[] _spawnPoints;
    [SerializeField, Min(0)] int _startTeamBCount = 1;

    // 서버 기준 스폰된 플레이어 목록 (서버 전용)
    public List<TeamBase> activePlayers = new();

    protected override void OnNetworkPostSpawn()
    {
        if (!IsServer) return;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SpawnAllPlayers;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SpawnAllPlayers;
    }

    // 팀별 플레이어 목록 조회
    public List<TeamBase> GetPlayersByTeam(TeamType team)
        => activePlayers.FindAll(r => r.Team.Value == team);

    void SpawnAllPlayers(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        activePlayers.Clear();
        // 셔플로 팀 배정 결정
        List<ulong> shuffled = new List<ulong>(clientsCompleted);
        Shuffle(shuffled);
        int teamBCount = Mathf.Min(_startTeamBCount, shuffled.Count);

        Dictionary<ulong, TeamType> teamMap = new Dictionary<ulong, TeamType>();
        for (int i = 0; i < shuffled.Count; i++)
            teamMap[shuffled[i]] = i < teamBCount ? TeamType.B : TeamType.A;

        // 팀에 맞는 프리팹으로 스폰 후 PlayerRoleBase에 팀 주입
        for (int i = 0; i < clientsCompleted.Count; i++)
        {
            ulong clientId = clientsCompleted[i];
            TeamType team = teamMap[clientId];
            Transform sp = _spawnPoints[i % _spawnPoints.Length];

            GameObject prefab = team == TeamType.B ? _playerPrefabB : _playerPrefabA;
            GameObject instance = Instantiate(prefab, sp.position, sp.rotation);
            instance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

            var role = instance.GetComponent<TeamBase>();
            role.Team.Value = team;
            activePlayers.Add(role);

            // 오너 클라이언트에게 직접 올바른 위치로 텔레포트하라고 명령
            ClientRpcParams rpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
            };
            role.ForceTeleportClientRpc(sp.position, sp.rotation, rpcParams);

            Debug.Log($"[TeamManager] Player {clientId} ({team})");
        }
            // 게임 시작
            GameStartRpc();
    }
    public void SpawnAllPlayers() // 테스트용 간편 호출
    {
        SpawnAllPlayers(SceneManager.GetActiveScene().name, LoadSceneMode.Single,
            new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds), new List<ulong>());
    }

    // 게임 시작을 클라이언트에게 전달
    [Rpc(SendTo.Everyone)]
    public void GameStartRpc()
    {
        BattleManager.Instance.StartCountdown(activePlayers).Forget();
    }

    void Shuffle(List<ulong> list)
    {
        System.Random rng = new System.Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
