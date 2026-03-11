using System.Collections;
using UnityEngine;

namespace ProtoType.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private float _spawnInterval = 0.5f;
        [SerializeField] private float _spawnRadius = 15f;

        private bool _isSpawning = false;

        private void Start()
        {
            ObjectPoolProvider.Instance.CreatePool(_enemyPrefab , 1000 , transform);

            StartSpawn();
        }

        public void StartSpawn()
        {
            if ( !_isSpawning )
            {
                _isSpawning = true;
                StartCoroutine(SpawnRoutine_cor());
            }
        }

        public void StopSpawn()
        {
            _isSpawning = false;
        }

        // 코루틴 함수 컨벤션 적용
        private IEnumerator SpawnRoutine_cor()
        {
            while ( _isSpawning )
            {
                SpawnSingleEnemy();
                yield return new WaitForSeconds(_spawnInterval);
            }
        }

        private void SpawnSingleEnemy()
        {
            if ( _playerTransform == null ) return;

            // 1. 오브젝트 풀에서 적 객체를 가져옵니다.
            GameObject tEnemyObject = ObjectPoolProvider.Instance.GetObject(_enemyPrefab);

            // 2. 적 컨트롤러 컴포넌트를 가져옵니다.
            EnemyController tEnemyController = tEnemyObject.GetComponent<EnemyController>();

            if ( tEnemyController != null )
            {
                // 3. 스폰 위치 계산 (플레이어 기준 특정 반경의 원주 상 무작위 위치)
                Vector2 tRandomDirection = Random.insideUnitCircle.normalized;
                Vector3 tSpawnPosition = _playerTransform.position + new Vector3(tRandomDirection.x, tRandomDirection.y, 0f) * _spawnRadius;

                tEnemyObject.transform.position = tSpawnPosition;

                // 4. 적 초기화 (추적 대상 할당 및 매니저에 연산 등록)
                tEnemyController.Init(_playerTransform);
            }
        }
    }

}
