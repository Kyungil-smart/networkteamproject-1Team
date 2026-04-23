using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// 자동으로 생성되는 싱글톤매니져, 컴포넌트를 사전에 세팅하지 않는 연결상태 관리
public partial class LinkManager : MonoBehaviour
{
    public static LinkManager Instance;

    public bool isInGame;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] // 씬 시작 전에 만들기
    private static void CreateInstance()
    {
        GameObject go = new GameObject("LinkManager");
        Instance = go.AddComponent<LinkManager>();
        DontDestroyOnLoad(go);
    }

    public virtual void Start()
    {
        if (NetworkManager.Singleton == null) return; // 테스트 씬 오류 방지
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        RelayNetworkService.InitializeAsync().Forget();
    }

    private void OnClientDisconnect(ulong clientId)
    {
        // 자기 자신이 해제된 경우 = 서버와의 연결이 끊김 = Host 이탈
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        Debug.Log("[Disconnect] 서버와의 연결이 끊겼습니다. 연결 씬으로 복귀합니다.");

        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(0);
    }
}
