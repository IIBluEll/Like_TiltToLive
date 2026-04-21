using HM.Item.Data;
using UnityEngine;

namespace HM.Item
{
    public class ItemController : MonoBehaviour
    {
        [Header("Item Settings")]
        [SerializeField] private ItemData _itemData;

        [Space(5f), Header("Effect Area")]
        [SerializeField] private GameObject _bombAreaPrefab;
        [SerializeField] private GameObject _iceAreaPrefab;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                SpawnEffectArea();

                Destroy(gameObject);
            }    
        }

        private void SpawnEffectArea()
        {
            GameObject areaPrefabToSpawn = null;

            switch(_itemData.itemType)
            {
                case ITEM_TYPE.BOMB:
                    areaPrefabToSpawn = _bombAreaPrefab;
                    break;

                case ITEM_TYPE.ICE:
                    areaPrefabToSpawn = _iceAreaPrefab;
                    break;
            }

            if(areaPrefabToSpawn != null)
            {
                GameObject spawnedArea = Instantiate(areaPrefabToSpawn, transform.position, Quaternion.identity);

              if(spawnedArea.TryGetComponent(out Effect.AEffectArea effectArea))
                {
                    effectArea.ActivateArea();
                }
            }
        }
    }
}

