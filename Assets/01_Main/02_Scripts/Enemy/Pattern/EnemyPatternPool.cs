using UnityEngine;

namespace HM.Enemy.Pattern
{
    public class EnemyPatternPool
    {
        private IEnemyPattern _randomPattern;
        private IEnemyPattern _circlePattern;

        public EnemyPatternPool(Vector2 spawnAreaMin, Vector2 spawnAreaMax)
        {
            _randomPattern = new RandomPattern(spawnAreaMin,spawnAreaMax);
            _circlePattern = new CirclePattern();
        }

        public IEnemyPattern GetPattern(float progress)
        {
            if ( progress < 0.1f )
            {
                return _randomPattern;
            }
            else if ( progress < 0.6f )
            {
                int tRandom = Random.Range(0, 100);
                if ( tRandom < 50 )
                {
                    return _randomPattern;
                }
                else
                {
                    return _circlePattern;
                }
            }
            else
            {
                int tRandom = Random.Range(0, 100);
                if(tRandom < 30)
                {
                    return _randomPattern;
                }
                else
                {
                    return _circlePattern;
                }
            }
        }
    }
}

