#if UNITY_EDITOR
using Battle;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

// 에디터 상에서 네트워크 기능 테스트용 코드
public class TEST_NetworkTester : NetworkBehaviour
{
    [SerializeField] TeamManager PSM;
    private void Reset()
    {
        PSM = FindFirstObjectByType<TeamManager>();
    }

    private void Update()
    {
        // F1: Host 시작
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            var nm = NetworkManager.Singleton;
            if (nm.IsListening || nm.IsClient || nm.IsServer)
            {
                Debug.LogWarning("[TEST] 이미 연결 중입니다. 씬을 완전히 정리하고 싶다면 에디터 플레이를 중지해주세요.");
                return;
            }

            nm.StartHost();
            Debug.Log("[TEST] 호스트 시작");
        }
        // F2: Client 시작
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            var nm = NetworkManager.Singleton;
            if (nm.IsListening || nm.IsClient || nm.IsServer)
            {
                Debug.LogWarning("[TEST] 이미 연결 중입니다.");
                return;
            }

            nm.StartClient();
            Debug.Log("[TEST] 클라이언트 시작");
        }
        // F3: 게임 시작 (재시작 겸용)
        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            if (!IsServer) return;
            BattleManager.Instance.StartGame();
        }
    }
}
#endif