using HM.Enemy.System;
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

        private void Start()
        {
            _gameStateManager.Init();
            _gameDifficultyManager.Init(_gameStateManager);

            _enemyManagement.Init(_gameDifficultyManager, _gameStateManager);

            _gameStateManager.StartGame();
        }
    }
}
