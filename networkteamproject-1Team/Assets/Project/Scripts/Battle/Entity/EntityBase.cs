using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Battle
{
    public abstract class EntityBase : NetworkBehaviour, IDamageable
    {
        public int maxHp = 100;
        public NetworkVariable<int> CurHp = new NetworkVariable<int>(0);

        bool _isDead;
        public bool IsDead
        {
            get => _isDead;
            set
            {
                if (!EqualityComparer<bool>.Default.Equals(_isDead, value))
                {
                    _isDead = value;
                    if (_isDead) onDeath?.Invoke();
                }
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
            if (next <= 0) IsDead = true;
        }

        // 서버에서 호출
        public virtual void TakeDamage(int damage) // Vector3 hitPoint, Vector3 hitNormal (추가 가능 구현시)
        {
            if (IsDead) return;
            CurHp.Value -= damage;
        }
    }
}
