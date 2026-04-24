using UnityEngine;

namespace HM.Item.Data
{
    public enum ITEM_TYPE
    {
        BOMB,
        ICE,
        SHIELD
    }

   [CreateAssetMenu(fileName = "ItemData", menuName = "HM/ItemData")]
   /// <summary>
   /// 개별 아이템의 종류, 효과 프리팹 등의 기본 정보를 정의하는 ScriptableObject 데이터
   /// </summary>
   public class ItemData : ScriptableObject
    {
        public ITEM_TYPE itemType;
        public Sprite itemIcon;
    }
}