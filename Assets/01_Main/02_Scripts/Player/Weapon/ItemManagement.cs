using Cysharp.Threading.Tasks;
using HM.Manager;
using System.Collections.Generic;
using UnityEngine;

namespace HM.Item
{
    public class ItemManagement : MonoBehaviour
    {
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private int _spawnInterval = 5000;

        [Header("Spawn Area")]
        [SerializeField] private Vector2 _spawnAreaMin;
        [SerializeField] private Vector2 _spawnAreaMax;

        private bool _isSpawning = false;
        private GameStateManager _gameStateManager;
        private readonly List<ItemController> _activeItem = new();

        public void Init(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
            StartSpawn();
        }

        public void StartSpawn()
        {
            if ( !_isSpawning )
            {
                _isSpawning = true;
                SpawnRoutine_async().Forget(); // 비동기 함수 컨벤션 적용
            }
        }

        public void StopSpawn()
        {
            _isSpawning = false;
        }

        private async UniTaskVoid SpawnRoutine_async()
        {
            await UniTask.Delay(1000);

            while ( _isSpawning )
            {
                // 게임이 플레이 중일 때만 스폰 타이머가 유효하도록 통제
                if ( _gameStateManager != null && _gameStateManager.CurrentState == GAME_STATE.PLAYING )
                {
                    SpawnItem();
                }

                await UniTask.Delay(_spawnInterval);
            }
        }

        private void SpawnItem()
        {
            Vector3 tSpawnPos = GetRandomSpawnPos();

            Debug.Log($"[ItemManagement] 아이템 스폰 시도! 위치: {tSpawnPos}");

            // 아이템은 적처럼 개수가 많지 않으므로 Instantiate 사용이 타당합니다.
            GameObject tItemObj = Instantiate(_itemPrefab, tSpawnPos, Quaternion.identity, transform);

            ItemController tItemController = tItemObj.GetComponent<ItemController>();
            if ( tItemController != null )
            {
                // 플레이어가 아이템을 먹었을 때 발생하는 이벤트를 구독하여 객체 파괴 처리
                tItemController.OnItemCollected -= OnItemCollectedActioned;
                tItemController.OnItemCollected += OnItemCollectedActioned;

                _activeItem.Add(tItemController);
            }
            else
            {
                // 3. 프리팹 세팅 누락 확인
                Debug.LogError("[ItemManagement] 프리팹에 ItemController 스크립트가 없습니다!");
            }
        }

        // 이벤트 바인딩 함수 컨벤션 적용
        private void OnItemCollectedActioned(ItemController item)
        {
            item.OnItemCollected -= OnItemCollectedActioned;
            _activeItem.Remove(item);

            // 획득된 아이템 캡슐 파괴
            Destroy(item.gameObject);
        }

        private Vector3 GetRandomSpawnPos()
        {
            float tRandomX = Random.Range(_spawnAreaMin.x, _spawnAreaMax.x);
            float tRandomY = Random.Range(_spawnAreaMin.y, _spawnAreaMax.y);
            return new Vector3(tRandomX , tRandomY , 0f);
        }
    }
}

