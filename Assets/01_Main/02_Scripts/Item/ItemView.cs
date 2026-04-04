using UnityEngine;
using HM.Item.Data;
using Player;

namespace HM.Item
{
    [RequireComponent(typeof(Collider2D))]
    public class ItemView : MonoBehaviour
    {
        [Header("아이템 데이터 (인스펙터 할당)")]
        [SerializeField] private ItemData _itemData;

        public ITEM_TYPE ItemType => _itemData.ItemType;

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 1. 충돌한 대상이 플레이어인지 확인 (Tag 검사)
            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent(out PlayerController tPlayerController))
                {
                    // 2. SO(ItemData)로부터 효과 객체를 생성하여 실행
                    IItemEffect tEffect = _itemData.GetEffect();
                    if (tEffect != null)
                    {
                        // 아이템 획득 위치, 대상(플레이어), 사용할 AoE 프리팹을 전달
                        tEffect.ExecuteEffect(transform.position, tPlayerController, _itemData.AoePrefab); 
                    }

                    // 3. 획득 피드백(사운드, 파티클 등) 처리 예정
                    // AudioManagerProvider.Instance.PlaySFX("ItemAcquire");

                    // 4. 사용 완료된 아이템 뷰를 풀에 반환
                    ItemObjectPoolProvider.Instance.ReturnObject(gameObject);
                }
            }
        }
    }
}
