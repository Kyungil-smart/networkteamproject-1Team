using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TeamType { None, A, B }

// 플레이어 스폰과 팀 배정 담당
// 팀 데이터는 각 PlayerRole.Team (NetworkVariable) 에 보관
public class PlayerSpawnManager : NetworkBehaviour
{
    [SerializeField] GameObject _playerPrefab;
    [SerializeField] Transform[] _spawnPoints;
    [SerializeField, Min(1)] int _startTeamBCount = 1;

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

    void SpawnAllPlayers(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        // 셔플로 팀 배정 결정
        List<ulong> shuffled = new List<ulong>(clientsCompleted);
        Shuffle(shuffled);
        int teamBCount = Mathf.Min(_startTeamBCount, shuffled.Count);

        Dictionary<ulong, TeamType> teamMap = new Dictionary<ulong, TeamType>();
        for (int i = 0; i < shuffled.Count; i++)
            teamMap[shuffled[i]] = i < teamBCount ? TeamType.B : TeamType.A;

        // 스폰 후 PlayerRole에 팀 직접 주입
        for (int i = 0; i < clientsCompleted.Count; i++)
        {
            ulong clientId = clientsCompleted[i];
            Transform sp = _spawnPoints[i % _spawnPoints.Length];

            GameObject instance = Instantiate(_playerPrefab, sp.position, sp.rotation);
            instance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

            TeamType team = teamMap[clientId];
            instance.GetComponent<PlayerRole>().Team.Value = team;

            Debug.Log($"[Spawn] Player {clientId} ({team})");
        }
    }
    public void SpawnAllPlayers()
    {
        SpawnAllPlayers(SceneManager.GetActiveScene().name, LoadSceneMode.Single,
            new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds), new List<ulong>());
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
