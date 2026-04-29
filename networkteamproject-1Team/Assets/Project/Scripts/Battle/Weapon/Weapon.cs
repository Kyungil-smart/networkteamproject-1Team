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
            input.onAttack += TryAttack;
            BattleManager.Instance.OnGameStart += Ready;
        }
        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;
            input.onAttack -= TryAttack;
            BattleManager.Instance.OnGameStart -= Ready;
        }

        void Ready() => _state = State.Ready;
        public void TryAttack()
        {
            if (_state == State.Ready && Time.time >= _lastAttackTime + weaponSO.cooltime)
            {
                _lastAttackTime = Time.time;
                Attack();
            }
        }
        void Attack()
        {
            // 아무것도 못 맞춤: Miss
            if (!Physics.Raycast(_attackPoint.position, transform.forward, out RaycastHit hit, weaponSO.range))
            {
                AudioManager.Instance.PlaySfx(weaponSO.attackMiss);
                return;
            }

            // 맞았지만 NetworkObject가 없음: Blocked
            NetworkObject targetNetObj = hit.collider.GetComponent<NetworkObject>();
            if (targetNetObj == null)
            {
                AudioManager.Instance.PlaySfx(weaponSO.attackBlocked);
                return;
            }

            // 네트워크 오브젝트에 명중
            AttackServerRpc(targetNetObj.NetworkObjectId, weaponSO.damage);
        }

        [ServerRpc]
        void AttackServerRpc(ulong targetId, int damage)
        {
            NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetId, out var targetNetObj);
            if (targetNetObj.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(damage);

            AttackClientRpc(OwnerClientId, damage, targetNetObj.OwnerClientId);
        }

        [ClientRpc]
        void AttackClientRpc(ulong attackerId, int damage, ulong targetId)
        {
            // 플레이어 명중: 공격자/피격자/주변 모든 클라이언트에서 Hit 사운드 재생
            AudioManager.Instance.PlaySfx(weaponSO.attackHit);
            Debug.Log($"[Weapon] 공격자={attackerId}, 피해자={targetId}, damage={damage}");
        }
    }
}
