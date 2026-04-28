using Unity.Netcode;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(int damage);
}

namespace Battle
{
    public class BattleManager : NetworkBehaviour
    {
        public static BattleManager Instance;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init() => Instance = null;

        public override void OnNetworkSpawn()
        {
            Instance = this;
        }

        public void DestroyEntity(EntityBase entity)
        {
            entity.NetworkObject.Despawn();
            //TODO: 탈락, 승패 판정 등
        }
    }
}

