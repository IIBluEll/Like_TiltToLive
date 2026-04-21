using HM.NewEnemy;
using UnityEngine;

namespace HM.Item.Effect
{
    public class BombArea : AEffectArea
    {
        protected override void ApplyEffectToEnmey(Collider2D collision)
        {
            if(collision.TryGetComponent(out EnemyController enemy))
            {
                if(!enemy.IsDead)
                {
                    enemy.Die();

                    //TODO : 점수 획득 로직
                }
            }
        }
    }
}

