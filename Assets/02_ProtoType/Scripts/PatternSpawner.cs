using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProtoType.Enemy
{
    public class PatternSpawner : MonoBehaviour
    {
        // Inspector에서 테스트용으로 할당할 임시 프리팹 (추후 풀링 유틸로 대체)
        [SerializeField] private GameObject _enemyPrefab;

        private float _warningDuration = 1.5f;
        private Dictionary<PATTERN_TYPE, IEnemyPattern> _patternCalculators;

        public int enemyCount = 12;
        public float spacing = 2f;

        private void Awake()
        {
            // 패턴 계산기 캐싱
            _patternCalculators = new Dictionary<PATTERN_TYPE , IEnemyPattern>
        {
            { PATTERN_TYPE.CIRCLE, new CirclePattern() },
            { PATTERN_TYPE.CROSS, new CrossPattern() }
        };
        }

        private void Update()
        {
            // 프로토타입 테스트용 입력 (스페이스바를 누르면 원형 패턴 스폰)
            if ( Input.GetKeyDown(KeyCode.F2) )
            {
                // 임의의 스폰 구역 중심점 설정 (0, 0, 0)
                Vector3 tSpawnZoneCenter = Vector3.zero;
                StartPatternSpawn_cor(PATTERN_TYPE.CIRCLE , tSpawnZoneCenter , enemyCount , spacing);
            }
            if ( Input.GetKeyDown(KeyCode.F3) )
            {
                Vector3 tSpawnZoneCenter = Vector3.zero;
                StartPatternSpawn_cor(PATTERN_TYPE.CROSS , tSpawnZoneCenter , enemyCount , spacing);
            }
        }

        public void StartPatternSpawn_cor(PATTERN_TYPE patternType , Vector3 spawnZoneCenter , int enemyCount , float spacing)
        {
            StartCoroutine(ExecutePatternSpawn_cor(patternType , spawnZoneCenter , enemyCount , spacing));
        }

        private IEnumerator ExecutePatternSpawn_cor(PATTERN_TYPE patternType , Vector3 spawnZoneCenter , int enemyCount , float spacing)
        {
            // 1. 구역 경고 (디버그 라인으로 임시 대체)
            Debug.Log($"[경고] {spawnZoneCenter} 위치에 {patternType} 패턴 스폰 임박!");

            // 2. 대기 시간
            yield return new WaitForSeconds(_warningDuration);

            // 3. 스폰 실행
            SpawnEnemies(patternType , spawnZoneCenter , enemyCount , spacing);
        }

        private void SpawnEnemies(PATTERN_TYPE patternType , Vector3 spawnZoneCenter , int enemyCount , float spacing)
        {
            if ( _patternCalculators.TryGetValue(patternType , out IEnemyPattern tPattern) )
            {
                Vector3[] tOffsets = tPattern.CalculateOffsets(enemyCount, spacing);

                for ( int tIndex = 0; tIndex < tOffsets.Length; tIndex++ )
                {
                    // [참고] 현재는 Instantiate를 사용하지만, 추후 유틸 클래스의 ObjectPool.Get()으로 변경해야 합니다.
                    GameObject tEnemy = Instantiate(_enemyPrefab);

                    Vector3 tTargetPosition = spawnZoneCenter + tOffsets[tIndex];
                    tEnemy.transform.position = tTargetPosition;

                    // 스폰 확인용 디버그 로그
                    Debug.Log($"적 {tIndex} 스폰 완료: 위치 {tTargetPosition}");
                }
            }
        }
    }

}

