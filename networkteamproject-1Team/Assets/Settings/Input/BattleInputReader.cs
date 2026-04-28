using UnityEngine;
using UnityEngine.InputSystem;
using static BattleInputAction;
using System;

// 인풋 액션을 받고 이벤트를 실행하는 SO 입니다. 필요한 곳에 SO를 참조하여 사용
public class BattleInputReader : ScriptableObject, IBattleActions
{
    public BattleInputAction inputAction;
    // => 추가
    private IInteractable _interactableTarget;
    private KYB.Player _p;

    public event Action<Vector2> onMove;
    public bool isSprint;

    public event Action onAttack;
    public event Action onInteract; public event Action Interact; public event Action offInteract;
    public event Action onJump;

    public event Action on1; public event Action on2; public event Action on3;

    public void Enable()
    {
        if (inputAction == null)
        {
            inputAction = new BattleInputAction();
            inputAction.Battle.SetCallbacks(this);
        }
        inputAction.Enable();
    }
    public void Disable()
    {
        inputAction.Disable();
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        onMove?.Invoke(context.ReadValue<Vector2>());
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprint = context.ReadValueAsButton();
    }

    void IBattleActions.OnAttack(InputAction.CallbackContext context)
    {
        if (context.started) onAttack?.Invoke();
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        /*
        if (context.started) onInteract?.Invoke();
        if (context.performed) Interact?.Invoke();
        if (context.canceled) offInteract?.Invoke();
        */
        // => 추가
        if (context.started)
        {
            _interactableTarget = _p.InteractiveObject();

            if (_interactableTarget != null)
            {
                _interactableTarget.InteractStart();
            }
        }
        else if (context.canceled)
        {
            if (_interactableTarget != null)
            {
                _interactableTarget.InteractStop();
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) onJump?.Invoke();
    }

    public void On_1(InputAction.CallbackContext context)
    {
        if (context.started) on1?.Invoke();
    }
    public void On_2(InputAction.CallbackContext context)
    {
        if (context.started) on2?.Invoke();
    }
    public void On_3(InputAction.CallbackContext context)
    {
        if (context.started) on3?.Invoke();
    }
}
