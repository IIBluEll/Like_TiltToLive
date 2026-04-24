using UnityEngine;

namespace HM.Manager
{
    /// <summary>
    /// 시간에 따른 적 스폰 주기, 이동 속도 및 패턴 규모를 선형 보간(Lerp)으로 계산하여 게임 난이도의 점진적 상승을 제어하는 매니저
    /// </summary>
    public class GameDifficultyManager : MonoBehaviour
    {
        [Header("Difficulty Settings")]
        [SerializeField] private float _baseEnemySpeed = 3f;
        [SerializeField] private float _maxEnemySpeed = 6f;
        [SerializeField] private int _baseSpawnInterval = 800;
        [SerializeField] private int _minSpawnInterval = 300;
        [SerializeField] private float _timeToMaxDifficulty = 360f;

        [Space(5f), Header("Pattern Setting")]
        [SerializeField] private int _basePatternEnemyCount = 5;
        [SerializeField] private int _maxPatternEnemyCount = 60;
        [SerializeField] private float _BasePatternSpacing = 3f;
        [SerializeField] private float _minPatternSpacing = 1f;

        private float _elapsedTime = 0f;

        private GameStateManager _gameStateManager;

        public float CurrentProgress { get; private set; }

        public float CurrentEnemySpeed { get; private set; }
        public int CurrentSpawnInterval { get; private set; }

        public int CurrentPatternEnemyCount { get; private set; }   
        public float CurrentPatternSpacing { get; private set; }

        public void Init(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;

            _elapsedTime = 0f;
            CurrentEnemySpeed = _baseEnemySpeed;
            CurrentSpawnInterval = _baseSpawnInterval;

            CurrentPatternEnemyCount = _basePatternEnemyCount;
            CurrentPatternSpacing = _BasePatternSpacing;
        }

        private void Update()
        {
            if(_gameStateManager == null || _gameStateManager.CurrentState != GAME_STATE.PLAYING)
            {
                return;
            }

            float tDeltaTime = Time.deltaTime;
            _elapsedTime += tDeltaTime;

            CalculateDifficulty();
        }

        private void CalculateDifficulty()
        {
            CurrentProgress = Mathf.Clamp01(_elapsedTime / _timeToMaxDifficulty);
            float tProgress = CurrentProgress;

            CurrentEnemySpeed = Mathf.Lerp(_baseEnemySpeed, _maxEnemySpeed, tProgress);
            CurrentSpawnInterval = Mathf.RoundToInt(Mathf.Lerp(_baseSpawnInterval, _minSpawnInterval, tProgress));

            CurrentPatternEnemyCount = Mathf.RoundToInt(Mathf.Lerp(_basePatternEnemyCount, _maxPatternEnemyCount, tProgress));
            CurrentPatternSpacing = Mathf.Lerp(_BasePatternSpacing, _minPatternSpacing, tProgress);
        }
    }
}

