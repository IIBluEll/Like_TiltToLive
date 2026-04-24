using Cysharp.Threading.Tasks;
using HM.Manager;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace HM.NewEnemy
{
    /// <summary>
    /// 최대 2000개의 객체 풀링과 O(1) Swap-back 삭제 연산을 통해 가비지(GC) 발생을 원천 차단하는 대규모 적 군집 제어 클래스
    /// </summary>
    public class EnemyManagement : MonoBehaviour
    {
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private Transform _playerTransform;

        private readonly List<EnemyController> _activeEnemies = new(2000);
        private readonly List<Vector3> _spawnPositionBuffer = new(100);

        private GameDifficultyManager _difficultyManager;
        private GameStateManager _gameStateManager;

        private CancellationTokenSource _cts;
        private bool _isSpawning;

        public async UniTask Init(GameDifficultyManager difficultyManager , GameStateManager gameStateManager)
        {
            _difficultyManager = difficultyManager;
            _gameStateManager = gameStateManager;

            await EnemyObjectPoolProvider.Instance.CreatePool_async(_enemyPrefab, 2000);
        }

        private void Update()
        {
            float tDeltaTime = Time.deltaTime;

            for(int i = _activeEnemies.Count -1; i >= 0; i-- )
            {
                EnemyController tEnemy = _activeEnemies[i];
                if(tEnemy.IsDead)
                {
                    EnemyObjectPoolProvider.Instance.ReturnObject(tEnemy.gameObject);

                    // Swap Back
                    int tLastIndex = _activeEnemies.Count - 1;
                    _activeEnemies[i] = _activeEnemies[tLastIndex];
                    _activeEnemies.RemoveAt(tLastIndex);
                    continue;
                }

                tEnemy.Tick(tDeltaTime, _activeEnemies);
            }
        }

        public void StartSpawn()
        {
            if(!_isSpawning)
            {
                _isSpawning = true;
                _cts = new CancellationTokenSource();
                SpawnRoutine_async(_cts.Token).Forget();
            }
        }

        public void StopSpawn()
        {
            _isSpawning = false;
            if ( _cts != null )
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        private async UniTaskVoid SpawnRoutine_async(CancellationToken token)
        {
            while ( _isSpawning && !token.IsCancellationRequested )
            {
                if ( _gameStateManager.CurrentState == GAME_STATE.PLAYING )
                {
                    int tEnemyCount = _difficultyManager.CurrentPatternEnemyCount;
                    float tSpacing = _difficultyManager.CurrentPatternSpacing;
                    float tSpeed = _difficultyManager.CurrentEnemySpeed;

                    // 0: 원형 패턴(플레이어 추적), 1: 일자형 벽 패턴(직진)
                    int tPatternType = Random.Range(0, 4);

                    if ( tPatternType == 0 )
                    {
                        // [패턴 1] 플레이어 주변 원형 스폰 + 스웜 추적 이동
                        Vector3 tSpawnCenter = _playerTransform != null ? _playerTransform.position : Vector3.zero;
                        SpawnPattern(new CirclePattern() , tEnemyCount , tSpawnCenter , tSpacing , tSpeed , MOVE_TYPE.SWARM , Vector3.zero);
                    }
                    else if (tPatternType == 1)
                    {
                        // [패턴 2] 4방향 벽 스폰 + 반대편으로 직진 이동
                        int tEdge = Random.Range(0, 4); // 0: 위, 1: 아래, 2: 왼쪽, 3: 오른쪽
                        Rect tBounds = HM.Manager.ScreenBoundary.PlayableArea;
                        float tOffScreenMargin = 1.5f;

                        IEnemySpawnPattern tSelectedPattern = null;
                        Vector3 tSpawnCenter = Vector3.zero;
                        Vector3 tDirection = Vector3.zero;

                        switch ( tEdge )
                        {
                            case 0: // 위에서 아래로
                                tSpawnCenter = new Vector3(tBounds.center.x , tBounds.yMax + tOffScreenMargin , 0f);
                                tDirection = Vector3.down;
                                tSelectedPattern = new HorizontalLinePattern();
                                break;
                            case 1: // 아래에서 위로
                                tSpawnCenter = new Vector3(tBounds.center.x , tBounds.yMin - tOffScreenMargin , 0f);
                                tDirection = Vector3.up;
                                tSelectedPattern = new HorizontalLinePattern();
                                break;
                            case 2: // 왼쪽에서 오른쪽으로
                                tSpawnCenter = new Vector3(tBounds.xMin - tOffScreenMargin , tBounds.center.y , 0f);
                                tDirection = Vector3.right;
                                tSelectedPattern = new VerticalLinePattern();
                                break;
                            case 3: // 오른쪽에서 왼쪽으로
                                tSpawnCenter = new Vector3(tBounds.xMax + tOffScreenMargin , tBounds.center.y , 0f);
                                tDirection = Vector3.left;
                                tSelectedPattern = new VerticalLinePattern();
                                break;
                        }

                        SpawnPattern(tSelectedPattern , tEnemyCount , tSpawnCenter , tSpacing , tSpeed * 0.8f , MOVE_TYPE.Linear , tDirection);
                    }
                    else
                    {
                        // 랜덤 생성
                        SpawnPattern(new RandomPattern() , tEnemyCount , Vector3.zero , tSpacing , tSpeed , MOVE_TYPE.SWARM , Vector3.zero);
                    }

                    int tSpawnInterval = _difficultyManager.CurrentSpawnInterval;
                    await UniTask.Delay(tSpawnInterval , cancellationToken: token);
                }
            }
        }
        public void SpawnEnemy(Vector3 spawnPos, float speed, MOVE_TYPE moveType = MOVE_TYPE.SWARM , Vector3 dir = default)
        {
            GameObject tObj = EnemyObjectPoolProvider.Instance.GetObject(_enemyPrefab);
            tObj.transform.position = spawnPos;

            EnemyController tEnemyController = tObj.GetComponent<EnemyController>();

            if(tEnemyController != null)
            {
                tEnemyController.Init(_playerTransform, speed , moveType , dir);
                _activeEnemies.Add(tEnemyController);
            }
        }

        public void SpawnPattern(IEnemySpawnPattern pattern , int count , Vector3 center , float spacing , float speed , MOVE_TYPE moveType , Vector3 dir)
        {
            pattern.CalculatePosition(count , center , spacing , _spawnPositionBuffer);

            for ( int i = 0; i < _spawnPositionBuffer.Count; i++ )
            {
                SpawnEnemy(_spawnPositionBuffer[ i ] , speed , moveType , dir);
            }
        }

        public void ClearAllEnemies()
        {
            StopSpawn();

            for(int i = _activeEnemies.Count -1; i >= 0; i-- )
            {
                EnemyObjectPoolProvider.Instance.ReturnObject(_activeEnemies[i].gameObject);
            }

            _activeEnemies.Clear();
        }
    }
}

