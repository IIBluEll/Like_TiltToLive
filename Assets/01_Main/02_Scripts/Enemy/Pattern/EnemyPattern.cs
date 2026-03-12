using UnityEngine;

namespace HM.Enemy.Pattern
{
    public interface IEnemyPattern
    {
        Vector3[] GetPatternPos(int enemyCount , float spacing , Vector3 spacingPos);
    }

    public class RandomPattern : IEnemyPattern
    {
        private Vector2 _spawnAreaMin;
        private Vector2 _spawnAreaMax;

        public RandomPattern(Vector2 spawnAreaMin, Vector2 spawnAreaMax)
        {
            _spawnAreaMin = spawnAreaMin;
            _spawnAreaMax = spawnAreaMax;
        }

        public Vector3[] GetPatternPos(int enemyCount, float spacing, Vector3 spacingPos)
        {
            Vector3[] tPatternPositions = new Vector3[enemyCount];

            for (int i = 0; i < enemyCount; i++)
            {
                float tRandomX = Random.Range(_spawnAreaMin.x, _spawnAreaMax.x);
                float tRandomY = Random.Range(_spawnAreaMin.y, _spawnAreaMax.y);
                tPatternPositions[i] = new Vector3(tRandomX, tRandomY, 0f);
            }
            return tPatternPositions;
        }
    }

    public class CirclePattern : IEnemyPattern
    {
        public Vector3[] GetPatternPos(int enemyCount , float spacing , Vector3 centerPos)
        {
            Vector3[] tPositions = new Vector3[enemyCount];

            float tRadius = spacing * 3f;
            float tAngleStep = 360f / enemyCount;

            for ( int tIndex = 0; tIndex < enemyCount; tIndex++ )
            {
                float tAngle = tIndex * tAngleStep * Mathf.Deg2Rad;
                float tX = centerPos.x + Mathf.Cos(tAngle) * tRadius;
                float tY = centerPos.y + Mathf.Sin(tAngle) * tRadius;
                tPositions[ tIndex ] = new Vector3(tX , tY , 0f);
            }
            return tPositions;
        }
    }
}