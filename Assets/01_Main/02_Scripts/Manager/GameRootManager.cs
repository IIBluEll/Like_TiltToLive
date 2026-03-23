using HM.Enemy.System;
using HM.Item;
using Player;
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

        private void Start()
        {
            _gameStateManager.Init();
            _gameDifficultyManager.Init(_gameStateManager);

            _enemyManagement.Init(_gameDifficultyManager, _gameStateManager);
            _itemManagement.Init(_gameStateManager);

            if(_playerController != null)
            {
                _playerController.OnplayerDead -= OnPlayerDeadAction;
                _playerController.OnplayerDead += OnPlayerDeadAction;
            }

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
