using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class TEST_ColorRpc : NetworkBehaviour
{
    Renderer _renderer;

    public override void OnNetworkSpawn()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        // Q 키: 서버에 색상 변경 요청
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            RequestChangeColorServerRpc(r, g, b);
        }
    }

    // 색상 변경 요청 (클라이언트 → 서버)
    [ServerRpc]
    private void RequestChangeColorServerRpc(float r, float g, float b)
    {
        // 서버가 검증 후 모든 클라이언트에 색상 변경 지시
        ApplyColorClientRpc(r, g, b);
    }
    // 색상 적용 (서버 → 모든 클라이언트)
    [ClientRpc]
    private void ApplyColorClientRpc(float r, float g, float b)
    {
        if (_renderer == null) return;
        _renderer.sharedMaterial.color = new Color(r, g, b);
    }
}
