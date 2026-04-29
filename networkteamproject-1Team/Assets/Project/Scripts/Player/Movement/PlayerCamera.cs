using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("ViewPoints (각 모델별)")]
        [SerializeField] private Transform _viewPointA;
        [SerializeField] private Transform _viewPointB;

        [Header("Look")]
        [SerializeField] private float _upClamp = -80f;
        [SerializeField] private float _downClamp = 80f;
        [SerializeField] private float _mouseSensitivity = 0.3f;

        private CinemachineCamera _virtualCamera;
        private Transform _activeViewPoint;
        private float _yaw;
        private float _pitch;
        private bool _isOwnerView;
        
        public float YawAngle => _yaw;
        public Transform ViewPoint => _activeViewPoint;

        public void SetupOwnerView()
        {
            if (_viewPointA == null)
            {
                Debug.LogError("[PlayerCamera] ViewPoint_A is not assigned.");
                return;
            }
            
            // 초기 ViewPoint = A (기본 모델)
            _activeViewPoint = _viewPointA;
            AssignToVirtualCamera();
            
            // B 전환 이벤트 구독
            if (LocalManager.Instance != null)
                LocalManager.Instance.OnIamBSet += SwitchToB;
            
            _yaw = transform.eulerAngles.y;
            _isOwnerView = true;
        }
        
        private void AssignToVirtualCamera()
        {
            // 씬에서 시네머신 카메라 찾아서 Target만 갈아끼우기
            if (_virtualCamera == null)
                _virtualCamera = FindAnyObjectByType<CinemachineCamera>();
        
            if (_virtualCamera == null)
            {
                Debug.LogError("[PlayerCamera] CinemachineCamera not found in scene.");
                return;
            }
        
            _virtualCamera.Target.TrackingTarget = _activeViewPoint;
        
            // Near Clip 설정 (메인 카메라에)
            if (UnityEngine.Camera.main != null)
                UnityEngine.Camera.main.nearClipPlane = 0.05f;
        }
        
        private void SwitchToB()
        {
            if (_viewPointB == null)
            {
                Debug.LogError("[PlayerCamera] ViewPoint_B is not assigned.");
                return;
            }
            _activeViewPoint = _viewPointB;
            AssignToVirtualCamera();  // 카메라 재부착
        }
        
        private void OnDestroy()
        {
            if (LocalManager.Instance != null)
                LocalManager.Instance.OnIamBSet -= SwitchToB;
        }

        private void Update()
        {
            if (!_isOwnerView || !enabled) return;
            HandleMouseInput();
        }

        private void LateUpdate()
        {
            // 위치는 Position Constraint가 자동 추적
            // 회전만 마우스 룩으로 직접 적용
            if (!_isOwnerView || _activeViewPoint == null) return; 
            _activeViewPoint.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }

        private void HandleMouseInput()
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            _yaw += delta.x * _mouseSensitivity;
            _pitch -= delta.y * _mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, _upClamp, _downClamp);
        }
    }
}