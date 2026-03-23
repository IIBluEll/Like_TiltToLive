using HM.Item.Weapon;
using System;
using UnityEngine;

namespace HM.Item
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ItemController : MonoBehaviour
    {
        [SerializeField] private GameObject _explosionPrefab;

        public Action<ItemController> OnItemCollected;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                GameObject tExplosionObj = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

                ExplosionController tExplosion = tExplosionObj.GetComponent<ExplosionController>();
                tExplosion?.InitExplosion();

                OnItemCollected?.Invoke(this);
            }
        }
    }
}

