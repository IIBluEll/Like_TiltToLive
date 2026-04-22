using Cysharp.Threading.Tasks;
using HM.Item;
using HM.NewEnemy;
using HM.UI;
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

        [Space(5f), Header("UI")]
        [SerializeField] private PresenterProvider _presenterProvider;

        [Space(5f), Header("Logic")]
        [SerializeField] private EnemyManagement _enemyManagement;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private ItemManager _itemManagement;

        [SerializeField] private ScreenBoundary _screenBoundary;

        public Action<float> OnLoadingProgressChanged;

        private void Start()
        {
            _presenterProvider.Init(this);

            InitialzieGame_async().Forget();
        }

        private async UniTaskVoid InitialzieGame_async()
        {
            await UniTask.Delay(1000);

            // [로딩 0%] 기본 매니저 동기 초기화
            Debug.Log("[로딩 0%] 기본 매니저 동기 초기화");
            OnLoadingProgressChanged?.Invoke(0f);
            _gameStateManager.Init();
            _gameDifficultyManager.Init(_gameStateManager);

            if ( _playerController != null )
            {
                _playerController.Init(_gameStateManager);
                _playerController.OnPlayerDead -= OnPlayerDeadAction;
                _playerController.OnPlayerDead += OnPlayerDeadAction;
            }

            await UniTask.Delay(100);

            // [로딩 20%] 가장 무거운 작업인 적 오브젝트 풀링 시작 및 대기
            Debug.Log("[로딩 20%] 가장 무거운 작업인 적 오브젝트 풀링 시작 및 대기");
            OnLoadingProgressChanged?.Invoke(0.2f);
            await _enemyManagement.Init(_gameDifficultyManager, _gameStateManager);

            await UniTask.Delay(100);

            // [로딩 80%] 아이템 시스템 등 기타 초기화
            Debug.Log("[로딩 80%] 아이템 시스템 등 기타 초기화");
            OnLoadingProgressChanged?.Invoke(0.8f);
            _itemManagement.Init(_gameStateManager);

            await UniTask.Delay(100);
            
            // [로딩 100%] 모든 준비 완료 후 게임 시작
            Debug.Log("[로딩 100%] 모든 준비 완료 후 게임 시작");
            OnLoadingProgressChanged?.Invoke(1f);

            // 시각적인 로딩 완료를 보여주기 위해 약간의 인위적 대기가 필요할 수 있습니다.
            await UniTask.Delay(500);
        }

        private void OnPlayerDeadAction()
        {
            _gameStateManager.GameOver();
            _enemyManagement.StopSpawn();

            _presenterProvider.ChangeState(UI_STATE.GAMEOVER);
            _itemManagement.StopSpawn();
        }

        public void ShowBoundary()
        {
            _playerController.SpriteRenderOn();
            _playerController.ResetPlayer();
            _screenBoundary.ShowBoundary();
        }

        public void StartGame()
        {
            _gameStateManager.StartGame();

            _enemyManagement.StartSpawn();
            _itemManagement.StartSpawn();

            if (_playerController != null)
            {
                _playerController.CalibrateInput();
            }
        }

        public void RetryGame()
        {
            _gameStateManager.Init();
            _enemyManagement.ClearAllEnemies();
            _itemManagement.ClearAllItems();
            _playerController.ResetPlayer();
            _gameDifficultyManager.Init(_gameStateManager);
            _presenterProvider.ChangeState(UI_STATE.INGAME);
        }

        public void GoToMainMenu()
        {
            _gameStateManager.Init();
            _enemyManagement.ClearAllEnemies();
            _itemManagement.ClearAllItems();
            _playerController.ResetPlayer();
            _playerController.SpriteRenderOff();
            _gameDifficultyManager.Init(_gameStateManager);
            _presenterProvider.ChangeState(UI_STATE.MAINMENU);
            _screenBoundary.HideBoundary();
        }
    }
}
