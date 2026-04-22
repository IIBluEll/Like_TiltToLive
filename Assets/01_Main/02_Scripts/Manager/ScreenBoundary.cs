using DG.Tweening;
using UnityEngine;

namespace HM.Manager
{
    [RequireComponent(typeof(LineRenderer))]
    public class ScreenBoundary : MonoBehaviour
    {
        [Header("Boundary Settings")]
        [SerializeField] private float _wallThickness = 1f; // 벽의 두께
        [SerializeField] private Vector2 _margin = new Vector2(0.5f, 0.5f); // 카메라 가장자리에서 안쪽으로 들어올 여백 (x: 가로, y: 세로)

        [SerializeField] private float _innerPadding = 0.5f;

        [Header("Visual Settings")]
        [SerializeField] private float _lineWidth = 0.1f;
        [SerializeField] private Color _lineColor = Color.red;

        [Header("BackGround Settings")]
        [SerializeField] private SpriteRenderer _backgroundImage;

        private Camera _mainCamera;
        private BoxCollider2D _topWall;
        private BoxCollider2D _bottomWall;
        private BoxCollider2D _leftWall;
        private BoxCollider2D _rightWall;
        private LineRenderer _lineRenderer;

        public static Rect PlayableArea { get; private set; }
        public static Rect KillArea { get; private set; }

        private void Awake()
        {
            _mainCamera = Camera.main;
            _lineRenderer = GetComponent<LineRenderer>();
            
            SetupLineRenderer();
            CreateWalls();
            UpdateBoundaries();

            HideBoundary();
        }

        private void SetupLineRenderer()
        {
            _lineRenderer.positionCount = 4;
            _lineRenderer.loop = true; // 사각형이 닫히도록 설정
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.startWidth = _lineWidth;
            _lineRenderer.endWidth = _lineWidth;
            
            // 기본 머티리얼이 없으면 핑크색으로 보이므로, Unlit 색상을 쓸 수 있도록 기본 머티리얼 할당
            if (_lineRenderer.sharedMaterial == null)
            {
                _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            }
            
            _lineRenderer.startColor = _lineColor;
            _lineRenderer.endColor = _lineColor;
        }

        private void CreateWalls()
        {
            // 빈 게임 오브젝트를 생성하고 BoxCollider2D를 추가합니다.
            _topWall = CreateWall("TopWall");
            _bottomWall = CreateWall("BottomWall");
            _leftWall = CreateWall("LeftWall");
            _rightWall = CreateWall("RightWall");
        }

        private BoxCollider2D CreateWall(string wallName)
        {
            GameObject tWallObj = new GameObject(wallName);
            tWallObj.transform.SetParent(transform); // 관리를 위해 자식으로 넣음

            BoxCollider2D tCollider = tWallObj.AddComponent<BoxCollider2D>();

            return tCollider;
        }

        public void ShowBoundary()
        {
            _lineRenderer.enabled = true;

            Color tStartColor = _lineColor;
            tStartColor.a = 0f;

            _lineRenderer.startColor = tStartColor;
            _lineRenderer.endColor = tStartColor;

            DOTween.To(() => tStartColor.a , x =>
            {
                tStartColor.a = x;
                _lineRenderer.startColor = tStartColor;
                _lineRenderer.endColor = tStartColor;
            } , _lineColor.a , 1f).SetEase(Ease.OutCubic);
        }

        public void HideBoundary()
        {
            if ( _lineRenderer != null )
            {
                _lineRenderer.DOKill();
                _lineRenderer.enabled = false;
            }
        }

        private void UpdateBoundaries()
        {
            if (_mainCamera == null) return;

            // 카메라의 높이와 너비를 월드 좌표 기준으로 계산
            float tHeight = _mainCamera.orthographicSize * 2f;
            float tWidth = tHeight * _mainCamera.aspect;

            // 여백(margin)을 적용하여 실제 벽이 배치될 공간 계산
            float tAdjustedHeight = tHeight - (_margin.y * 2f);
            float tAdjustedWidth = tWidth - (_margin.x * 2f);

            // 외부에서 사용할 Rect 정보
            float tHalfWidth = tAdjustedWidth / 2f;
            float tHalfHeight = tAdjustedHeight / 2f;
            Vector3 tCamPos = _mainCamera.transform.position;

            // 배경 이미지 크기 조절
            if (_backgroundImage != null && _backgroundImage.sprite != null)
            {
                float tSpriteWidth = _backgroundImage.sprite.bounds.size.x;
                float tSpriteHeight = _backgroundImage.sprite.bounds.size.y;

                // 카메라 사이즈에 맞춰 스케일 조정 (필요에 따라 _margin을 포함한 크기로 맞출 수도 있습니다)
                _backgroundImage.transform.localScale = new Vector3(tWidth / tSpriteWidth, tHeight / tSpriteHeight, 1f);
                _backgroundImage.transform.position = new Vector3(tCamPos.x, tCamPos.y, _backgroundImage.transform.position.z);
            }

            // 시각적인 선(halfWidth)에서 패딩(_innerPadding)만큼 더 안쪽으로 들어온 좌표를 구합니다.
            float tInnerHalfWidth = tHalfWidth - _innerPadding;
            float tInnerHalfHeight = tHalfHeight - _innerPadding;

            PlayableArea = new Rect(
                tCamPos.x - tInnerHalfWidth ,
                tCamPos.y - tInnerHalfHeight ,
                tInnerHalfWidth * 2f ,
                tInnerHalfHeight * 2f
            );

            // PlayableArea를 기준으로 상하좌우 5f (기획에 따라 조절) 확장된 영역을 킬존으로 정의합니다.
            float tKillMargin = 5f;
            KillArea = new Rect(
                PlayableArea.x - tKillMargin ,
                PlayableArea.y - tKillMargin ,
                PlayableArea.width + ( tKillMargin * 2f ) ,
                PlayableArea.height + ( tKillMargin * 2f )
            );

            // 1. 물리 벽 (Collider) 위치/크기 업데이트
            _topWall.size = new Vector2(tAdjustedWidth + _wallThickness * 2, _wallThickness);
            _bottomWall.size = new Vector2(tAdjustedWidth + _wallThickness * 2, _wallThickness);
            _leftWall.size = new Vector2(_wallThickness, tAdjustedHeight + _wallThickness * 2);
            _rightWall.size = new Vector2(_wallThickness, tAdjustedHeight + _wallThickness * 2);

            _topWall.transform.position = new Vector3(tCamPos.x, tCamPos.y + (tAdjustedHeight / 2f) + (_wallThickness / 2f), 0f);
            _bottomWall.transform.position = new Vector3(tCamPos.x, tCamPos.y - (tAdjustedHeight / 2f) - (_wallThickness / 2f), 0f);
            _leftWall.transform.position = new Vector3(tCamPos.x - (tAdjustedWidth / 2f) - (_wallThickness / 2f), tCamPos.y, 0f);
            _rightWall.transform.position = new Vector3(tCamPos.x + (tAdjustedWidth / 2f) + (_wallThickness / 2f), tCamPos.y, 0f);

            // 2. 시각적 선 (LineRenderer) 위치 업데이트
            if (_lineRenderer != null)
            {

                // 4개의 모서리 좌표 계산
                Vector3 bottomLeft = new Vector3(tCamPos.x - tHalfWidth, tCamPos.y - tHalfHeight, 0f);
                Vector3 topLeft = new Vector3(tCamPos.x - tHalfWidth, tCamPos.y + tHalfHeight, 0f);
                Vector3 topRight = new Vector3(tCamPos.x + tHalfWidth, tCamPos.y + tHalfHeight, 0f);
                Vector3 bottomRight = new Vector3(tCamPos.x + tHalfWidth, tCamPos.y - tHalfHeight, 0f);

                _lineRenderer.SetPosition(0, bottomLeft);
                _lineRenderer.SetPosition(1, topLeft);
                _lineRenderer.SetPosition(2, topRight);
                _lineRenderer.SetPosition(3, bottomRight);
                
                // 에디터에서 값이 변경될 수 있으므로 선 굵기/색상도 갱신
                _lineRenderer.startWidth = _lineWidth;
                _lineRenderer.endWidth = _lineWidth;
                _lineRenderer.startColor = _lineColor;
                _lineRenderer.endColor = _lineColor;
            }
        }

        [ContextMenu("UpdateBoundary")]
        private void OnRectTransformDimensionsChange()
        {
            if (_lineRenderer == null)
            {
                _lineRenderer = GetComponent<LineRenderer>();
            }
            UpdateBoundaries();
        }
    }
}
