using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

namespace Battle
{
    public class PlayerEntity : EntityBase
    {
        public AudioResource hitClip; //피격

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            onDeath += AlertDeath;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            onDeath -= AlertDeath;
        }

        // onDeath는 서버의 OnHpChanged에서만 발행됨
        void AlertDeath()
        {
            if (!IsServer) return;

            // 오너 클라이언트에게 사망 연출 RPC 전송
            NotifyDeathClientRpc();

            // 게임 규칙 처리 위임
            BattleManager.Instance.DestroyEntity(this);
        }

        [ClientRpc]
        void NotifyDeathClientRpc()
        {
            if (IsOwner) YouDied();
        }
        void YouDied()
        {
            Debug.LogWarning("[PlayerEntity] You Died!");
            // TODO: 사망 UI, 카메라 전환 등
        }
    }
}
