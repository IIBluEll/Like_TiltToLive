using System.Collections.Generic;
using UnityEngine;
using HM.CodeBase;
using Cysharp.Threading.Tasks;

namespace HM.Item
{
    public class ItemObjectPoolProvider : ASingletone<ItemObjectPoolProvider>
    {
        // 프리팹 이름을 키로 사용하여 큐를 관리
        private Dictionary<string, Queue<GameObject>> _poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // 부모 트랜스폼 (하이어라키 정리용)
        private Transform _poolRoot;

        public override void Awake()
        {
            base.Awake();
            
            // 씬에 풀 루트 생성
            GameObject tRootGo = new GameObject("[ItemObjectPool_Root]");
            _poolRoot = tRootGo.transform;
            DontDestroyOnLoad(tRootGo); // 씬 전환 시 유지 (필요에 따라 제거 가능)
        }

        /// <summary>
        /// 비동기로 일정 개수(chunkSize)만큼 오브젝트를 미리 생성하여 풀에 넣습니다.
        /// (로딩 화면 등에서 호출)
        /// </summary>
        public async UniTask CreatePool_async(GameObject prefab, int chunkSize)
        {
            if (prefab == null) return;
            string tPoolKey = prefab.name;

            if (!_poolDictionary.ContainsKey(tPoolKey))
            {
                _poolDictionary.Add(tPoolKey, new Queue<GameObject>());
            }

            for (int i = 0; i < chunkSize; i++)
            {
                GameObject tGo = Instantiate(prefab, _poolRoot);
                tGo.name = tPoolKey; // 중요: (Clone) 이름 제거
                tGo.SetActive(false);
                _poolDictionary[tPoolKey].Enqueue(tGo);

                // 너무 한 번에 많이 생성하면 렉이 걸리므로, 프레임 양보
                if (i % 5 == 0)
                {
                    await UniTask.Yield();
                }
            }
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져옵니다. 부족하면 새로 생성합니다.
        /// </summary>
        public GameObject GetObject(GameObject prefab)
        {
            if (prefab == null) return null;
            string tPoolKey = prefab.name;

            if (_poolDictionary.ContainsKey(tPoolKey) && _poolDictionary[tPoolKey].Count > 0)
            {
                GameObject tGo = _poolDictionary[tPoolKey].Dequeue();
                tGo.SetActive(true);
                return tGo;
            }
            else
            {
                // 풀이 비어있으면 즉시 생성 (경고 로그를 남겨 풀 크기를 늘릴지 판단할 수 있음)
                Debug.LogWarning($"[ItemPool] '{tPoolKey}' 풀이 부족하여 추가 생성합니다.");
                GameObject tGo = Instantiate(prefab, _poolRoot);
                tGo.name = tPoolKey;
                tGo.SetActive(true);
                return tGo;
            }
        }

        /// <summary>
        /// 사용이 끝난 오브젝트를 풀에 반환합니다.
        /// </summary>
        public void ReturnObject(GameObject instance)
        {
            if (instance == null) return;
            string tPoolKey = instance.name;

            if (_poolDictionary.ContainsKey(tPoolKey))
            {
                instance.SetActive(false);
                instance.transform.SetParent(_poolRoot); // 루트로 복귀
                _poolDictionary[tPoolKey].Enqueue(instance);
            }
            else
            {
                // 풀에 등록되지 않은 객체는 파괴 (안전장치)
                Destroy(instance);
            }
        }
    }
}
