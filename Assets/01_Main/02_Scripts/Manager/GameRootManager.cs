using Cysharp.Threading.Tasks;
using HM.Enemy.System;
using HM.Item;
using Player;
using System;
using UnityEngine;

namespace HM.Manager
{
    public class GameRootManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private GameStateManager _gameStateManager;
        [SerializeField] private GameDifficultyManager _gameDifficultyManager;

        [Space(5f), Header("Logic")]
        [SerializeField] private EnemyManagement _enemyManagement;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private ItemManagement _itemManagement;

        public Action<float> OnLoadingProgressChanged;

        private void Start()
        {
            InitialzieGame_async().Forget();
        }

        private async UniTaskVoid InitialzieGame_async()
        {
            // [로딩 0%] 기본 매니저 동기 초기화
            Debug.Log("[로딩 0%] 기본 매니저 동기 초기화");
            OnLoadingProgressChanged?.Invoke(0f);
            _gameStateManager.Init();
            _gameDifficultyManager.Init(_gameStateManager);

            if ( _playerController != null )
            {
                _playerController.OnplayerDead -= OnPlayerDeadAction;
                _playerController.OnplayerDead += OnPlayerDeadAction;
            }

            // [로딩 20%] 가장 무거운 작업인 적 오브젝트 풀링 시작 및 대기
            Debug.Log("[로딩 20%] 가장 무거운 작업인 적 오브젝트 풀링 시작 및 대기");
            OnLoadingProgressChanged?.Invoke(0.2f);
            await _enemyManagement.Init(_gameDifficultyManager , _gameStateManager);

            // [로딩 80%] 아이템 시스템 등 기타 초기화
            Debug.Log("[로딩 80%] 아이템 시스템 등 기타 초기화");
            OnLoadingProgressChanged?.Invoke(0.8f);
            _itemManagement.Init(_gameStateManager);

            // [로딩 100%] 모든 준비 완료 후 게임 시작
            Debug.Log("[로딩 100%] 모든 준비 완료 후 게임 시작");
            OnLoadingProgressChanged?.Invoke(1f);

            // 시각적인 로딩 완료를 보여주기 위해 약간의 인위적 대기가 필요할 수 있습니다.
            await UniTask.Delay(500);

            _gameStateManager.StartGame();
        }

        private void OnPlayerDeadAction()
        {
            _gameStateManager.GameOver();
            _enemyManagement.StopSpawn();
            _itemManagement.StopSpawn();
        }
    }
}
