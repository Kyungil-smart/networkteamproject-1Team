using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MonsterSpawnManager : NetworkBehaviour
{
    [SerializeField] private GameObject _monsterPrefab;
    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        
        MonsterSpawn();
    }

    private void Init()
    {
    }

    private void MonsterSpawn()
    {
        if (_monsterPrefab == null || _spawnPoints == null || _spawnPoints.Count == 0) return;
        
        int rand = UnityEngine.Random.Range(0, _spawnPoints.Count);
        
        GameObject monster = Instantiate(_monsterPrefab, _spawnPoints[rand].position, _spawnPoints[rand].rotation);
        monster.GetComponent<NetworkObject>().Spawn();
    }
}
