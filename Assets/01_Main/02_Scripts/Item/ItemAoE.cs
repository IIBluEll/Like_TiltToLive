using UnityEngine;
using Player;
using HM.NewEnemy; // Assumes HM.NewEnemy for EnemyController based on previous grep results

namespace HM.Item
{
    public enum AOE_TYPE
    {
        EXPLOSION,
        ICE,
        SHIELD
    }

    [RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
    public class ItemAoE : MonoBehaviour
    {
        private AOE_TYPE _aoeType;
        private float _duration;
        private float _timer;
        private bool _isActive;
        private PlayerController _attachedPlayer;
        
        private CircleCollider2D _collider;
        private Rigidbody2D _rigid;

        private void Awake()
        {
            _collider = GetComponent<CircleCollider2D>();
            _collider.isTrigger = true;

            _rigid = GetComponent<Rigidbody2D>();
            _rigid.bodyType = RigidbodyType2D.Kinematic; // 물리력에 밀리지 않음
            _rigid.simulated = true;
        }

        // 1. 폭발 셋업 (지속형 장판)
        public void SetupExplosion(float radius, float duration)
        {
            _aoeType = AOE_TYPE.EXPLOSION;
            _collider.radius = radius;
            _duration = duration;
            _timer = 0f;
            _isActive = true;
            _attachedPlayer = null;
            
            //transform.localScale = new Vector3(radius * 2, radius * 2, 1f);
        }

        // 2. 얼음 셋업 (순간형 거대 장판)
        public void SetupIce(float radius, float freezeDuration)
        {
            _aoeType = AOE_TYPE.ICE;
            _collider.radius = radius;
            _duration = 0.5f; // 얼음 장판 자체는 0.5초만 유지되고 사라짐
            _timer = 0f;
            _isActive = true;
            _attachedPlayer = null;

            //transform.localScale = new Vector3(radius * 2, radius * 2, 1f);
            
            // 실제 빙결 시간은 부딪히는 적에게 넘겨줌 (여기서는 _timer 대신 사용될 수 있음)
            // 구현 상 편의를 위해 이 클래스에는 멤버로 저장하지 않고 OnTriggerEnter에서 처리
            // 본 예제에서는 Freeze 로직을 단순화하여 3초 등 고정값으로 전달할 수도 있음 (원한다면 필드 추가)
        }

        // 3. 보호막 셋업 (플레이어 부착형, 충돌 시 폭발로 전환)
        public void SetupShield(PlayerController targetPlayer, float radius, float detachedDuration)
        {
            _aoeType = AOE_TYPE.SHIELD;
            _collider.radius = radius;
            _duration = detachedDuration; // 떨어져 나간 후 폭발로 유지될 시간
            _timer = 0f;
            _isActive = true;
            _attachedPlayer = targetPlayer;

            //transform.localScale = new Vector3(radius * 2, radius * 2, 1f);

            // 플레이어에게 부착 (부모-자식 관계 혹은 Update에서 위치 동기화)
            transform.SetParent(targetPlayer.transform);
            transform.localPosition = Vector3.zero;

            // 플레이어에게 무적 상태 부여 지시
            targetPlayer.SetInvincibleState(true);
        }

        private void Update()
        {
            if (!_isActive) return;

            // 쉴드 상태가 아니고(폭발이거나 얼음이거나, 분리된 쉴드) 지속시간이 다 되면 풀로 반환
            if (_aoeType != AOE_TYPE.SHIELD)
            {
                _timer += Time.deltaTime;
                if (_timer >= _duration)
                {
                    Despawn();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_isActive) return;

            if (collision.CompareTag("Enemy"))
            {
                if (collision.TryGetComponent(out HM.NewEnemy.EnemyController tEnemy))
                {
                    switch (_aoeType)
                    {
                        case AOE_TYPE.EXPLOSION:
                            // 적 파괴
                            tEnemy.Die();
                            break;

                        case AOE_TYPE.ICE:
                            // 적 빙결
                            tEnemy.SetFreezeState(true);
                            break;

                        case AOE_TYPE.SHIELD:
                            // 쉴드 상태에서 적과 충돌하면 분리되어 폭발 장판으로 전환
                            DetachShieldAndConvertToExplosion();
                            
                            // 부딪힌 적은 파괴
                            tEnemy.Die();
                            break;
                    }
                }
            }
        }

        private void DetachShieldAndConvertToExplosion()
        {
            // 플레이어에게 부착 해제
            transform.SetParent(null);
            
            // 플레이어 무적 해제
            if (_attachedPlayer != null)
            {
                _attachedPlayer.SetInvincibleState(false);
                _attachedPlayer = null;
            }

            // 폭발 타입으로 변경하고 타이머 시작
            _aoeType = AOE_TYPE.EXPLOSION;
            _timer = 0f;
            // 반경이나 이펙트 변경도 여기서 가능

            Debug.Log("쉴드 분리 -> 폭발 장판으로 전환");
        }

        private void Despawn()
        {
            _isActive = false;
            _attachedPlayer = null;
            transform.SetParent(null); // 혹시 부착되어있었다면 해제
            
            ItemObjectPoolProvider.Instance.ReturnObject(gameObject);
        }
    }
}
