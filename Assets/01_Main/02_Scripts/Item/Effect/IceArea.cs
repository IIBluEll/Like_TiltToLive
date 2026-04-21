using HM.NewEnemy;
using UnityEngine;

namespace HM.Item.Effect
{
    public class IceArea : AEffectArea
    {
        [Header("Ice Setting")]
        [SerializeField] private float _freezeDuration = 3f;

        protected override void ApplyEffectToEnmey(Collider2D collision)
        {
            if(collision.TryGetComponent(out EnemyController enemy))
            {
                if(!enemy.IsDead)
                {
                    enemy.ApplyFreeze(_freezeDuration);
                }
            }
        }
    }
}
 
