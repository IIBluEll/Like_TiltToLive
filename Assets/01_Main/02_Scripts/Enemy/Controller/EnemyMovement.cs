using UnityEngine;

namespace HM.Enemy.Controller
{
    public interface IEnemyMovement
    {
        void Move(Transform enemyTransform , Transform playerTransform , float moveSpeed , float deltaTime);
    }

    // 플레이어 추적 이동
    public class TrackingMovement : IEnemyMovement
    {
        public void Move(Transform enemyTransform , Transform playerTransform , float moveSpeed , float deltaTime)
        {
            if ( playerTransform == null ) return;
            Vector3 tDirection = (playerTransform.position - enemyTransform.position).normalized;
            enemyTransform.position += tDirection * ( moveSpeed * deltaTime );
        }
    }

    // 군집 이동
    public class SwarmMovement : IEnemyMovement
    {
        private readonly float _separationRadius = 0.25f;
        private readonly float _separationWeight = 0.3f;

        public void Move(Transform enemyTransform , Transform playerTransform , float moveSpeed , float deltaTime)
        {
            if ( playerTransform == null ) return;

            Vector3 tCurrentPos = enemyTransform.position;
            Vector3 tTargetDir = (playerTransform.position - tCurrentPos).normalized;

            Vector3 tSeparationDir = Vector3.zero;
            Collider2D[] tColliders = Physics2D.OverlapCircleAll(tCurrentPos, _separationRadius);

            foreach ( Collider2D tCollider in tColliders )
            {
                if ( tCollider.CompareTag("Enemy") && tCollider.transform != enemyTransform )
                {
                    Vector3 tDiff = tCurrentPos - tCollider.transform.position;
                    tSeparationDir += tDiff.normalized / tDiff.magnitude;
                }
            }

            Vector3 tFinalDir = (tTargetDir + (tSeparationDir * _separationWeight)).normalized;
            enemyTransform.position += tFinalDir * ( moveSpeed * deltaTime );
        }
    }

    // 직선 이동
    public class LinearMovement : IEnemyMovement
    {
        private readonly Vector3 _moveDirection;

        public LinearMovement(Vector3 direction)
        {
            _moveDirection = direction.normalized;
        }

        public void Move(Transform enemyTransform , Transform playerTransform , float moveSpeed , float deltaTime)
        {
            enemyTransform.position += _moveDirection * ( moveSpeed * deltaTime );
        }
    }

    public class SinusoidalMovement : IEnemyMovement
    {
        private readonly Vector3 _baseDirection;
        private readonly float _frequency;
        private readonly float _amplitude;
        private float _time;

        public SinusoidalMovement(Vector3 baseDirection , float frequency , float amplitude)
        {
            _baseDirection = baseDirection.normalized;
            _frequency = frequency;
            _amplitude = amplitude;
            _time = 0f;
        }

        public void Move(Transform enemyTransform , Transform playerTransform , float moveSpeed , float deltaTime)
        {
            _time += deltaTime;
            Vector3 tForwardMove = _baseDirection * (moveSpeed * deltaTime);

            // 전진 방향의 수직 벡터(직교 벡터)를 구하여 좌우 진동 계산
            Vector3 tRight = new Vector3(-_baseDirection.y, _baseDirection.x, 0f);
            Vector3 tOscillation = tRight * (Mathf.Sin(_time * _frequency) * _amplitude * deltaTime);

            enemyTransform.position += tForwardMove + tOscillation;
        }
    }

    public class DashMovement : IEnemyMovement
    {
        private float _timer = 0f;
        private readonly float _dashInterval = 5f;
        private readonly float _dashDuration = 1f;
        private readonly float _dashMultiplier = 4f;

        private Vector3 _dashDirection = Vector3.zero;
        private bool _isDashing = false;

        public void Move(Transform enemyTransform , Transform playerTransform , float moveSpeed , float deltaTime)
        {
            if ( playerTransform == null ) return;

            _timer += deltaTime;

            if ( _isDashing )
            {
                // 돌진 상태: 고정된 방향으로 빠르게 이동
                enemyTransform.position += _dashDirection * ( moveSpeed * _dashMultiplier * deltaTime );

                if ( _timer >= _dashDuration )
                {
                    _isDashing = false;
                    _timer = 0f;
                }
            }
            else
            {
                // 대기 상태: 플레이어를 쳐다보며 아주 느리게 이동 (압박감 부여)
                Vector3 tTargetDir = (playerTransform.position - enemyTransform.position).normalized;
                enemyTransform.position += tTargetDir * ( moveSpeed * 0.1f * deltaTime );

                if ( _timer >= _dashInterval )
                {
                    _isDashing = true;
                    _timer = 0f;
                    _dashDirection = tTargetDir; // 돌진 순간의 방향을 고정
                }
            }
        }
    }
}
