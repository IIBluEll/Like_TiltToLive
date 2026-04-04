using UnityEngine;
using Player;

namespace HM.Item.Data
{
    // 1. 폭발 효과: 획득 위치에 지정된 반경과 시간 동안 유지되는 파괴 장판 생성
    public class ExplosionEffect : IItemEffect
    {
        private float _radius;
        private float _duration;

        public ExplosionEffect(float radius, float duration)
        {
            _radius = radius;
            _duration = duration;
        }

        public void ExecuteEffect(Vector3 itemPosition, PlayerController targetPlayer, GameObject aoePrefab)
        {
            if (aoePrefab == null) return;

            // 풀에서 AoE 객체를 가져옴
            GameObject tAoeObj = ItemObjectPoolProvider.Instance.GetObject(aoePrefab);
            tAoeObj.transform.position = itemPosition;
            
            // 폭발 장판 셋업
            if (tAoeObj.TryGetComponent(out ItemAoE tAoe))
            {
                tAoe.SetupExplosion(_radius, _duration);
            }
        }
    }

    // 2. 얼음 효과: 획득 즉시 화면 전체(매우 큰 반경)에 순간적으로 빙결 장판 생성
    public class IceEffect : IItemEffect
    {
        private float _radius; // 얼음 장판의 크기 (화면을 덮을 만큼 크게 설정됨)
        private float _freezeDuration; // 적이 얼어있는 지속 시간

        public IceEffect(float radius, float freezeDuration)
        {
            _radius = radius;
            _freezeDuration = freezeDuration;
        }

        public void ExecuteEffect(Vector3 itemPosition, PlayerController targetPlayer, GameObject aoePrefab)
        {
            if (aoePrefab == null) return;

            GameObject tAoeObj = ItemObjectPoolProvider.Instance.GetObject(aoePrefab);
            tAoeObj.transform.position = itemPosition;

            if (tAoeObj.TryGetComponent(out ItemAoE tAoe))
            {
                // 얼음 장판은 순식간에 나타났다 사라지며 적에게 빙결 지속시간을 넘김
                tAoe.SetupIce(_radius, _freezeDuration);
            }
        }
    }

    // 3. 보호막 효과: 플레이어에 부착되는 장판, 충돌 시 떨어져 나와 폭발 장판으로 변환
    public class ShieldEffect : IItemEffect
    {
        private float _radius;
        private float _detachedDuration; // 떨어져 나온 후 유지되는 시간

        public ShieldEffect(float radius, float detachedDuration)
        {
            _radius = radius;
            _detachedDuration = detachedDuration;
        }

        public void ExecuteEffect(Vector3 itemPosition, PlayerController targetPlayer, GameObject aoePrefab)
        {
            if (aoePrefab == null) return;

            GameObject tAoeObj = ItemObjectPoolProvider.Instance.GetObject(aoePrefab);
            
            if (tAoeObj.TryGetComponent(out ItemAoE tAoe))
            {
                // 플레이어를 타겟으로 보호막 부착 셋업
                tAoe.SetupShield(targetPlayer, _radius, _detachedDuration);
            }
        }
    }
}
