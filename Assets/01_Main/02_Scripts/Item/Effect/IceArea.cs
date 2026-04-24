using HM.NewEnemy;
using UnityEngine;

namespace HM.Item.Effect
{
    /// <summary>
    /// 빙결 아이템 획득 시 생성되어 일정 범위 내의 적들의 이동을 일시적으로 멈추게 하는 이펙트 영역
    /// </summary>
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
 
