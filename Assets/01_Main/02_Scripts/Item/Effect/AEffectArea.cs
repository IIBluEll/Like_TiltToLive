using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace HM.Item.Effect
{
    [RequireComponent(typeof(CircleCollider2D))]
    public abstract class AEffectArea : MonoBehaviour
    {
        [Header("Area Settings")]
        [SerializeField] protected float _duration = 0.5f;

        private CircleCollider2D _coll;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _coll = GetComponent<CircleCollider2D>();
            _coll.isTrigger = true;
        }

        public void ActivateArea()
        {
            gameObject.SetActive(true);

            if(_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }

            _cts = new CancellationTokenSource();
            Deactivate_async(_cts.Token).Forget();
        }

        private async UniTaskVoid Deactivate_async(CancellationToken token)
        {
            // 오브젝트가 파괴될 때 발생하는 토큰과 병합
            var tLinkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, this.GetCancellationTokenOnDestroy()).Token;

            // _duration 초 대기 (에러를 던지지 않고 취소 여부를 반환하도록 억제)
            bool tIsCanceled = await UniTask.Delay(TimeSpan.FromSeconds(_duration), cancellationToken: tLinkedToken).SuppressCancellationThrow();

            if(tIsCanceled)
            {
                return;
            }

            Destroy(gameObject);
        }

        // 구체적인 타격 효과는 자식 클래스에서 구현
        protected abstract void ApplyEffectToEnmey(Collider2D collision);

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ( collision.CompareTag("Enemy") )
            {
                ApplyEffectToEnmey(collision);
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if ( collision.CompareTag("Enemy") )
            {
                ApplyEffectToEnmey(collision);
            }
        }

        private void OnDestroy()
        {
            if(_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }
    }
}

