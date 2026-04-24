using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InGameRule
{
    public class PlayerSpawnManager : NetworkBehaviour
    {
        [SerializeField] GameObject _playerPrefab;    // NetworkObject 가 붙은 플레이어 프리팹
        [SerializeField] Transform[] _spawnPoints;

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
            // 모든 클라이언트가 씬 로드 후, 캐릭터를 스폰시켜줍니다
            int index = 0;
            foreach (ulong clientId in clientsCompleted)
            {
                Transform sp = _spawnPoints[index % _spawnPoints.Length];

                GameObject instance = Instantiate(_playerPrefab, sp.position, sp.rotation);
                instance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

                Debug.Log($"[Spawn] Player {clientId} → {sp.position}");
                index++;
            }
        }
    }
}