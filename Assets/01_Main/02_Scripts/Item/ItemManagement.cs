using Cysharp.Threading.Tasks;
using HM.Manager;
using HM.Item.Data; 
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace HM.Item
{
    public class ItemManagement : MonoBehaviour
    {
        [Header("Item Spawn Settings")]
        [Tooltip("게임에 등장할 아이템 데이터들")]
        [SerializeField] private List<ItemData> _availableItems;
        [SerializeField] private float _spawnInterval = 10f; // 10초마다 스폰

        private GameStateManager _gameStateManager;
        private CancellationTokenSource _cts;

        // 현재 필드에 활성화된 아이템들 추적용
        private readonly List<GameObject> _activeItems = new List<GameObject>();

        /// <summary>
        /// GameRootManager에서 호출되는 비동기 초기화 로직
        /// </summary>
        public async UniTask Init(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;

            // 등록된 모든 아이템의 프리팹을 풀링 시스템에 등록 및 미리 생성
            foreach (var tItemData in _availableItems)
            {
                if (tItemData.ItemPrefab != null)
                {
                    // 뷰 프리팹 풀링
                    await ItemObjectPoolProvider.Instance.CreatePool_async(tItemData.ItemPrefab, 5);
                }
                if (tItemData.AoePrefab != null)
                {
                    // AoE 프리팹 풀링
                    await ItemObjectPoolProvider.Instance.CreatePool_async(tItemData.AoePrefab, 5);
                }
            }
            
            Debug.Log("[ItemManagement] 아이템 초기화 및 풀링 완료");
        }

        public void StartSpawn()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            _cts = new CancellationTokenSource();

            SpawnRoutine_async(_cts.Token).Forget();
        }

        public void StopSpawn()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        /// <summary>
        /// 재시작(Retry) 시 기존에 깔려있던 아이템들을 모두 수거합니다.
        /// </summary>
        public void ClearAllItems()
        {
            foreach (var tItemGo in _activeItems)
            {
                if (tItemGo != null && tItemGo.activeSelf)
                {
                    ItemObjectPoolProvider.Instance.ReturnObject(tItemGo);
                }
            }
            _activeItems.Clear();
        }

        private async UniTaskVoid SpawnRoutine_async(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // PLAYING 상태일 때만 타이머 체크 및 스폰
                if (_gameStateManager.CurrentState == GAME_STATE.PLAYING)
                {
                    await UniTask.Delay(System.TimeSpan.FromSeconds(_spawnInterval), cancellationToken: token);

                    if (token.IsCancellationRequested) return;
                    if (_gameStateManager.CurrentState != GAME_STATE.PLAYING) continue;

                    SpawnRandomItem();
                }
                else
                {
                    // 일시정지나 게임오버 상태일 때는 프레임을 양보하며 대기
                    await UniTask.Yield(token);
                }
            }
        }

        private void SpawnRandomItem()
        {
            if (_availableItems == null || _availableItems.Count == 0) return;

            // 1. 랜덤 아이템 선택
            int tRandIndex = Random.Range(0, _availableItems.Count);
            ItemData tSelectedData = _availableItems[tRandIndex];
            
            if (tSelectedData == null || tSelectedData.ItemPrefab == null) return;

            // 2. ScreenBoundary를 활용한 안전한 랜덤 위치 선정
            Rect tPlayableArea = ScreenBoundary.PlayableArea;
            
            Vector3 tSpawnPos = new Vector3(
                Random.Range(tPlayableArea.xMin, tPlayableArea.xMax), 
                Random.Range(tPlayableArea.yMin, tPlayableArea.yMax), 
                0f
            );

            // 3. 풀에서 오브젝트 가져와 활성화
            GameObject tItemGo = ItemObjectPoolProvider.Instance.GetObject(tSelectedData.ItemPrefab);
            tItemGo.transform.position = tSpawnPos;
            
            _activeItems.Add(tItemGo);
        }
    }
}
