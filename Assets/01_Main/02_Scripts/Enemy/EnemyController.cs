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
        //TODO - 시간에 따라 이동 속도 변화 필요
        [SerializeField] private float _moveSpeed = 3f;

        private ENEMY_STATE _currentState = ENEMY_STATE.None;
        private Transform _playerTransform;

        public Action<EnemyController> OnEnemyDead;

        public void InitEnemy(Transform playerTransform)
        {
            _currentState = ENEMY_STATE.TRACKING;
            _playerTransform = playerTransform;
        }

        public void Tick(float deltaTime)
        {
            if(_currentState == ENEMY_STATE.TRACKING)
            {
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

        public void Dead()
        {
            _currentState = ENEMY_STATE.DEAD;

            OnEnemyDead?.Invoke(this);
        }
    }
}

