using UnityEngine;

namespace Enemy
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

        private ENEMY_STATE _currentState = ENEMY_STATE.IDLE;
        private Transform _targetTransform;

        public void Init(Transform targetTransform)
        {
            _targetTransform = targetTransform;
            _currentState = ENEMY_STATE.TRACKING;

            EnemyManagerProvider.Instance.RegisterEnemy(this);
        }

        public void Tick(float tDeltaTime)
        {
            if ( _currentState == ENEMY_STATE.TRACKING )
            {
                MoveTowardsTarget(tDeltaTime);
            }
        }

        private void MoveTowardsTarget(float deltaTime)
        {
            if(_targetTransform == null)
            {
                return;
            }

            Vector3 tDir = (_targetTransform.position - transform.position).normalized;

            transform.position += tDir * (_moveSpeed * deltaTime);
        }

        public void SetDead()
        {
            _currentState = ENEMY_STATE.DEAD;
            gameObject.SetActive(false);

            EnemyManagerProvider.Instance.UnregisterEnemy(this);
        }
    }
}

