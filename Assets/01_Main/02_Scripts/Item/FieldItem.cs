using HM.Item.Manager;
using HM.Item.Data;
using UnityEngine;
using System;

namespace HM.Item
{
    [RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))]
    public class FieldItem : MonoBehaviour
    {
        private WeaponItemDataSO _itemData;
        private SpriteRenderer _spriteRender;
        private Action<FieldItem> _returnAction;

        private void Awake()
        {
            _spriteRender = GetComponent<SpriteRenderer>();
            GetComponent<CircleCollider2D>().isTrigger = true;
        }

        public void Init(WeaponItemDataSO itemData)
        {
            _itemData = itemData;
            _spriteRender.sprite = _itemData.ItemIcon;
        }

        public void SetReturnAction(Action<FieldItem> returnAction)
        {
            _returnAction = returnAction;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                WeaponManagerProvider.Instance.ExecuteWeapon(_itemData.WeaponType, transform.position, collision.transform);
                
                if (_returnAction != null)
                {
                    _returnAction.Invoke(this);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
