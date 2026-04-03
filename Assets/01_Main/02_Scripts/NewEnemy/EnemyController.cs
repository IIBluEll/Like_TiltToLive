using DG.Tweening;
using HM.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace HM.NewEnemy
{
    public enum ENEMY_STATE
    {
        SPAWNING,
        TRACKING,
        DEAD
    }

    public enum MOVE_TYPE
    {
        SWARM,
        Linear
    }

    public class EnemyController : MonoBehaviour
    {
        [Header("Animation Setting")]
        [SerializeField] private float _spawningAnimationSpeed = 1.2f;
        [SerializeField] private float _flashSpeed = 0.5f;

        [SerializeField] private SpriteRenderer _spriteRender;
        private Collider2D _collider;

        [Header("Life Settings")]
        [SerializeField] private float _maxLifespan = 60f; // 최대 생존 시간
        private float _lifeTimer;

        private Transform _thisTransform;
        private Transform _playerTransform;
        private float _moveSpeed;

        public ENEMY_STATE CurrentState { get; private set; }
        public bool IsDead { get; private set; }

        // 군집 패턴 연산을 위한 캐싱
        private readonly float SEPARATION_RADIUS = 0.3f * 0.3f;
        private readonly float SEPARATION_WEIGHT = 0.5f;

        private MOVE_TYPE _moveType;
        private Vector3 _fixedDirection;

        private void Awake()
        {
            _thisTransform = transform;
            _collider = GetComponent<Collider2D>();
        }

        public void Init(Transform playerTransform , float moveSpeed, MOVE_TYPE moveType = MOVE_TYPE.SWARM , Vector3 fixedDir = default)
        {
            _playerTransform = playerTransform;
            _moveSpeed = moveSpeed;
            IsDead = false;

            _thisTransform.DOKill();
            _spriteRender.DOKill();
            _collider.enabled = false;

            PlaySpawnAnimation();

            _lifeTimer = 0f;
            PlaySpawnAnimation();

            _moveType = moveType;
            _fixedDirection = fixedDir == default ? Vector3.down : fixedDir.normalized;
        }

        private void PlaySpawnAnimation()
        {
            _thisTransform.localScale = Vector3.zero;

            if ( _spriteRender != null )
            {
                Color tStartColor = _spriteRender.color;
                tStartColor.a = 1f;
                _spriteRender.color = tStartColor;

                _spriteRender.DOFade(0f , _flashSpeed).SetLoops(-1 , LoopType.Yoyo).SetEase(Ease.InOutSine);
            }

            _thisTransform.DOScale(Vector3.one , _spawningAnimationSpeed).SetEase(Ease.OutBack).OnComplete(OnSpawnAnimationFinished);
        }

        private void OnSpawnAnimationFinished()
        {
            _spriteRender.DOKill();
            Color tFinalColor = _spriteRender.color;
            tFinalColor.a = 1f;
            _spriteRender.color = tFinalColor;

            CurrentState = ENEMY_STATE.TRACKING;
            _collider.enabled = true;
        }

        public void Tick(float deltaTime , IReadOnlyList<EnemyController> allEnemies)
        {
            if ( IsDead || _playerTransform == null || CurrentState == ENEMY_STATE.SPAWNING )
            {
                return;
            }

            //수명 체크
            _lifeTimer += deltaTime;
            if ( _lifeTimer >= _maxLifespan )
            {
                Despawn();
                return;
            }

            // 맵 이탈시 제거
            if ( !ScreenBoundary.KillArea.Contains(_thisTransform.position) )
            {
                Despawn();
                return;
            }

            if ( _moveType == MOVE_TYPE.Linear )
            {
                //주어진 방향으로 직진
                _thisTransform.position += _fixedDirection * ( _moveSpeed * deltaTime );
            }
            else
            {
                Vector3 tTargetDir = (_playerTransform.position - _thisTransform.position).normalized;
                _thisTransform.position += tTargetDir * ( _moveSpeed * deltaTime );

                // 주변 동료들을 밀어내는 군집패턴
                #region 군집 패턴

                Vector3 tCurrentPos = _thisTransform.position;
                Vector3 tSeparationDir = Vector3.zero;

                int tCount = allEnemies.Count;
                for ( int i = 0; i < tCount; i++ )
                {
                    EnemyController tOther = allEnemies[i];
                    if ( tOther == this || tOther.IsDead || tOther.CurrentState == ENEMY_STATE.SPAWNING )
                    {
                        continue;
                    }

                    Vector3 tDiff = tCurrentPos - tOther._thisTransform.position;
                    float tDistance = tDiff.sqrMagnitude;

                    if ( tDistance < SEPARATION_RADIUS && tDistance > 0.0001f )
                    {
                        tSeparationDir += ( tDiff.normalized / tDistance );
                    }
                }

                Vector3 tFinalDir = (tTargetDir + (tSeparationDir * SEPARATION_WEIGHT)).normalized;

                _thisTransform.position += tFinalDir * ( _moveSpeed * deltaTime );

                #endregion
            }
        }

        public void Die()
        {
            _thisTransform.DOKill();
            _spriteRender.DOKill();

            CurrentState = ENEMY_STATE.DEAD;
            _collider.enabled = false;
            IsDead = true;
        }

        private void Despawn()
        {
            _thisTransform.DOKill();
            _spriteRender.DOKill();

            CurrentState = ENEMY_STATE.DEAD;
            _collider.enabled = false;
            IsDead = true;
        }
    }
}

