using System;
using UnityEngine;

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

        float tRandomNum = UnityEngine.Random.Range(1f,3f);

        transform.localScale = Vector3.one * tRandomNum;
        tAnim.speed = 1f - (0.15f * tRandomNum);

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
