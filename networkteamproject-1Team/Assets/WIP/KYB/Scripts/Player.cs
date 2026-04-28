using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WIP.KYB.Scripts
{
    public class Player : NetworkBehaviour
    {
        private PlayerInput _playerInput;
        private Rigidbody _rb;
        private Vector2 _input;
        public Camera cam;

        [SerializeField] private float moveSpeed;

        [Header("레이 사거리")] public float interactionDistance = 3.0f;

        private IInteractable _interactableTarget;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                Camera playerCamera = GetComponentInChildren<Camera>();
                if (playerCamera != null)
                {
                    playerCamera.gameObject.SetActive(false);
                }
            }

            _playerInput = GetComponent<PlayerInput>();
            _rb = GetComponent<Rigidbody>();

            var moveAction = _playerInput.actions["Move"];
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;


            var interAction = _playerInput.actions["Interaction"];
            interAction.performed += OnInteractive;
            interAction.canceled += OnInteractive;
            interAction.started += OnInteractive;
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity =
                new Vector3( /*x,y,z*/ _input.x * moveSpeed, _rb.linearVelocity.y, _input.y * moveSpeed);
        }

        public void OnMove(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;

            _input = ctx.ReadValue<Vector2>();
        }

        private void OnInteractive(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;

            // TODO: 상호작용 코드 작성
            if (ctx.started)
            {
                _interactableTarget = InteractiveObject();

                if (_interactableTarget != null)
                {
                    _interactableTarget.InteractStart();
                }
            }
            // else if (ctx.performed) Debug.Log("상호작용 버튼 preformed");
            else if (ctx.canceled)
            {
                if (_interactableTarget != null)
                {
                    _interactableTarget.InteractStop();
                }
            }
        }

        private IInteractable InteractiveObject()
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                // 부딪힌 물체에 IInteractable이 가능한지 확인
                IInteractable target = hit.collider.GetComponent<IInteractable>();

                if (target != null)
                {
                    return target;
                }
            }

            return null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Transform camTransform = cam.transform;
            Gizmos.DrawRay(camTransform.position, camTransform.forward * interactionDistance);
        }
    }
}