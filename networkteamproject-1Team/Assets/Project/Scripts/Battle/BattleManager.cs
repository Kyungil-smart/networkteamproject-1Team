using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int damage);
}

namespace Battle
{
    [RequireComponent(typeof(TeamManager))]
    public class BattleManager : NetworkBehaviour
    {
        public static BattleManager Instance;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init() => Instance = null;
        public override void OnNetworkSpawn() => Instance = this;

        [HideInInspector] public TeamManager tm;
        private void Awake() => tm = GetComponent<TeamManager>();


        public async UniTaskVoid GameStart(List<TeamBase> players)
        {

        }

        public void DestroyEntity(EntityBase entity)
        {
            entity.NetworkObject.Despawn();
            //TODO: 탈락, 승패 판정 등
        }
    }
}

