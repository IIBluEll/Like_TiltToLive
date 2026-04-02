using HM.CodeBase;
using HM.Item.Weapon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace HM.Item.Manager
{
    public class WeaponManagerProvider : ASingletone<WeaponManagerProvider>
    {
        [System.Serializable]
        public struct WeaponPrefabMap
        {
            public WEAPON_TYPE WeaponType;
            public GameObject WeaponPrefab;
        }

        [SerializeField] private List<WeaponPrefabMap> _weaponPrefabs;

        private Dictionary<WEAPON_TYPE, IObjectPool<GameObject>> _weaponPools = new Dictionary<WEAPON_TYPE, IObjectPool<GameObject>>();

        public override void Awake()
        {
            base.Awake();
            InitPools();
        }

        private void InitPools()
        {
            foreach (var map in _weaponPrefabs)
            {
                WEAPON_TYPE tType = map.WeaponType;
                GameObject tPrefab = map.WeaponPrefab;

                _weaponPools[tType] = new ObjectPool<GameObject>(
                    createFunc: () => Instantiate(tPrefab, transform),
                    actionOnGet: (obj) => { }, // 활성화는 무기 내부의 Execute에서 수행
                    actionOnRelease: (obj) => obj.SetActive(false),
                    actionOnDestroy: (obj) => Destroy(obj),
                    defaultCapacity: 5,
                    maxSize: 20
                );
            }
        }

        public void ExecuteWeapon(WEAPON_TYPE weaponType, Vector3 spawnPos, Transform playerTransform)
        {
            if (!_weaponPools.ContainsKey(weaponType)) return;

            GameObject tWeaponObj = _weaponPools[weaponType].Get();
            IWeapon tWeapon = tWeaponObj.GetComponent<IWeapon>();

            if (tWeapon != null)
            {
                if (tWeapon is ExplosionWeapon tExplosion)
                {
                    tExplosion.OnWeaponFinished -= ReturnExplosionToPool;
                    tExplosion.OnWeaponFinished += ReturnExplosionToPool;
                }
                
                tWeapon.Execute(spawnPos, playerTransform);
            }
        }

        private void ReturnExplosionToPool(ExplosionWeapon weapon)
        {
            weapon.OnWeaponFinished -= ReturnExplosionToPool;
            _weaponPools[weapon.WeaponType].Release(weapon.gameObject);
        }
    }
}
