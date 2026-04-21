using System.Collections.Generic;
using UnityEngine;

public class PlanetObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject[] _planetPrefab;

    private readonly Dictionary<string, Queue<GameObject>> _dic_Pools = new();

    public void InitPool()
    {
        if ( _planetPrefab == null || _planetPrefab.Length == 0 ) return;

        for ( int i = 0; i < _planetPrefab.Length; i++ )
        {
            GameObject tPrefab = _planetPrefab[i];
            string tKey = tPrefab.name;
            _dic_Pools[ tKey ] = new Queue<GameObject>();

            GameObject tObj = Instantiate(tPrefab, transform);
            tObj.name = tPrefab.name;
            tObj.SetActive(false);
            _dic_Pools[ tKey ].Enqueue(tObj);
        }
    }

    public GameObject GetObject()
    {
        if ( _planetPrefab == null || _planetPrefab.Length == 0 ) return null;

        int tRandomIndex = Random.Range(0, _planetPrefab.Length);
        GameObject tPrefab = _planetPrefab[tRandomIndex];
        string tKey = tPrefab.name;

        if ( _dic_Pools.ContainsKey(tKey) && _dic_Pools[ tKey ].Count > 0 )
        {
            GameObject tObj = _dic_Pools[tKey].Dequeue();
            tObj.SetActive(true);
            return tObj;
        }

        // 풀이 모자랄 경우 추가 생성
        GameObject tNewObj = Instantiate(tPrefab, transform);
        tNewObj.name = tPrefab.name;
        return tNewObj;
    }

    public void ReturnObject(PlanetMove planet)
    {
        planet.gameObject.SetActive(false);
        string tKey = planet.gameObject.name;

        if ( _dic_Pools.ContainsKey(tKey) )
        {
            _dic_Pools[ tKey ].Enqueue(planet.gameObject);
        }
    }
}
