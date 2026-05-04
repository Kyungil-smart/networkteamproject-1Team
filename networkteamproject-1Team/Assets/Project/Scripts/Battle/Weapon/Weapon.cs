using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

namespace Battle
{
    public class Weapon : NetworkBehaviour
    {
        public enum State
        {
            None, Ready,//Empty, Reloading,
        }
        State _state;
        public WeaponSO weaponSO;

        [SerializeField] Transform _attackPoint;
        float _lastAttackTime;

        public BattleInputReader input;
#if UNITY_EDITOR
        private void Reset()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BattleInputReader");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                input = UnityEditor.AssetDatabase.LoadAssetAtPath<BattleInputReader>(path);
            }
            string[] guids2 = UnityEditor.AssetDatabase.FindAssets("t:WeaponSO");
            if (guids2.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids2[0]);
                weaponSO = UnityEditor.AssetDatabase.LoadAssetAtPath<WeaponSO>(path);
            }
        }
#endif
        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            
            input.Enable();
            BattleManager.Instance.OnGameStart += Ready;
        }
        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;
            BattleManager.Instance.OnGameStart -= Ready;
        }

        void Ready() => _state = State.Ready;

        public bool IsReady => _state == State.Ready
                               && Time.time >= _lastAttackTime + weaponSO.cooltime;
        public void TryAttack()
        {
            if (!IsReady) return;
    
            _lastAttackTime = Time.time;
            Attack();
        }
        
        public void Attack()
        {
            // 아무것도 못 맞춤: Miss
            if (!Physics.Raycast(_attackPoint.position, transform.forward, out RaycastHit hit, weaponSO.range))
            {
                AudioManager.Instance.PlaySfxWet(weaponSO.attackMiss, _attackPoint.position);
                return;
            }

            // 맞았지만 NetworkObject가 없음: Blocked (히트 위치로 전파)
            NetworkObject targetNetObj = hit.collider.GetComponent<NetworkObject>();
            if (targetNetObj == null)
            {
                BlockedServerRpc(hit.point);
                return;
            }

            // 네트워크 오브젝트에 명중 (히트 위치로 전파)
            AttackServerRpc(targetNetObj.NetworkObjectId, weaponSO.damage, hit.point);
        }

        [ServerRpc]
        void BlockedServerRpc(Vector3 hitPoint) => BlockedClientRpc(hitPoint);
        [ClientRpc]
        void BlockedClientRpc(Vector3 hitPoint) => AudioManager.Instance.PlaySfxWet(weaponSO.attackBlocked, hitPoint);

        [ServerRpc]
        void AttackServerRpc(ulong targetId, int damage, Vector3 hitPoint)
        {
            NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetId, out var targetNetObj);
            if (targetNetObj.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(damage);

            AttackClientRpc(OwnerClientId, damage, targetNetObj.OwnerClientId, hitPoint);
        }
        [ClientRpc]
        void AttackClientRpc(ulong attackerId, int damage, ulong targetId, Vector3 hitPoint)
        {
            // 타격 위치 기준 3D 공간음 재생
            AudioManager.Instance.PlaySfxWet(weaponSO.attackHit, hitPoint);
            Debug.Log($"[Weapon] 공격자={attackerId}, 피해자={targetId}, damage={damage}");
        }
    }
}
