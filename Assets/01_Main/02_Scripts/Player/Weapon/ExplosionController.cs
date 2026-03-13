using DG.Tweening;
using UnityEngine;

namespace HM.Item.Weapon
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class ExplosionController : MonoBehaviour
    {
        [Header("폭발 세팅")]
        [SerializeField] private float _expandTime = 0.5f;
        [SerializeField] private float _untilTime  = 1.5f;

        private void Awake()
        {
            CircleCollider2D tCollider = GetComponent<CircleCollider2D>();
            tCollider.isTrigger = true;
        }

        public void InitExplosion()
        {
            transform.localScale = Vector3.zero;
            Sequence tSeq = DOTween.Sequence();

            tSeq.Append(transform.DOScale(Vector3.one , _expandTime).SetEase(Ease.OutQuad)).AppendInterval(_untilTime).OnComplete(OnExplosionFinished);
        }

        private void OnExplosionFinished()
        {
            Destroy(gameObject);
        }
    }
}

