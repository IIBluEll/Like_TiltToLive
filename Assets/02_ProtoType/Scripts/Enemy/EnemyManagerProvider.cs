using HM.CodeBase;
using System.Collections.Generic;
using UnityEngine;

namespace ProtoType.Enemy
{
    public class EnemyManagerProvider : ASingletone<EnemyManagerProvider>
    {
        private readonly List<EnemyController> _enemyControllers = new();

        public void RegisterEnemy(EnemyController enemy)
        {
            if ( !_enemyControllers.Contains(enemy) )
            {
                _enemyControllers.Add(enemy);
            }
        }

        public void UnregisterEnemy(EnemyController enemy)
        {
            if ( _enemyControllers.Contains(enemy) )
            {
                _enemyControllers.Remove(enemy);
            }
        }

        private void Update()
        {
            float tDeltaTime = Time.deltaTime;

            for ( int i = 0; i < _enemyControllers.Count; i++ )
            {
                _enemyControllers[i].Tick(tDeltaTime, _enemyControllers);
            }
        }
    }
}

