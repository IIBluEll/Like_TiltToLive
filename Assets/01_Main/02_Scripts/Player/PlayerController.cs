using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 10f;

        private Rigidbody2D _rigid;
        private Camera _mainCam;
        private Vector2 _targetPos;

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _mainCam = Camera.main;

            _rigid.gravityScale = 0f;
            _rigid.freezeRotation = true;
        }

        private void Update()
        {
            Vector3 tMousePos = Input.mousePosition;
            _targetPos = _mainCam.ScreenToWorldPoint(tMousePos);
        }

        private void FixedUpdate()
        {
            MoveToTarget();
        }

        private void MoveToTarget()
        {
            Vector2 tCurrentPos = _rigid.position;

            Vector2 tNextPos = Vector2.Lerp(tCurrentPos, _targetPos, _moveSpeed * Time.fixedDeltaTime);

            _rigid.MovePosition(tNextPos);
        }
    }
}

