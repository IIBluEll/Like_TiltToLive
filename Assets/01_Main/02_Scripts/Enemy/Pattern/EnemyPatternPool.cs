using System.Collections.Generic;
using UnityEngine;

namespace HM.Enemy.Pattern
{
    public class EnemyPatternPool
    {
        private readonly List<(IEnemyPattern pattern, float weight)> _weightedPatterns = new();

        public EnemyPatternPool()
        {
            _weightedPatterns.Add((new CircleSwarmPattern(), 1f));
            _weightedPatterns.Add((new HorizontalWallPattern(), 1f));
            _weightedPatterns.Add((new WobbleWallPattern(), 1f));    
            _weightedPatterns.Add((new CrossDashPattern(), 1f));
        }

        public PatternData GetPattern(float progress , int enemyCount , float spacing , Vector3 centerPos)
        {
            float tTotalWeight = 0f;
            foreach ( var tItem in _weightedPatterns )
            {
                tTotalWeight += tItem.weight;
            }

            float tRandomValue = Random.Range(0, tTotalWeight);
            float tCumulativeWeight = 0f;

            IEnemyPattern tSelectedPattern = _weightedPatterns[0].pattern;


            foreach ( var tItem in _weightedPatterns )
            {
                tCumulativeWeight += tItem.weight;
                if ( tRandomValue <= tCumulativeWeight )
                {
                    tSelectedPattern = tItem.pattern;
                    break;
                }
            }

            Debug.Log($"선택된 패턴 : {tSelectedPattern.GetType().Name}");

            return tSelectedPattern.GetPatternPos(enemyCount , spacing , centerPos);
        }
    }
}

