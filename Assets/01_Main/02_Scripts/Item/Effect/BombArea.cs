using HM.NewEnemy;
using UnityEngine;

namespace HM.Item.Effect
{
    /// <summary>
    /// 폭발 아이템 획득 시 생성되어 일정 범위 내의 모든 적을 파괴하는 이펙트 영역
    /// </summary>
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

