using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Cinemachine;

public class TEST_PlayerMove : NetworkBehaviour
{
    [Header("Move")]
    [SerializeField] float _moveSpeed = 4f;
    [SerializeField] float _sprintMultiplier = 2f;
    [SerializeField] float _jumpHeight = 4.2f;
    [SerializeField] float _rotationSmoothTime = 0.12f;

    [Header("Camera")]
    [SerializeField] GameObject _cameraTarget;
    [SerializeField] float _topClamp = 70f;
    [SerializeField] float _bottomClamp = -30f;
    [SerializeField] float _mouseSensitivity = 0.3f;

    // 컴포넌트
    Animator _animator;
    CharacterController _controller;
    Camera _mainCamera;

    // 내부 상태
    Vector2 _moveInput;
    float _yaw;
    float _pitch;
    float _rotationVelocity;

    // 중력
    float _verticalVelocity;
    public float gravity = -9.81f;

    public BattleInputReader input;
#if UNITY_EDITOR
    private void Reset()
    {
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:BattleInputReader");
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            input = UnityEditor.AssetDatabase.LoadAssetAtPath<BattleInputReader>(path);
        }
        else
        {
            Debug.LogWarning("BattleInputReader SO를 찾을 수 없습니다");
        }
    }
#endif
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        input.Enable();
        input.onMove += OnMove;
        input.onJump += OnJump;

        _mainCamera = Camera.main;
        _yaw = _cameraTarget.transform.rotation.eulerAngles.y;

        SetupCinemachineCamera();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        input.onMove -= OnMove;
        input.onJump -= OnJump;
    }

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        ApplyGravity();
        MovePlayer();

        // ESC를 누르면 나가기 (테스트용)
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            NetworkManager.Singleton.Shutdown();
            LinkManager.Instance.isInGame = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    private void LateUpdate()
    {
        if (!IsOwner) return;
        RotateCamera();
    }

    // ────────────────────────────────────────────
    private void SetupCinemachineCamera()
    {
        GameObject camObj = GameObject.FindWithTag("GameController");
        camObj.TryGetComponent(out CinemachineCamera vcam);
        camObj.transform.SetParent(_cameraTarget.transform, false);
        vcam.Target.TrackingTarget = transform;
    }

    private void OnMove(Vector2 value) => _moveInput = value;
    private void OnJump() { if (_controller.isGrounded) _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * gravity); }

    private void MovePlayer()
    {
        float targetSpeed = _moveInput == Vector2.zero ? 0f : _moveSpeed * (input.isSprint ? _sprintMultiplier : 1f);

        // 카메라 기준 이동 방향
        Vector3 inputDir = new Vector3(_moveInput.x, 0f, _moveInput.y).normalized;
        if (_moveInput != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg
                                + _mainCamera.transform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y, targetAngle, ref _rotationVelocity, _rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
            inputDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        _controller.Move(inputDir.normalized * (targetSpeed * Time.deltaTime)
                         + Vector3.up * (_verticalVelocity * Time.deltaTime));

        _animator.SetFloat("Speed", targetSpeed);
    }

    private void ApplyGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = -2f;
        else
            _verticalVelocity += gravity * Time.deltaTime;
    }

    private void RotateCamera()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        _yaw += mouseDelta.x * _mouseSensitivity;
        _pitch -= mouseDelta.y * _mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, _bottomClamp, _topClamp);

        _cameraTarget.transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }
}
