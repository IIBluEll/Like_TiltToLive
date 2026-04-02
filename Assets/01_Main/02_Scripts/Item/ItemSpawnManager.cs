using Cysharp.Threading.Tasks;
using HM.Item.Data;
using HM.Manager;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

namespace HM.Item.Manager
{
    public class ItemSpawnManager : MonoBehaviour
    {
        [Header("Item Spawn System")]
        [SerializeField] private FieldItem _fieldItemPrefab;
        [SerializeField] private WeaponItemDataSO[] _itemDatas;

        [SerializeField] private int _spawnInterval = 5000;
        [SerializeField] private int _defaultPoolSize = 5;
        [SerializeField] private int _maxPoolSize = 20;

        [Space(5f), Header("Spawn Area")]
        [SerializeField] private Vector2 _spawnAreaMin;
        [SerializeField] private Vector2 _spawnAreaMax;

        private GameStateManager _gameStateManager;
        private IObjectPool<FieldItem> _fieldItemPool;
        private CancellationTokenSource _spawnCts;

        public void Init(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;

            _fieldItemPool = new ObjectPool<FieldItem>(
                createFunc: () => Instantiate(_fieldItemPrefab, transform),
                actionOnGet: (tItem) => tItem.gameObject.SetActive(true),
                actionOnRelease: (tItem) => tItem.gameObject.SetActive(false),
                actionOnDestroy: (tItem) => Destroy(tItem.gameObject),
                collectionCheck: false,
                defaultCapacity: _defaultPoolSize,
                maxSize: _maxPoolSize
            );

            StartSpawn();
        }

        public void StartSpawn()
        {
            if (_spawnCts != null) return;

            _spawnCts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());
            SpawnRoutine_async(_spawnCts.Token).Forget();
        }

        public void StopSpawn()
        {
            _spawnCts?.Cancel();
            _spawnCts?.Dispose();
            _spawnCts = null;
        }

        private async UniTaskVoid SpawnRoutine_async(CancellationToken cancellationToken)
        {
            try
            {
                await UniTask.Delay(1000, cancellationToken: cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_gameStateManager != null && _gameStateManager.CurrentState == GAME_STATE.PLAYING)
                    {
                        SpawnFieldItem();
                    }
                    await UniTask.Delay(_spawnInterval, cancellationToken: cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // [확실한 사실] CancellationToken에 의해 정상적으로 취소됨 (메모리 누수 방지)
            }
        }

        private void SpawnFieldItem()
        {
            if (_itemDatas == null || _itemDatas.Length == 0) return;

            Vector3 tSpawnPos = GetRandomSpawnPos();
            FieldItem tFieldItem = _fieldItemPool.Get();

            tFieldItem.transform.position = tSpawnPos;

            int tRandomIndex = UnityEngine.Random.Range(0, _itemDatas.Length);
            tFieldItem.Init(_itemDatas[tRandomIndex]);

            // [추론적 접근] FieldItem이 비활성화(OnDisable)될 때 자동으로 풀에 반환되도록 설정
            // 이 처리를 위해 FieldItem 내부에 풀 반환 로직을 추가하거나, Manager에서 관리해야 합니다.
            // 가장 깔끔한 방법은 FieldItem 컴포넌트에 이벤트를 달거나, OnDisable에서 Action을 호출하는 것입니다.
            // 여기서는 임시로 ReturnAction을 추가하여 주입하는 방식을 사용하겠습니다.
            tFieldItem.SetReturnAction(ReturnItemToPool);
        }

        private void ReturnItemToPool(FieldItem item)
        {
            // 이미 풀에 반환되었는지 체크 로직이 필요할 수 있으나, ObjectPool에서 기본 방어가 가능합니다.
            if (item.gameObject.activeSelf)
            {
                _fieldItemPool.Release(item);
            }
        }

        private Vector3 GetRandomSpawnPos()
        {
            float tRandomX = UnityEngine.Random.Range(_spawnAreaMin.x, _spawnAreaMax.x);
            float tRandomY = UnityEngine.Random.Range(_spawnAreaMin.y, _spawnAreaMax.y);
            return new Vector3(tRandomX, tRandomY, 0f);
        }

        private void OnDestroy()
        {
            StopSpawn();
        }
    }
}
