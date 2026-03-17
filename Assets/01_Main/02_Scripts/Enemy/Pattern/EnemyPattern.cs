using HM.Enemy.Controller;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace HM.Enemy.Pattern
{
    public struct PatternData
    {
        public Vector3[] SpawnPos;
        public IEnemyMovement MovementPattern;
    }

    public interface IEnemyPattern
    {
        PatternData GetPatternPos(int enemyCount , float spacing , Vector3 spacingPos);
    }

    // 원형 패턴 + 군집 이동
    public class CircleSwarmPattern : IEnemyPattern
    {
        public PatternData GetPatternPos(int enemyCount , float spacing , Vector3 centerPos)
        {
            Vector3[] tPos = new Vector3[enemyCount];
            float tRadius = spacing * 3f;
            float tAngleStep = 360f / enemyCount;

            for ( int i = 0; i < enemyCount; i++ )
            {
                float tAngle = i * tAngleStep * Mathf.Deg2Rad;
                float tX = centerPos.x + Mathf.Cos(tAngle) * tRadius;
                float tY = centerPos.y + Mathf.Sin(tAngle) * tRadius;
                tPos[ i ] = new Vector3(tX , tY , 0f);
            }

            return new PatternData
            {
                SpawnPos = tPos ,
                MovementPattern = new SwarmMovement()
            };
        }
    }

    // 가로 벽 + 아래로 직진
    public class HorizontalWallPattern : IEnemyPattern
    {
        public PatternData GetPatternPos(int enemyCount , float spacing , Vector3 centerPos)
        {
            Vector3[] tPositions = new Vector3[enemyCount];
            float tHalfLength = (enemyCount - 1) * spacing * 0.5f;
            Vector3 tSpawnCenter = centerPos + Vector3.up * 10f; // 화면 밖 위쪽 가정

            for ( int i = 0; i < enemyCount; i++ )
            {
                float tOffset = (i * spacing) - tHalfLength;
                tPositions[ i ] = new Vector3(tSpawnCenter.x + tOffset , tSpawnCenter.y , 0f);
            }

            return new PatternData
            {
                SpawnPos = tPositions ,
                MovementPattern = new LinearMovement(Vector3.down)
            };
        }
    }

    //public class RandomPattern : IEnemyPattern
    //{
    //    private Vector2 _spawnAreaMin;
    //    private Vector2 _spawnAreaMax;

    //    public RandomPattern(Vector2 spawnAreaMin, Vector2 spawnAreaMax)
    //    {
    //        _spawnAreaMin = spawnAreaMin;
    //        _spawnAreaMax = spawnAreaMax;
    //    }

    //    public Vector3[] GetPatternPos(int enemyCount, float spacing, Vector3 spacingPos)
    //    {
    //        Vector3[] tPatternPositions = new Vector3[enemyCount];

    //        for (int i = 0; i < enemyCount; i++)
    //        {
    //            float tRandomX = Random.Range(_spawnAreaMin.x, _spawnAreaMax.x);
    //            float tRandomY = Random.Range(_spawnAreaMin.y, _spawnAreaMax.y);
    //            tPatternPositions[i] = new Vector3(tRandomX, tRandomY, 0f);
    //        }
    //        return tPatternPositions;
    //    }
    //}

    //public class CirclePattern : IEnemyPattern
    //{
    //    public Vector3[] GetPatternPos(int enemyCount , float spacing , Vector3 centerPos)
    //    {
    //        Vector3[] tPositions = new Vector3[enemyCount];

    //        float tRadius = spacing * 3f;
    //        float tAngleStep = 360f / enemyCount;

    //        for ( int tIndex = 0; tIndex < enemyCount; tIndex++ )
    //        {
    //            float tAngle = tIndex * tAngleStep * Mathf.Deg2Rad;
    //            float tX = centerPos.x + Mathf.Cos(tAngle) * tRadius;
    //            float tY = centerPos.y + Mathf.Sin(tAngle) * tRadius;
    //            tPositions[ tIndex ] = new Vector3(tX , tY , 0f);
    //        }
    //        return tPositions;
    //    }
    //}
}