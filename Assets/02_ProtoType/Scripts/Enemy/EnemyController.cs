using System.Collections.Generic;
using UnityEngine;

namespace ProtoType.Enemy
{
    public enum ENEMY_STATE
    {
        IDLE,
        TRACKING,
        DEAD
    }

    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 3f;

        [Space(5f), Header("Boids Settings")]
        [SerializeField] private float _separationRadius = 0.05f; // 회피를 시작할 반경
        [SerializeField] private float _trackingWeight = 1.0f;   // 플레이어 추적 가중치
        [SerializeField] private float _separationWeight = 0.05f; // 동료 회피 가중치

        private ENEMY_STATE _currentState = ENEMY_STATE.IDLE;
        private Transform _targetTransform;

        public void Init(Transform targetTransform)
        {
            _targetTransform = targetTransform;
            _currentState = ENEMY_STATE.TRACKING;

            EnemyManagerProvider.Instance.RegisterEnemy(this);
        }

        public void Tick(float deltaTime , List<EnemyController> activeEnemies)
        {
            if ( _currentState == ENEMY_STATE.TRACKING )
            {
                MoveTowardsTarget(deltaTime , activeEnemies);
            }
        }

        private void MoveTowardsTarget(float deltaTime , List<EnemyController> activeEnemies)
        {
            if ( _targetTransform == null ) return;

            // 지역 변수 컨벤션
            Vector3 tCurrentPosition = transform.position;
            Vector3 tTargetPosition = _targetTransform.position;

            // 1. 추적(Tracking) 벡터 계산
            Vector3 tTrackingDirection = (tTargetPosition - tCurrentPosition).normalized;

            // 2. 분리(Separation) 벡터 계산
            Vector3 tSeparationVector = Vector3.zero;
            int tNeighborCount = 0;

            // 최적화: Vector3.Distance 대신 제곱근 연산이 없는 sqrMagnitude를 사용하기 위해 반경도 제곱합니다.
            float tSqrRadius = _separationRadius * _separationRadius;

            for ( int tIndex = 0; tIndex < activeEnemies.Count; tIndex++ )
            {
                EnemyController tOtherEnemy = activeEnemies[tIndex];

                // 자기 자신이거나 비활성화된 개체는 연산에서 제외
                if ( tOtherEnemy == this || !tOtherEnemy.gameObject.activeSelf ) continue;

                Vector3 tOtherPosition = tOtherEnemy.transform.position;
                float tSqrDistance = (tCurrentPosition - tOtherPosition).sqrMagnitude;

                // 지정된 반경 내에 다른 적이 있다면 밀어내는 벡터 연산
                if ( tSqrDistance < tSqrRadius && tSqrDistance > 0.001f )
                {
                    Vector3 tPushDirection = tCurrentPosition - tOtherPosition;

                    // 거리가 가까울수록 더 강하게 밀어내기 위해 가중치 적용 (반비례)
                    tSeparationVector += tPushDirection.normalized / Mathf.Sqrt(tSqrDistance);
                    tNeighborCount++;
                }
            }

            if ( tNeighborCount > 0 )
            {
                // 이웃한 적들의 밀어내는 힘의 평균을 구함
                tSeparationVector /= tNeighborCount;
            }

            // 3. 최종 이동 방향 = (추적 방향 * 추적 가중치) + (분리 방향 * 회피 가중치)
            Vector3 tFinalDirection = (tTrackingDirection * _trackingWeight + tSeparationVector * _separationWeight).normalized;

            transform.position += tFinalDirection * ( _moveSpeed * deltaTime );
        }

        public void SetDead()
        {
            _currentState = ENEMY_STATE.DEAD;
            gameObject.SetActive(false);

            EnemyManagerProvider.Instance.UnregisterEnemy(this);
        }
    }
}

