using System;
using UnityEngine;

/// <summary>
/// 배경 행성 오브젝트가 화면 아래로 떨어지도록 이동시키고 화면을 벗어나면 풀로 반환하는 로직
/// </summary>
public class PlanetMove : MonoBehaviour
{
    private float _moveSpeed;
    private float _leftCleanUpX;
    private Action<PlanetMove> _onReturnToPool;

    public void Init(float movespeed, float leftCleanUpX, Action<PlanetMove> onReturnToPool)
    {
        _moveSpeed = movespeed;
        _leftCleanUpX = leftCleanUpX;
        _onReturnToPool = onReturnToPool;

        Animator tAnim = GetComponent<Animator>();
        SpriteRenderer tSprite = GetComponent<SpriteRenderer>();

        float tRandomNum = UnityEngine.Random.Range(0.5f,2f);

        transform.localScale = Vector3.one * tRandomNum;

        tAnim.speed = Mathf.Lerp(0.6f , 0.1f , ( tRandomNum - 1f ) / 2f);

        var tColor = tSprite.color;
        tColor.a = Mathf.Lerp(0.3f, 0.7f, (tRandomNum - 1f) / 2f);
        tSprite.color = tColor;
    }

    private void Update()
    {
        transform.Translate(Vector3.left * ( _moveSpeed * Time.deltaTime ));

        if(transform.position. x < _leftCleanUpX )
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        transform.localScale = Vector3.one;
        _onReturnToPool?.Invoke(this);
        gameObject.SetActive(false);
    }
}
