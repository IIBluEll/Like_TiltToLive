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
   public class ItemData : ScriptableObject
    {
        public ITEM_TYPE itemType;
        public Sprite itemIcon;
    }
}