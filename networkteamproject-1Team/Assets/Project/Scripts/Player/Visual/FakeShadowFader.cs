// 캐릭터에 부착, Decal Projector 자식 참조
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FakeShadowFader : MonoBehaviour
{
    [SerializeField] DecalProjector _decal;
    [SerializeField] float _maxHeight = 1f;
    [SerializeField] LayerMask _groundLayer;
    
    // 바닥과 거리에 따라 데칼을 페이드해서 자연스러운 Fake Shadow 연출
    void LateUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down,
                out RaycastHit hit, _maxHeight, _groundLayer))
        {
            float distance = hit.distance;
            _decal.fadeFactor = 1f - (distance / _maxHeight);
        }
        else
        {
            _decal.fadeFactor = 0f;
        }
    }
}