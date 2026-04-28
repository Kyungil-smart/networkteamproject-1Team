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
        private PlayerMovement _movement;
        // 카메라
        // 전투
        // 상호작용
        
        private bool _lastSprintState;
        
        // 하위 모듈 생성자 할당 및 초기화
        public void Initialize(PlayerMovement move)
        {
            // 생성된 모듈 연결
            _movement = move;
            // _camera = c; _combat = cb; _interactor = i;
            BindEvents();
        }
        
        // 이동 관련 이벤트 할당
        void BindEvents()
        {
            _input.Enable();
             _input.onMove += OnMove;
        }
        
        // 이동 관련 이벤트 할당 해제(메모리 누수 방지)
        private void OnDestroy()
        {
            // _input이 없다면 할당 해제할 이벤트도 없으므로 return
            if (_input == null) return;
            _input.onMove -= OnMove;
            _input.onJump          += OnJump;
            // _input.onAttack        += OnAttack;
            // _input.onInteract      += OnInteractStart;
            // _input.Interact        += OnInteractTick;
            // _input.offInteract     += OnInteractCancel;
            // _input.onSprintChanged += OnSprintChanged;
        }
        
        private void Update()
        {
            // TODO: BattleInputReader에 onSprintChanged 이벤트 추가 후 이벤트 구독으로 전환
            // 다른 팀원의 BattleInputReader 작업이 마무리되면 박언약과 합의하여 변경 예정
            if (_input == null || _movement == null) return;
            
            bool currentSprint = _input.isSprint;
            if (currentSprint != _lastSprintState)
            {
                // _movement.SetSprint(currentSprint);
                _lastSprintState = currentSprint;
            }
        }
        
        // 이벤트 할당 함수(람다식 연결)
        void OnMove(Vector2 v) => _movement.SetMoveInput(v);
        private void OnJump() => _movement?.RequestJump();
        private void OnSprintChanged(bool b) => _movement?.SetSprint(b);
        
        // private void OnAttack() => _combat?.RequestAttack();
        // private void OnInteractStart() => _interactor?.OnInteractStart();
        // private void OnInteractTick() => _interactor?.OnInteractTick();
        // private void OnInteractCancel() => _interactor?.OnInteractCancel();
    }
}
