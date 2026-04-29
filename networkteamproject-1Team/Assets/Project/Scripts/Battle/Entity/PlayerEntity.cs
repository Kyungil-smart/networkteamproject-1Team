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

        void AlertDeath()
        {
            if (!IsServer) return;

            NotifyDeathClientRpc();

            // 게임 규칙 처리 위임
            BattleManager.Instance.DestroyPlayer(this);
        }

        [ClientRpc]
        void NotifyDeathClientRpc()
        {
            if (IsOwner) YouDied(); // 오너만 사망 연출
        }
        void YouDied()
        {
            Debug.LogWarning("[PlayerEntity] You Died!");
            // TODO: 사망 UI, 카메라 전환 등
        }
    }
}
