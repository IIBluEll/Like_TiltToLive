using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 일정 주기마다 다양한 크기와 속도의 배경 행성 오브젝트를 스폰하여 우주 공간의 깊이감을 연출하는 매니저
/// </summary>
public class BackGroundManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private PlanetObjectPool _planetObjectPool;

    [Header("Spawn Settings")]
    [SerializeField] private float _spawnIntervalMin = 10f;
    [SerializeField] private float _spawnIntervalMax = 15f;
    [SerializeField] private float _minSpeed = 0.5f;
    [SerializeField] private float _maxSpeed = 2f;

    [Header("Spawn Bounds")]
    [SerializeField] private float _spawnRightX = 15f;
    [SerializeField] private float _cleanupLeftX = -15f;
    [SerializeField] private float _spawnMinY = -5f;
    [SerializeField] private float _spawnMaxY = 5f;

    private bool _isSpawning = false;

    [ContextMenu("Init")]
    public void Init()
    {
        _planetObjectPool.InitPool();
        StartSpawning();
    }

    public void StartSpawning()
    {
        if ( !_isSpawning )
        {
            _isSpawning = true;
            SpawnRoutine_async().Forget();
        }
    }

    public void StopSpawning()
    {
        _isSpawning = false;
    }

    private async UniTaskVoid SpawnRoutine_async()
    {
        while ( _isSpawning )
        {
            SpawnBlackHole();

            float tRandomInterval = Random.Range(_spawnIntervalMin, _spawnIntervalMax);
            await UniTask.Delay(System.TimeSpan.FromSeconds(tRandomInterval));
        }
    }

    private void SpawnBlackHole()
    {
        GameObject tObj = _planetObjectPool.GetObject();
        if ( tObj == null ) return;

        PlanetMove tPlanet = tObj.GetComponent<PlanetMove>();
        if ( tPlanet != null )
        {
            float tRandomY = Random.Range(_spawnMinY, _spawnMaxY);
            tPlanet.transform.position = new Vector3(_spawnRightX , tRandomY , 0f);

            float tRandomSpeed = Random.Range(_minSpeed, _maxSpeed);

            tPlanet.Init(tRandomSpeed , _cleanupLeftX , OnBlackHoleReturnedActioned);
        }
    }

    private void OnBlackHoleReturnedActioned(PlanetMove planet)
    {
        _planetObjectPool.ReturnObject(planet);
    }
}
