using UnityEngine;

namespace HM.Item.Data
{
    public enum ITEM_TYPE
    {
        EXPLOSION,
        ICE,
        SHIELD
    }

    [CreateAssetMenu(fileName = "NewItemData", menuName = "Like_TiltToLive/ItemData")]
    public class ItemData : ScriptableObject
    {
        [Header("기본 정보")]
        public ITEM_TYPE ItemType;
        public string ItemName;
        
        [Tooltip("필드에 드랍되는 아이템 외형 프리팹 (ItemView가 붙어있어야 함)")]
        public GameObject ItemPrefab;

        [Tooltip("아이템 획득 시 발동되는 장판(AoE) 프리팹 (ItemAoE가 붙어있어야 함)")]
        public GameObject AoePrefab;

        [Header("효과 수치 (Effect Parameters)")]
        [Tooltip("장판의 반경 (보호막 크기, 폭발 크기 등)")]
        public float Radius = 2f;
        
        [Tooltip("효과 지속 시간 (폭발 장판 지속 시간, 얼음 빙결 시간, 보호막 분리 후 장판 지속 시간)")]
        public float Duration = 3f;

        // 아이템 타입에 맞는 효과 객체를 생성하여 반환 (전략 패턴 팩토리)
        public IItemEffect GetEffect()
        {
            switch (ItemType)
            {
                case ITEM_TYPE.EXPLOSION:
                    return new ExplosionEffect(Radius, Duration);
                case ITEM_TYPE.ICE:
                    // 얼음은 획득 시 화면을 덮는 매우 큰 사이즈를 가정하여 Radius를 크게 주거나
                    // 이펙트 내부 로직으로 처리할 수 있지만 여기선 입력받은 Radius를 그대로 사용
                    return new IceEffect(Radius, Duration);
                case ITEM_TYPE.SHIELD:
                    return new ShieldEffect(Radius, Duration);
                default:
                    Debug.LogWarning($"[ItemData] {ItemType}에 해당하는 효과가 구현되지 않았습니다.");
                    return null;
            }
        }
    }
}
