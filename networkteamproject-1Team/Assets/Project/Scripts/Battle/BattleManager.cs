using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

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

        [Header("오디오")]
        AudioResource _countSound;

        public async UniTaskVoid GameStart(List<TeamBase> players)
        {
            AudioManager.Instance.PlaySfx(_countSound);
            await UniTask.Delay(3000);
        }

        public void DestroyEntity(EntityBase entity)
        {
            entity.NetworkObject.Despawn();
            //TODO: 탈락, 승패 판정 등
        }
    }
}

