using UnityEngine;
using HM.CodeBase;
using System.Collections.Generic;

public class ObjectPoolProvider : ASingletone<ObjectPoolProvider>
{
    private readonly Dictionary<string, Queue<GameObject>> _dic_Pools = new();

    public void CreatePool(GameObject prefab, int size, Transform parent = null)
    {
        string tKey = prefab.name;

        if(!_dic_Pools.ContainsKey(tKey))
        {
            _dic_Pools[tKey] = new Queue<GameObject>();
        }

        for(int i = 0; i < size; i++ )
        {
            GameObject tObj = Instantiate(prefab, parent);
            tObj.name = prefab.name;
            tObj.SetActive(false);
            _dic_Pools[tKey].Enqueue(tObj);
        }
    }

    public GameObject GetObject(GameObject prefab, Transform parent = null)
    {
        string tKey = prefab.name;

        if(_dic_Pools.ContainsKey(tKey) && _dic_Pools[tKey].Count > 0)
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

        if(!_dic_Pools.ContainsKey(tKey))
        {
            _dic_Pools[tKey] = new Queue<GameObject>();
        }

        _dic_Pools[tKey].Enqueue(obj);
    }
}
