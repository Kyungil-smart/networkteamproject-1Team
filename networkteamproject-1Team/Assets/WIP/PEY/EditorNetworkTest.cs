#if UNITY_EDITOR
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

// 에디터 상에서 네트워크 기능 테스트용 코드
public class EditorNetworkTest : NetworkBehaviour
{
    [SerializeField] PlayerSpawnManager PSM;
    private void Reset()
    {
        PSM = FindFirstObjectByType<PlayerSpawnManager>();
    }

    private void Update()
    {
        // F1: Host 시작
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            NetworkManager.Singleton.StartHost();
            Debug.Log("[TEST] 호스트 시작");
        }
        // F2: Client 시작
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("[TEST] 클라이언트 시작");
        }
        // F3: 모든 플레이어 스폰
        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            if (!IsServer) return;
            PSM.SpawnAllPlayers();
        }
    }
}
#endif