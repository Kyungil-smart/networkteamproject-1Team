using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimation : MonoBehaviour
    {
        // 애니메이션 파라미터 해싱
        // Animator Param Hashes
        private static readonly int AnimSpeed       = Animator.StringToHash("Speed");
        private static readonly int AnimGrounded    = Animator.StringToHash("Grounded");
        private static readonly int AnimJump        = Animator.StringToHash("Jump");
        private static readonly int AnimFreeFall    = Animator.StringToHash("FreeFall");
        private static readonly int AnimMotionSpeed = Animator.StringToHash("MotionSpeed");
        
        private Animator _animator;
        private PlayerMovement _movement;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _movement = GetComponent<PlayerMovement>();
        }

        void Update()
        {
            if (_movement == null) return;
            
            // 이동 관련 ( Layer 0 )
            _animator.SetFloat(AnimSpeed, _movement.CurrentSpeed);
            _animator.SetFloat(AnimMotionSpeed, _movement.MotionSpeed);
            _animator.SetBool(AnimGrounded, _movement.IsGrounded);
            
            if (_movement.JustJumped)
                _animator.SetBool(AnimJump, true);
            else if (_movement.IsGrounded)
                _animator.SetBool(AnimJump, false);
            
            // 땅에 닿아있지 않으며 점프 상승 중이 아닌 아래로 떨어질 때만 낙하로 판정
            _animator.SetBool(AnimFreeFall, !_movement.IsGrounded && _movement.VerticalVelocity < 0.0f);
        }
        
        /// <summary>
        /// 전투 스크립트에서 상태 변경에 따라 호출
        /// NetworkVariable.OnValueChanged 자동 동기화
        /// Layer 1 (상체) 트리거 처리.
        /// </summary>
        public void PlayStateAnimation()
        {
            // TODO: 상태에 따른 애니메이터 처리 (Switch 문 활용)
            // Normal, 공격, 피격, 사망 상태.
        }
    }
}
