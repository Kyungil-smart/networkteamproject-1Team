using System;
using Unity.Netcode;
using UnityEngine;

namespace Battle
{
    public abstract class EntityBase : NetworkBehaviour, IDamageable
    {
        public int maxHp = 100;

        // 서버가 Write, 모든 클라이언트가 Read → HP가 자동으로 동기화됨
        public NetworkVariable<int> CurHp = new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        bool _isDead;
        public bool IsDead
        {
            get => _isDead;
            set
            {
                _isDead = value;
                if (_isDead) onDeath?.Invoke();
            }
        }
        public event Action onDeath;

        public override void OnNetworkSpawn()
        {
            if (IsServer) CurHp.Value = maxHp;
            CurHp.OnValueChanged += OnHpChanged;
        }

        public override void OnNetworkDespawn()
        {
            CurHp.OnValueChanged -= OnHpChanged;
        }

        void OnHpChanged(int prev, int next)
        {
            if (next <= 0 && !IsDead) IsDead = true;
        }

        // 서버에서 호출
        public virtual void TakeDamage(int damage) // Vector3 hitPoint, Vector3 hitNormal (추가 가능 구현시)
        {
            if (IsDead) return;
            CurHp.Value = Mathf.Clamp(CurHp.Value - damage, 0, maxHp);
        }
    }
}
