using UnityEngine;

namespace HM.Item
{
    public enum WEAPON_TYPE
    {
        NONE,
        EXPLOSION,
        SHIELD,
        ICE_WAVE
    }

    public interface IWeapon
    {
        WEAPON_TYPE WeaponType { get; }
        void Execute(Vector3 spawnPos, Transform playerTransform);
    }
}
