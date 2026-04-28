using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WIP.KYB.Scripts
{
    public class Player : NetworkBehaviour
    {
        private PlayerInput _playerInput;
        private Vector2 _input;
        public Camera cam;
        
        [Header("레이 사거리")] public float interactionDistance = 3.0f;

        private IInteractable _interactableTarget;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                GetComponentInChildren<Camera>().gameObject.SetActive(false);
            }
            
            cam = GetComponentInChildren<Camera>();
            _playerInput = GetComponent<PlayerInput>();
            
            var interAction = _playerInput.actions["Interaction"];
            interAction.performed += OnInteractive;
            interAction.canceled += OnInteractive;
            interAction.started += OnInteractive;
        }

        public override void OnNetworkDespawn()
        {
            var interAction = _playerInput.actions["Interaction"];
            interAction.performed -= OnInteractive;
            interAction.canceled -= OnInteractive;
            interAction.started -= OnInteractive;
        }
        
        private void OnInteractive(InputAction.CallbackContext ctx)
        {
            if (!IsOwner) return;
            
            if (ctx.started)
            {
                _interactableTarget = InteractiveObject();

                if (_interactableTarget != null)
                {
                    _interactableTarget.InteractStart();
                }
            }
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
            if (cam == null)
            {
                Debug.Log("[cam] cam이 null입니다.");
            }
            
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
            if (cam == null) return;
            
            Gizmos.color = Color.red;

            Transform camTransform = cam.transform;
            Gizmos.DrawRay(camTransform.position, camTransform.forward * interactionDistance);
        }
    }
}