using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 10f;

        private IInputProvider _inputProvider;
        private Rigidbody2D _rigid;
        
        public Action OnPlayerDead;
        private bool _isDead = false;

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _rigid.gravityScale = 0f;
            _rigid.freezeRotation = true;
            _rigid.bodyType = RigidbodyType2D.Kinematic;

#if UNITY_ANDROID && !UNITY_EDITOR
            _inputProvider = gameObject.AddComponent<GyroInput>();            
#else
            _inputProvider = gameObject.AddComponent<MouseInput>();
#endif
        }

        private void FixedUpdate()
        {
            if (_isDead)
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
            // 기존에 잘 작동하던 부드러운 Lerp 이동 로직을 그대로 사용합니다.
            Vector2 tNextPos = Vector2.Lerp(currentPos, targetPos, _moveSpeed * Time.fixedDeltaTime);
            _rigid.MovePosition(tNextPos);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_isDead)
            {
                return;
            }

            if (collision.CompareTag("Enemy"))
            {
                //_isDead = true;
                //OnPlayerDead?.Invoke();
            }
        }
    }
}