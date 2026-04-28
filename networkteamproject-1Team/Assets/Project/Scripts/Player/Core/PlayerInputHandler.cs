using System;
using UnityEngine;

namespace Player
{
    // BattleInputReader (SO) 구독 → 각 모듈로 라우팅.
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private BattleInputReader _input;
#if UNITY_EDITOR
        private void Reset()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BattleInputReader");
            if (guids.Length > 0)
                _input = UnityEditor.AssetDatabase.LoadAssetAtPath<BattleInputReader>(
                    UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]));
            else
                Debug.LogWarning("BattleInputReader SO를 찾을 수 없습니다");
        }
#endif
        
        // 하위로 입력을 받는 모듈 선언
        // 이동
        // 카메라
        // 전투
        // 상호작용

        // 하위 모듈 생성자 할당 및 초기화
        public void Initialize()
        {
            // 생성된 모듈 연결
            BindEvents();
        }
        
        // 이동 관련 이벤트 할당
        void BindEvents()
        {
            _input.Enable();
            // _input.onMove += OnMove;
        }
        
        // 이동 관련 이벤트 할당 해제(메모리 누수 방지)
        private void OnDestroy()
        {
            // _input이 없다면 할당 해제할 이벤트도 없으므로 return
            if (_input == null) return;
            // _input.onMove -= OnMove;
        }
        
        void Update()
        {
            // 입력이 있을시 이동 모듈에서 입력 Vector2 값을 전달해 이동 함수 호출
            // 추후 반응형으로 대체( 프로퍼티 )
        }
        
        // 이벤트 할당 함수(람다식 연결)
        // void OnMove(Vector2 v);
        // void OnJump();
        // void OnAttack();
        // void OnInteractStart();
        // void OnInteractTick();
        // void OnInteractCancel();
    }
}
