using UnityEngine;
using TMPro;
using Unity.Netcode;

/// <summary>
/// 게임 씬 진입 후 다른 플레이어 합류를 기다리는 동안 표시되는 오버레이 UI
/// 서버(TeamManager)가 보내는 ClientRpc를 받아 UI를 통제
/// </summary>
public class GameWaitingUI : NetworkBehaviour
{
    public static GameWaitingUI Instance { get; private set; }

    [SerializeField] GameObject _panel;
    [SerializeField] TMP_Text _statusText;

    private void Awake()
    {
        Instance = this;
    }

    protected override void OnNetworkPostSpawn()
    {
        _statusText.text = "플레이어 합류 대기 중...";
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (Instance == this) Instance = null;
    }

    public void UpdateWaitingText(int current, int expected)
    {
        _statusText.text = $"플레이어 합류 대기 중... ({current}/{expected})";
    }

    public void ShowTimeoutError(int timeoutCount)
    {
        _statusText.color = Color.red;
        _statusText.text = $"씬 로드 Timeout 발생! ({timeoutCount}명)";
    }

    public void HideWaitingPanel()
    {
        _panel.SetActive(false);
    }
}
