using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using Cysharp.Threading.Tasks;

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

        public event Action OnGameStart;

        [Header("오디오")]
        public AudioResource countSound;

        public void StartGame() // 재시작 겸용
        {
            // 아직 살아있는 플레이어 제거
            for (int i = tm.activePlayers.Count - 1; i >= 0; i--)
                tm.activePlayers[i].NetworkObject.Despawn();

            // 재스폰 + 팀 재배정 + GameStart
            tm.SpawnAllPlayers();
        }
        public async UniTaskVoid StartCountdown(List<TeamBase> players) // 게임 시작전에는 움직이지 못하게 한다던가 고려중
        {
            AudioManager.Instance.PlaySfx(countSound);
            await UniTask.Delay(1000); // 시작 딜레이 (임시로 짧게)
            OnGameStart?.Invoke();
            Debug.Log("게임을 시작하지");
        }

        public void DestroyPlayer(EntityBase entity)
        {
            if (entity.TryGetComponent(out TeamBase tb)) tm.activePlayers.Remove(tb);
            entity.NetworkObject.Despawn();
            //TODO: 탈락, 승패 판정 등
        }

    }
}

