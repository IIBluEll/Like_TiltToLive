using DG.Tweening;
using UnityEngine;
using System;

namespace HM.Item.Weapon
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ExplosionWeapon : MonoBehaviour, IWeapon
    {
        [Header("Explosion Settings")]
        [SerializeField] private float _expandTime = 0.5f;
        [SerializeField] private float _maxScale = 5f;

        private CircleCollider2D _collider;
        public Action<ExplosionWeapon> OnWeaponFinished;

        public WEAPON_TYPE WeaponType => WEAPON_TYPE.EXPLOSION;

        private void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();
            _collider.isTrigger = true;
        }

        public void Execute(Vector3 spawnPos, Transform playerTransform)
        {
            transform.position = spawnPos;
            transform.localScale = Vector3.zero;
            gameObject.SetActive(true);

            transform.DOScale(Vector3.one * _maxScale, _expandTime)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => 
                {
                    gameObject.SetActive(false);
                    OnWeaponFinished?.Invoke(this);
                });
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                // [검증 필요] EnemyController의 Die 처리가 필요합니다.
                // collision.GetComponent<EnemyController>()?.Die();
            }
        }
    }
}
