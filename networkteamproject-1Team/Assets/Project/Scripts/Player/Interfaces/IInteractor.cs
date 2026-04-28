using UnityEngine;

// 상호작용 관련 인터페이스
public interface IInteractor
{
    bool IsInteracting { get; }
    ulong OwnerClientId { get; }
    Vector3 OriginPosition { get; }
    Vector3 OriginForward { get; }
    
    void OnInteractStart();
    void OnInteractTick();
    void OnInteractCancel();
}