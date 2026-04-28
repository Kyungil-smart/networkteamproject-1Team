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
        public event Action<TeamType> OnGameEnd;

        [Header("오디오")]
        public AudioResource countSound;

        // 재시작 겸용
        public void StartGame()
        {
            if (!IsServer) return;

            // 아직 살아있는 플레이어 제거
            for (int i = tm.activePlayers.Count - 1; i >= 0; i--) tm.activePlayers[i].NetworkObject.Despawn();

            // 재스폰 + 팀 재배정 + GameStart
            tm.SpawnAllPlayers();
        }

        // 모든 클라이언트에서 실행
        public async UniTaskVoid StartCountdown(List<TeamBase> players)
        {
            AudioManager.Instance.PlaySfx(countSound);
            await UniTask.Delay(1000); // 시작 딜레이 (임시로 짧게)
            OnGameStart?.Invoke();
            Debug.Log("게임을 시작하지");
        }

        // 사망한 플레이어를 제거하고 승패 판정 실행 (서버만 호출)
        public void DestroyPlayer(EntityBase entity)
        {
            if (entity.TryGetComponent(out TeamBase tb)) tm.activePlayers.Remove(tb);

            entity.NetworkObject.Despawn();

            CheckWinCondition();
        }

        // 각 팀 생존자 수를 확인하여 한 팀이 전멸했을 때 승리팀을 선언
        void CheckWinCondition()
        {
            int aliveA = tm.GetPlayersByTeam(TeamType.A).Count;
            int aliveB = tm.GetPlayersByTeam(TeamType.B).Count;

            Debug.Log($"[BattleManager] 생존: A팀={aliveA}, B팀={aliveB}");

            if (aliveA == 0)
            {
                // A팀 전멸: B팀 승리
                DeclareResultRpc(TeamType.B);
            }
            else if (aliveB == 0)
            {
                // B팀 전멸: A팀 승리 (B 팀이 전멸해도 게임끝나지 않는 게임디자인 고려중)
                DeclareResultRpc(TeamType.A);
            }
            else if (aliveA == 0 && aliveB == 0)
            {
                // 동시 사망: 무승부?!
                DeclareResultRpc(TeamType.None);
            }
        }

        // 승리팀을 모든 클라이언트에 전파
        [Rpc(SendTo.Everyone)]
        void DeclareResultRpc(TeamType winner)
        {
            string msg = winner == TeamType.None ? "무승부?!" : $"{winner}팀 승리!";
            Debug.Log($"<color=green>[BattleManager] 게임 종료: {msg}</color>");
            OnGameEnd?.Invoke(winner);
            // TODO: [BattleManager] 게임 종료 UI 표시등 추가 작업
        }

    }
}

