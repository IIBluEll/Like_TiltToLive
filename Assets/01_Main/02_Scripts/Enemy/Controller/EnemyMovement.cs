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
}
