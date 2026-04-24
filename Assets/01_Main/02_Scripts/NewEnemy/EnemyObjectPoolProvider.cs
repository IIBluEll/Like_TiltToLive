using Cysharp.Threading.Tasks;
using HM.CodeBase;
using System.Collections.Generic;
using UnityEngine;

namespace HM.NewEnemy
{
    /// <summary>
    /// 대규모 적 객체의 빠른 생성과 반환을 위해 UniTask 비동기 초기화를 지원하는 적 전용 싱글톤 객체 풀
    /// </summary>
    public class EnemyObjectPoolProvider : ASingletone<EnemyObjectPoolProvider>
    {
        private readonly Dictionary<string, Queue<GameObject>> _dic_Pools = new();

        public async UniTask CreatePool_async(GameObject prefab , int size , Transform parent = null)
        {
            string tKey = prefab.name;

            if ( !_dic_Pools.ContainsKey(tKey) )
            {
                _dic_Pools[ tKey ] = new Queue<GameObject>();
            }

            // 한 프레임에 생성할 객체
            int tChunkSize = 100;

            for ( int i = 0; i < size; i++ )
            {
                GameObject tObj = Instantiate(prefab, parent);
                tObj.name = prefab.name;
                tObj.SetActive(false);
                _dic_Pools[ tKey ].Enqueue(tObj);

                if ( i > 0 && i % tChunkSize == 0 )
                {
                    await UniTask.Yield();
                }
            }
        }

        public void CreatePool(GameObject prefab , int size , Transform parent = null)
        {
            string tKey = prefab.name;

            if ( !_dic_Pools.ContainsKey(tKey) )
            {
                _dic_Pools[ tKey ] = new Queue<GameObject>();
            }

            for ( int i = 0; i < size; i++ )
            {
                GameObject tObj = Instantiate(prefab, parent);
                tObj.name = prefab.name;
                tObj.SetActive(false);
                _dic_Pools[ tKey ].Enqueue(tObj);
            }
        }

        public GameObject GetObject(GameObject prefab , Transform parent = null)
        {
            string tKey = prefab.name;

            if ( _dic_Pools.ContainsKey(tKey) && _dic_Pools[ tKey ].Count > 0 )
            {
                GameObject tObj = _dic_Pools[tKey].Dequeue();
                tObj.SetActive(true);
                return tObj;
            }

            // 풀에 객체가 없으면 새로 생성
            GameObject tNewObj = Instantiate(prefab, parent);
            tNewObj.name = prefab.name;
            return tNewObj;
        }

        public void ReturnObject(GameObject obj)
        {
            obj.SetActive(false);
            string tKey = obj.name;

            if ( !_dic_Pools.ContainsKey(tKey) )
            {
                _dic_Pools[ tKey ] = new Queue<GameObject>();
            }

            _dic_Pools[ tKey ].Enqueue(obj);
        }
    }
}

