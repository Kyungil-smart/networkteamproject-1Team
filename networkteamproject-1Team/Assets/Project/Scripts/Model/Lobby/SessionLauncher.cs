using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;
using System.IO;

namespace Lobby
{
    public partial class SessionLauncher : NetworkBehaviour
    {
        [SerializeField] TMP_Text _loadingText;
        const int MIN_PLAYERS_TO_START = 2;

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
                input.Disable();
            }
        }

        // 빌드 인덱스 번호를 직접 넘깁니다 (예: Map1이 빌드 인덱스 1인 경우)
        void OnScene1() => TryLoadScene(1);
        //void OnScene2() => TryLoadScene(2);
        //void OnScene3() => TryLoadScene(3);

        private void OnClientConnected(ulong clientId)
        {
            if (LocalManager.Instance.isInGame) return;

            int count = NetworkManager.Singleton.ConnectedClientsIds.Count;
            Debug.Log($"[Session] 현재 접속자: {count}/{MIN_PLAYERS_TO_START}");
        }

        private void TryLoadScene(int index)
        {
            if (!IsServer) return;
            if (LocalManager.Instance.isInGame) return;

            int count = NetworkManager.Singleton.ConnectedClientsIds.Count;
            if (count < MIN_PLAYERS_TO_START)
            {
                Debug.Log($"[Session] 플레이어 부족 ({count}/{MIN_PLAYERS_TO_START}), 씬 전환 불가");
                return;
            }

            ShowLoadingClientRpc();

            // 1. 빌드 인덱스로 씬의 전체 경로를 가져옵니다 (예: "Assets/Scenes/Map1.unity")
            string scenePath = SceneUtility.GetScenePathByBuildIndex(index);

            // 2. 경로에서 확장자를 제외한 씬 이름만 추출합니다 (예: "Map1")
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);

            // 3. NGO의 SceneManager를 통해 씬을 로드합니다
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        [ClientRpc]
        void ShowLoadingClientRpc()
        {
            LocalManager.Instance.isInGame = true;
            _loadingText.gameObject.SetActive(true);
        }
    }
}
