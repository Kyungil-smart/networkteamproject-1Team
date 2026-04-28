using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // 싱글톤 (GameManager와 동일한 패턴)
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
    }
    
    // [UI 오브젝트 연결]
    // 인스펙터에서 각 UI 오브젝트를 드래그로 연결
    
    [Header("HUD (게임 중 화면)")]
    [SerializeField] private GameObject hudPanel;                // 발전기, 생존자 표시 패널
    [SerializeField] private TextMeshProUGUI generatorCountText; // "발전기: 3/5"
    [SerializeField] private TextMeshProUGUI survivorCountText;  // "생존자: 3명"

    [Header("결과 화면")]
    [SerializeField] private GameObject resultPanel;             // 결과창 패널 
    [SerializeField] private TextMeshProUGUI resultCountText;    // "생존자 승리!" or "킬러 승리!"
    [SerializeField] private Button restartButton;               // 다시하기 버튼

    [Header("대기 화면")]
    [SerializeField] private GameObject waitingPanel;            // "플레이어 대기중..."

    // [이벤트 구독]
    private void start()
    {
        // GameManager 이벤트 에 내 함수들을 구독
        GameManager.Instance.OnGameStarted += HandleGameStarted;
        GameManager.Instance.OnGameOver += HandleGameOver;
        
        // 초기 상태: 대기 화면만 표시
        ShowWaitingScreen();
    }
    
    // [오브젝트가 파괴될 때 구독 해제]
    private void OnDestroy()
    {
        if  (Instance == this) Instance = null;
        GameManager.Instance.OnGameStarted -= 
        
    }




}
