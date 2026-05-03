using UnityEngine;
using UnityEngine.Audio;

// AnimationEvent를 받아 사운드 재생
public class AcPlayerSound : MonoBehaviour
{
    [SerializeField] AudioResource _footStep;

    private void OnFootstep(AnimationEvent animationEvent)
    {
        AudioManager.Instance.PlaySfxWet(_footStep, this.transform.position);
    }

    private void OnLand(AnimationEvent animationEvent)
    {

    }
}
