using Cysharp.Threading.Tasks;
using HM.Manager;
using System;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// 사용자 입력(마우스/자이로)을 받아 구면 선형 보간(Slerp)으로 부드러운 이동과 회전을 처리하고, 생존 판별을 담당하는 컨트롤러
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _rotationSpeed = 15f;

        private GameStateManager _gameStateManager;
        private IInputProvider _inputProvider;
        private Rigidbody2D _rigid;
        private SpriteRenderer _spriteRender;

        public Action OnPlayerDead;
        private bool _isDead = false;
        private bool _isInvincible = false;

        public void Init(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _rigid.gravityScale = 0f;
            _rigid.freezeRotation = true;
            _rigid.bodyType = RigidbodyType2D.Kinematic;

            _spriteRender = GetComponent<SpriteRenderer>();
            _spriteRender.enabled = false;

#if UNITY_ANDROID && !UNITY_EDITOR
            _inputProvider = gameObject.AddComponent<GyroInput>();   
#else
            _inputProvider = gameObject.AddComponent<MouseInput>();
#endif
        }

        private void FixedUpdate()
        {
            if (_isDead || (_gameStateManager != null && _gameStateManager.CurrentState != GAME_STATE.PLAYING))
            {
                return;
            }

            if (_inputProvider != null)
            {
                Vector2 tCurrentPos = _rigid.position;
                // 인터페이스를 통해 목표 지점을 받아옵니다.
                Vector2 tTargetPos = _inputProvider.GetTargetPosition(tCurrentPos);
                MoveToTarget(tCurrentPos, tTargetPos);
            }
        }

        private void MoveToTarget(Vector2 currentPos, Vector2 targetPos)
        {
            Vector2 tNextPos = Vector2.Lerp(currentPos, targetPos, _moveSpeed * Time.fixedDeltaTime);
            
            tNextPos.x = Mathf.Clamp(tNextPos.x , ScreenBoundary.PlayableArea.xMin , ScreenBoundary.PlayableArea.xMax);
            tNextPos.y = Mathf.Clamp(tNextPos.y , ScreenBoundary.PlayableArea.yMin , ScreenBoundary.PlayableArea.yMax);

            _rigid.MovePosition(tNextPos);

            // 방향 회전
            Vector2 tDir = targetPos - currentPos;

            if(tDir.sqrMagnitude > 0.03f)
            {
                float tTargetAngle = MathF.Atan2(tDir.y,tDir.x) * Mathf.Rad2Deg - 90f;

                Quaternion tTargetRotation = Quaternion.Euler(0f, 0f, tTargetAngle);
                transform.rotation = Quaternion.Slerp(transform.rotation, tTargetRotation, _rotationSpeed * Time.fixedDeltaTime);
            }
        }

        public void SpriteRenderOn()
        {
            _spriteRender.enabled = true;
        }

        public void SpriteRenderOff()
        {
            _spriteRender.enabled = false;
        }

        public void ResetPlayer()
        {
            _isDead = false;
            _isInvincible = false;
            transform.position = Vector3.zero;
        }

        public void CalibrateInput()
        {
            if (_inputProvider != null)
            {
                _inputProvider.Calibrate();
            }
        }

        public void SetInvincibleState(bool state)
        {
            _isInvincible = state;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isDead)
            {
                return;
            }
           
            if (collision.CompareTag("Enemy"))
            {
                // 보호막일때
                if(_isInvincible)
                {
                    return;
                }

                _isDead = true;
                OnPlayerDead?.Invoke();
            }
        }
    }
}