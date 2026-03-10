using Cysharp.Threading.Tasks;
using HM.Enemy.Controller;
using System.Collections.Generic;
using UnityEngine;

namespace HM.Enemy.System
{
    public class EnemyManagement : MonoBehaviour
    {
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private Transform _playerTransform;

        // TODO 차후 최상단 매니저가 시간을 재서 그에 맞춰 스폰 주기 변경
        [SerializeField] private int _spawnInterval = 500;
        [SerializeField] private float _spawnRadius = 15f;

        [Header("Spawn Area")]
        [SerializeField] private Vector2 _spawnAreaMin;
        [SerializeField] private Vector2 _spawnAreaMax;

        private bool _isSpawning = false;

        private readonly List<EnemyController> _activeEnemies = new();

        private void Start()
        {
            EnemyObjectPoolProvider.Instance.CreatePool(_enemyPrefab , 1000 , transform);
            StartSpawn();
        }

        private void Update()
        {
            float tDeltaTime = Time.deltaTime;

            for ( int i = 0; i < _activeEnemies.Count; i++ )
            {
                _activeEnemies[ i ].Tick(tDeltaTime);
            }
        }

        #region 스폰 로직

        public void StartSpawn()
        {
            if(!_isSpawning)
            {
                _isSpawning = true;

                SpawnRoutine_Async().Forget();
            }
        }

        public void StopSpawn()
        {
            _isSpawning = false;
        }

        private async UniTaskVoid SpawnRoutine_Async()
        {
            while(_isSpawning)
            {
                SpawnSingleEnemy();
                await UniTask.Delay(_spawnInterval);
            }
        }

        private void SpawnSingleEnemy()
        {
            GameObject tEnemyObject = EnemyObjectPoolProvider.Instance.GetObject(_enemyPrefab);

            EnemyController tEnemyController = tEnemyObject.GetComponent<EnemyController>();

            if(tEnemyController != null)
            {
                Vector3 tSpawnPos = GetRandomSpawnPos();
                tEnemyObject.transform.position = tSpawnPos;

                tEnemyController.InitEnemy(_playerTransform);
                RegisterEnemy(tEnemyController);
                tEnemyController.OnEnemyDead += UnregisterEnemy;
            }
        }

        private Vector3 GetRandomSpawnPos()
        {
            Vector3 tPos = Vector3.zero;

            float tRandomX = Random.Range(_spawnAreaMin.x, _spawnAreaMax.x);
            float tRandomY = Random.Range(_spawnAreaMin.y, _spawnAreaMax.y);
            tPos = new Vector3(tRandomX , tRandomY , 0f);

            return tPos;
        }
        #endregion

        #region 적 관리 로직

        public void RegisterEnemy(EnemyController enemy)
        {
            if ( !_activeEnemies.Contains(enemy) )
            {
                _activeEnemies.Add(enemy);
            }
        }

        public void UnregisterEnemy(EnemyController enemy)
        {
            if ( _activeEnemies.Contains(enemy) )
            {
                _activeEnemies.Remove(enemy);
                EnemyObjectPoolProvider.Instance.ReturnObject(enemy.gameObject);
            }
        }

        #endregion
    }
}

