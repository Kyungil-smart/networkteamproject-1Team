using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class TEST_CubeMove : NetworkBehaviour
{
    [SerializeField] float _moveSpeed = 1f;

    private void FixedUpdate()
    {
        if (!IsOwner || LocalManager.Instance.isInGame) return;

        if (Keyboard.current.aKey.isPressed)
        {
            transform.position += new Vector3(-_moveSpeed, 0, 0);
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            transform.position += new Vector3(_moveSpeed, 0, 0);
        }
        else if (Keyboard.current.wKey.isPressed)
        {
            transform.position += new Vector3(0, 0, _moveSpeed);
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            transform.position += new Vector3(0, 0, -_moveSpeed);
        }
    }
}