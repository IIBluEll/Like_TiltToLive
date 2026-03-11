using Cysharp.Threading.Tasks;
using HM.Enemy.Controller;
using HM.Enemy.Pattern;
using HM.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace HM.Enemy.System
{
    public class EnemyManagement : MonoBehaviour
    {
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private Transform _playerTransform;

        [Header("Spawn Area")]
        [SerializeField] private Vector2 _spawnAreaMin;
        [SerializeField] private Vector2 _spawnAreaMax;

        private bool _isSpawning = false;

        private readonly List<EnemyController> _activeEnemies = new();

        private GameDifficultyManager _gameDifficultyManager;
        private GameStateManager _gameStateManager;
        private EnemyPatternPool _enemyPatternPool;

        public void Init(GameDifficultyManager gameDifficultyManager, GameStateManager gameStateManager)
        {
            _gameDifficultyManager = gameDifficultyManager;
            _gameStateManager = gameStateManager;

            _enemyPatternPool = new EnemyPatternPool(_spawnAreaMin, _spawnAreaMax);

            EnemyObjectPoolProvider.Instance.CreatePool(_enemyPrefab , 1000 , transform);
            StartSpawn();
        }
        
        private void Update()
        {
            if(_gameStateManager == null || _gameStateManager.CurrentState != GAME_STATE.PLAYING)
            {
                return;
            }

            float tDeltaTime = Time.deltaTime;
            float tCurrentSpeed = _gameDifficultyManager.CurrentEnemySpeed;

            for ( int i = 0; i < _activeEnemies.Count; i++ )
            {
                _activeEnemies[ i ].Tick(tDeltaTime, tCurrentSpeed);
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
                if(_gameStateManager.CurrentState == GAME_STATE.PLAYING)
                {
                    SpawnPatternEnemy();
                }

                int tSpawnInterval = _gameDifficultyManager.CurrentSpawnInterval;
                await UniTask.Delay(tSpawnInterval);
            }
        }

        private void SpawnPatternEnemy()
        {
            float tProgress = _gameDifficultyManager.CurrentProgress;
            int tEnemyCount = _gameDifficultyManager.CurrentPatternEnemyCount;
            float tSpacing = _gameDifficultyManager.CurrentPatternSpacing;

            IEnemyPattern tPattern = _enemyPatternPool.GetPattern(tProgress);
            Vector3 tCenterPos = _playerTransform != null ? _playerTransform.position : Vector3.zero;

            Vector3[] tPos = tPattern.GetPatternPos(tEnemyCount, tSpacing, tCenterPos); 

            for(int i = 0; i < tPos.Length; i++ )
            {
                GameObject tEnemyObj = EnemyObjectPoolProvider.Instance.GetObject(_enemyPrefab);
                EnemyController tEnemyController = tEnemyObj.GetComponent<EnemyController>();

                if(tEnemyController != null)
                {
                    tEnemyObj.transform.position = tPos[i];

                    tEnemyController.InitEnemy(_playerTransform);
                    RegisterEnemy(tEnemyController);

                    tEnemyController.OnEnemyDead -= UnregisterEnemy;
                    tEnemyController.OnEnemyDead += UnregisterEnemy;
                }
            }
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

