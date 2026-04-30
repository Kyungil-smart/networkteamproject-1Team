using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// 자동생성 싱글톤, 로컬정보 관리
public partial class LocalManager : MonoBehaviour
{
    public static LocalManager Instance;

    public bool isInGame;

    public event Action OnIamBSet;
    bool _iamB;
    public bool IamB
    {
        get => _iamB;
        set { _iamB = value; if (value) OnIamBSet?.Invoke(); }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] // 씬 시작 전에 만들기
    private static void CreateInstance()
    {
        GameObject go = new GameObject("LocalManager");
        Instance = go.AddComponent<LocalManager>();
        DontDestroyOnLoad(go);
    }

    public virtual void Start()
    {
        if (NetworkManager.Singleton == null) return; // 테스트 씬 오류 방지

        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        // 자기 자신이 해제된 경우 = 서버와의 연결이 끊김 = Host 이탈
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        Debug.Log("[LinkManager] 서버와의 연결이 끊겼습니다. 연결 씬으로 복귀합니다.");

        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);
    }
}
