using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.Netcode;

public class PressAction : NetworkBehaviour
{
    public Image image;
    
    public Action OnPressStart;
    public Action OnPressCanceled;
    public Action OnPressCompleted;
    
    private Coroutine _coroutine;
    
    // 흠 그니까 클리어된 객체는 모두가 읽고, 쓰기를 할 수 있게 해야제 ㅇㅇ 
    private NetworkVariable<bool> _isPressClear = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
        );

    private NetworkVariable<float> _currentTime = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
        );
    
    
    private float _holdTime = 10f;
    
    
    
    public override void OnNetworkSpawn()
    {
        _currentTime.OnValueChanged += OnUpdateFillAmountUI;
        
        OnUpdateFillAmountUI(0, _currentTime.Value);
    }

    public override void OnNetworkDespawn()
    {
        _currentTime.OnValueChanged -= OnUpdateFillAmountUI;
    }
    
    /*
    /// <summary>
    /// 스크립트가 활성화될 때 이벤트를 구독한다.
    /// </summary>
    private void OnEnable()
    {
        interactAction.action.started += OnPressStarted;
        interactAction.action.canceled += OnPressCanceled;
    }
    
    /// <summary>
    /// 스크립트가 비활성화될 때 이벤트를 구독 해제한다 (안하면 메모리 누수)
    /// </summary>
    private void OnDisable()
    {
        interactAction.action.started -= OnPressStarted;
        interactAction.action.canceled -= OnPressCanceled;
    }
    */
    
    /*
    /// <summary>
    /// 키가 눌렸을 때 실행될 콜백 함수들
    /// </summary>
    private void OnPressStarted(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled) return;  
        
        Debug.Log("F키 눌림! 코루틴 시작!");
        _coroutine = StartCoroutine(StartPressCoroutine());
    }
    
    private void _OnPressCanceled(InputAction.CallbackContext ctx)
    {
        if (ctx.started || ctx.performed) return;
        
        Debug.Log("F키가 취소됨! 코루틴 취소!");
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            image.fillAmount = 0f;
        }
    }
    */
    
    public void StartInteraction()
    {
        StartPressServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartPressServerRpc()
    {
        if (_isPressClear.Value)
        {
            OnPressCompleted?.Invoke();
            return;
        }
        
        // 서버가 직접 코루틴을 돌리게
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(StartPressCoroutine());
    }
    
    public void StopInteraction()
    {
        StopPressServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopPressServerRpc()
    {
        if (_isPressClear.Value) return;
        
        // 코루틴 강제 종료
        if (_coroutine != null) StopCoroutine(_coroutine);
        
        _coroutine = StartCoroutine(DecreasePressCoroutine());
    }

    private IEnumerator StartPressCoroutine()
    {
        _holdTime = 10f;
        
        while (_currentTime.Value < _holdTime)
        {
            _currentTime.Value += Time.deltaTime;
            
            yield return null;
        }
        
        OnPressCompleted?.Invoke();
        _isPressClear.Value = true;
    }
    
    private IEnumerator DecreasePressCoroutine()
    {
        _holdTime = 10f;
        
        while (_currentTime.Value > 0f)
        {
            _currentTime.Value -= Time.deltaTime;

            if (_currentTime.Value < 0f) _currentTime.Value = 0f;
            
            yield return null;
        }
        
        // OnPressCanceled?.Invoke();
    }
    
    
    private void OnUpdateFillAmountUI(float previousValue, float newValue)
    {
        image.fillAmount = newValue / _holdTime;
    }
    
}
