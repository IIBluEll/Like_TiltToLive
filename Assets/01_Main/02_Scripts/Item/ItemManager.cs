using Cysharp.Threading.Tasks;
using HM.Manager;
using System;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HM.Item
{
    /// <summary>
    /// 인게임 중 주기적으로 아이템을 스폰하고 모든 아이템 객체의 생명주기를 관리하는 매니저
    /// </summary>
    public class ItemManager : MonoBehaviour
    {
        [Header("Spawn Setting")]
        [SerializeField] private float _spawnInterval = 5f;
        [SerializeField] private int _maxItemOnScreen = 3;

        [Space(5f), Header("Item Prefabs")]
        [SerializeField] private GameObject _bombItem;
        [SerializeField] private GameObject _freezeItem;

        [Space(5f)]
        [SerializeField] private GameStateManager _gameStateManager;

        private CancellationTokenSource _spawnCts;
        private int CurrnetItemCount => transform.childCount;

        public void Init(GameStateManager stateManager)
        {
            _gameStateManager = stateManager;
        }

        public void StartSpawn()
        {
            if ( _spawnCts != null )
            {
                _spawnCts.Cancel();
                _spawnCts.Dispose();
                _spawnCts = null;
            }

            _spawnCts = new CancellationTokenSource();
            SpawnRoutine_async(_spawnCts.Token).Forget();
        }

        public void StopSpawn()
        {
            if ( _spawnCts != null )
            {
                _spawnCts.Cancel();
                _spawnCts.Dispose();
                _spawnCts = null;
            }
        }

        public void ClearAllItems()
        {
            StopSpawn();

            foreach ( Transform child in transform )
            {
                Destroy(child.gameObject);
            }
        }

        private async UniTaskVoid SpawnRoutine_async(CancellationToken token)
        {
            // 이 매니저 오브젝트가 파괴될 때 자동으로 취소되도록 토큰 연결
            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy()).Token;

            while ( !linkedToken.IsCancellationRequested )
            {
                // 게임오버나 메인메뉴 상태가 되면 스폰 루프 종료
                if ( _gameStateManager != null && _gameStateManager.CurrentState != GAME_STATE.PLAYING )
                {
                    break;
                }

                if ( CurrnetItemCount < _maxItemOnScreen )
                {
                    SpawnRandomItem();
                }

                // _spawnInterval 만큼 대기
                bool isCanceled = await UniTask.Delay(TimeSpan.FromSeconds(_spawnInterval), cancellationToken: linkedToken).SuppressCancellationThrow();

                if ( isCanceled )
                {
                    return;
                }
            }
        }

        private void SpawnRandomItem()
        {
            Rect tPlayableArea = ScreenBoundary.PlayableArea;

            float tRandomX = Random.Range(tPlayableArea.xMin, tPlayableArea.xMax);
            float tRandomY = Random.Range(tPlayableArea.yMin, tPlayableArea.yMax);

            Vector2 tSpawnPos = new Vector2(tRandomX, tRandomY);

            GameObject tPrefab = Random.Range(0, 2) == 0 ? _bombItem : _freezeItem;

            if ( tPrefab != null )
            {
                GameObject tItem = Instantiate(tPrefab, tSpawnPos, Quaternion.identity, transform);
            }
        }
    }
}

