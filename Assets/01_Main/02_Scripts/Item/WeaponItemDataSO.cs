using UnityEngine;

namespace HM.Item.Data
{
    [CreateAssetMenu(fileName = "NewWeaponItem", menuName = "HM/Item/WeaponData")]
    public class WeaponItemDataSO : ScriptableObject
    {
        [Header("Weapon Info")]
        public string WeaponName;
        public Sprite ItemIcon;
        public WEAPON_TYPE WeaponType;
    }
}
