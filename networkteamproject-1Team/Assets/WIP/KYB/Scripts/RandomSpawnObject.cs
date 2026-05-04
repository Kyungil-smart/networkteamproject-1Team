using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WIP.KYB.Scripts
{
    public class RandomSpawnObject : NetworkBehaviour
    {
        private static RandomSpawnObject instance;

        public GameObject spawnObject;
        public Transform[] spawnPoints;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public static RandomSpawnObject Instance
        {
            get
            {
                if (instance == null)
                {
                    return null;
                }

                return instance;
            }
        }


        public void SpawnObjects()
        {
            if (!IsServer) return;

            int random = Random.Range(0, spawnPoints.Length);
            Transform selectedPoint = spawnPoints[random];

            GameObject spawn = Instantiate(spawnObject, selectedPoint.position, Quaternion.identity);
            spawn.GetComponent<NetworkObject>().Spawn();
        }
    }
}
