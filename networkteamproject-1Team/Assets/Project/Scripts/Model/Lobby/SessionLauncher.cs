using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;

namespace Lobby
{
    public partial class SessionLauncher : NetworkBehaviour
    {
        [SerializeField] TMP_Text _loadingText;
        const int MIN_PLAYERS_TO_START = 2;
        static readonly string[] sceneNames = { "Map1" };

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
            else
            {
                Debug.LogWarning("BattleInputReader SO를 찾을 수 없습니다");
            }
        }
#endif

        public override void OnNetworkSpawn()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

            if (IsServer)
            {
                input.Enable();
                input.on1 += OnScene1;
                //input.on2 += OnScene2;
                //input.on3 += OnScene3;
            }
        }

        public override void OnNetworkDespawn()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;

            if (IsServer)
            {
                input.on1 -= OnScene1;
                //input.on2 -= OnScene2;
                //input.on3 -= OnScene3;
            }
        }

        void OnScene1() => TryLoadScene(0);
        //void OnScene2() => TryLoadScene(1);
        //void OnScene3() => TryLoadScene(2);

        private void OnClientConnected(ulong clientId)
        {
            if (LinkManager.Instance.isInGame) return;

            int count = NetworkManager.Singleton.ConnectedClientsIds.Count;
            Debug.Log($"[Session] 현재 접속자: {count}/{MIN_PLAYERS_TO_START}");
        }

        private void TryLoadScene(int index)
        {
            if (!IsServer) return;
            if (LinkManager.Instance.isInGame) return;

            int count = NetworkManager.Singleton.ConnectedClientsIds.Count;
            if (count < MIN_PLAYERS_TO_START)
            {
                Debug.Log($"[Session] 플레이어 부족 ({count}/{MIN_PLAYERS_TO_START}), 씬 전환 불가");
                return;
            }

            ShowLoadingClientRpc();
            string sceneName = sceneNames[index];
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        [ClientRpc]
        void ShowLoadingClientRpc()
        {
            LinkManager.Instance.isInGame = true;
            _loadingText.gameObject.SetActive(true); // 테스트용 로딩 보이기
        }
    }
}