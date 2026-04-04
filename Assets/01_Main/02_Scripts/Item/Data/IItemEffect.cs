using UnityEngine;
using Player;

namespace HM.Item.Data
{
    public interface IItemEffect
    {
        // itemPosition: 아이템 획득 위치
        // targetPlayer: 아이템을 획득한 플레이어 (보호막 부착 등에 사용)
        // aoePrefab: 실제 효과를 발생시킬 장판 프리팹 (ItemData에서 전달)
        void ExecuteEffect(Vector3 itemPosition, PlayerController targetPlayer, GameObject aoePrefab);
    }
}
