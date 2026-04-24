using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InGameRule
{
    public enum TeamType
    {
        None,
        A,
        B
    }

    public class PlayerSpawnManager : NetworkBehaviour
    {
        [SerializeField] GameObject _playerPrefab;    // NetworkObject 가 붙은 플레이어 프리팹
        [SerializeField] Transform[] _spawnPoints;
        [SerializeField, Min(1)] int _targetTeamBCount = 1;

        public Dictionary<ulong, TeamType> assignedTeams = new Dictionary<ulong, TeamType>();

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SpawnAllPlayers;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer) return;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SpawnAllPlayers;
        }

        private void SpawnAllPlayers(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            // 팀 배정 진행
            AssignTeams(clientsCompleted);

            // 모든 클라이언트가 씬 로드 후, 캐릭터를 스폰시켜줍니다
            int index = 0;
            foreach (ulong clientId in clientsCompleted)
            {
                Transform sp = _spawnPoints[index % _spawnPoints.Length];

                TeamType team = assignedTeams[clientId];
                GameObject instance = Instantiate(_playerPrefab, sp.position, sp.rotation);
                instance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

                Debug.Log($"[Spawn] Player {clientId} ({team}) → {sp.position}");
                index++;
            }
        }
        private void AssignTeams(List<ulong> clients)
        {
            assignedTeams.Clear();

            List<ulong> randomizedClients = new List<ulong>(clients);
            Shuffle(randomizedClients);

            for (int i = 0; i < randomizedClients.Count; i++)
            {
                TeamType team = i < _targetTeamBCount ? TeamType.B : TeamType.A;
                assignedTeams[randomizedClients[i]] = team;
            }
        }
        private static void Shuffle(List<ulong> clientIds)
        {
            System.Random random = new System.Random();
            for (int i = clientIds.Count - 1; i > 0; i--)
            {
                int swapIndex = random.Next(i + 1);
                (clientIds[i], clientIds[swapIndex]) = (clientIds[swapIndex], clientIds[i]);
            }
        }

        public bool TryGetTeam(ulong clientId, out TeamType team)
        {
            return assignedTeams.TryGetValue(clientId, out team);
        }

    }
}