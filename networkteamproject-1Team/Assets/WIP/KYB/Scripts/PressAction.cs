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

        if (!_isPressClear.Value)
        {
            OnPressCompleted?.Invoke();
        }
        
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
    }
    
    
    private void OnUpdateFillAmountUI(float previousValue, float newValue)
    {
        image.fillAmount = newValue / _holdTime;
    }
    
}
