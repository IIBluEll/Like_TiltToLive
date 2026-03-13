using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

namespace HM.Enemy.Controller
{
    public enum ENEMY_STATE
    {
        None,
        SPAWNING,
        TRACKING,
        ICED,       // 얼어붙은 상태
        SCARED,     // 도망가는 상태
        SHOCKED,    // 감전된 상태
        DEAD
    }

    //TODO - 차후 더 높은 지능의 적 추가 필요 (예 : 아이템을 몸으로 막는다, 플레이어 이동 예측 무빙 )
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 3f;

        [Header("Animation Setting")]
        [SerializeField] private float _spawingAnimationSpeed = 1.2f;
        [SerializeField] private float _flashSpeed = 0.5f;
        [SerializeField] private SpriteRenderer _innerSprite;

        [Space(5),Header("상태 표현")]
        [SerializeField] private GameObject _iceOverlay;

        private ENEMY_STATE _currentState = ENEMY_STATE.None;
        private Transform _playerTransform;

        public Action<EnemyController> OnEnemyDead;

        public void InitEnemy(Transform playerTransform)
        {
            _playerTransform = playerTransform;
            _currentState = ENEMY_STATE.SPAWNING;

            transform.DOKill();
            _innerSprite.DOKill();

            if(_iceOverlay != null)
            {
                _iceOverlay.SetActive(false);
            }

            transform.localScale = Vector3.zero;

            Color tStartColor = _innerSprite.color;
            tStartColor.a = 1f;
            _innerSprite.color = tStartColor;

            _innerSprite.DOFade(0f , _flashSpeed)
                .SetLoops(-1 , LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            transform.DOScale(Vector3.one , _spawingAnimationSpeed)
                .SetEase(Ease.OutBack)
                .OnComplete(OnAnimationFinished);
        }

        private void OnAnimationFinished()
        {
            _innerSprite.DOKill();

            Color tFinalColor = _innerSprite.color;
            tFinalColor.a = 1f;
            _innerSprite.color = tFinalColor;

            _currentState = ENEMY_STATE.TRACKING;
        }

        public void Tick(float deltaTime, float moveSpeed)
        {
            if(_currentState == ENEMY_STATE.TRACKING)
            {
                _moveSpeed = moveSpeed;
                MoveTowardsTarget(deltaTime);
            }
        }

        private void MoveTowardsTarget(float deltaTime)
        {
            if ( _playerTransform == null ) return;

            Vector3 tCurrentPosition = transform.position;
            Vector3 tTargetPosition = _playerTransform.position;
            Vector3 tDirection = (tTargetPosition - tCurrentPosition).normalized;

            transform.position += tDirection * ( _moveSpeed * deltaTime );
        }

        public async UniTaskVoid ApplyIceEffect_async(float duration)
        {
            if ( _currentState == ENEMY_STATE.DEAD || _currentState == ENEMY_STATE.SPAWNING || _iceOverlay == null )
            {
                return;
            }

            _currentState = ENEMY_STATE.ICED;

            _iceOverlay.SetActive(true);

            await UniTask.Delay(TimeSpan.FromSeconds(duration));

            if ( _currentState == ENEMY_STATE.ICED )
            {
                _currentState = ENEMY_STATE.TRACKING;

                _iceOverlay.SetActive(false);
            }
        }

        public void Dead()
        {
            transform.DOKill();
            _currentState = ENEMY_STATE.DEAD;

            if ( _iceOverlay != null )
            {
                _iceOverlay.SetActive(false);
            }

            OnEnemyDead?.Invoke(this);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ( _currentState == ENEMY_STATE.DEAD || _currentState == ENEMY_STATE.SPAWNING)
            {
                return;
            }

            if(collision.CompareTag("Weapon"))
            {
                Dead();
            }
        }
    }
}

