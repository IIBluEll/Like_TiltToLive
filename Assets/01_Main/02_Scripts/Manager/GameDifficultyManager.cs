using UnityEngine;

namespace HM.Manager
{
    public class GameDifficultyManager : MonoBehaviour
    {
        [Header("Difficulty Settings")]
        [SerializeField] private float _baseEnemySpeed = 3f;
        [SerializeField] private float _maxEnemySpeed = 6f;
        [SerializeField] private int _baseSpawnInterval = 800;
        [SerializeField] private int _minSpawnInterval = 300;
        [SerializeField] private float _timeToMaxDifficulty = 360f;
    }
}

