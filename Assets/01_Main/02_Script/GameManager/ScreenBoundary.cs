using UnityEngine;

namespace HM.Manager
{
    [RequireComponent(typeof(LineRenderer))]
    public class ScreenBoundary : MonoBehaviour
    {
        [Header("Boundary Settings")]
        [SerializeField] private float _wallThickness = 1f; // 벽의 두께
        [SerializeField] private float _margin = 0.5f; // 카메라 가장자리에서 안쪽으로 들어올 여백

        [Header("Visual Settings")]
        [SerializeField] private float _lineWidth = 0.1f;
        [SerializeField] private Color _lineColor = Color.red;

        private Camera _mainCamera;
        private BoxCollider2D _topWall;
        private BoxCollider2D _bottomWall;
        private BoxCollider2D _leftWall;
        private BoxCollider2D _rightWall;
        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _lineRenderer = GetComponent<LineRenderer>();
            
            SetupLineRenderer();
            CreateWalls();
            UpdateBoundaries();
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

            // 필요에 따라 레이어를 설정할 수 있습니다. 예: "Wall" 레이어
            // tWallObj.layer = LayerMask.NameToLayer("Wall"); 

            BoxCollider2D tCollider = tWallObj.AddComponent<BoxCollider2D>();

            return tCollider;
        }

        private void UpdateBoundaries()
        {
            if (_mainCamera == null) return;

            // 카메라의 높이와 너비를 월드 좌표 기준으로 계산
            float tHeight = _mainCamera.orthographicSize * 2f;
            float tWidth = tHeight * _mainCamera.aspect;

            // 여백(margin)을 적용하여 실제 벽이 배치될 공간 계산
            float tAdjustedHeight = tHeight - (_margin * 2f);
            float tAdjustedWidth = tWidth - (_margin * 2f);

            // 1. 물리 벽 (Collider) 위치/크기 업데이트
            _topWall.size = new Vector2(tAdjustedWidth + _wallThickness * 2, _wallThickness);
            _bottomWall.size = new Vector2(tAdjustedWidth + _wallThickness * 2, _wallThickness);
            _leftWall.size = new Vector2(_wallThickness, tAdjustedHeight + _wallThickness * 2);
            _rightWall.size = new Vector2(_wallThickness, tAdjustedHeight + _wallThickness * 2);

            Vector3 tCamPos = _mainCamera.transform.position;

            _topWall.transform.position = new Vector3(tCamPos.x, tCamPos.y + (tAdjustedHeight / 2f) + (_wallThickness / 2f), 0f);
            _bottomWall.transform.position = new Vector3(tCamPos.x, tCamPos.y - (tAdjustedHeight / 2f) - (_wallThickness / 2f), 0f);
            _leftWall.transform.position = new Vector3(tCamPos.x - (tAdjustedWidth / 2f) - (_wallThickness / 2f), tCamPos.y, 0f);
            _rightWall.transform.position = new Vector3(tCamPos.x + (tAdjustedWidth / 2f) + (_wallThickness / 2f), tCamPos.y, 0f);

            // 2. 시각적 선 (LineRenderer) 위치 업데이트
            if (_lineRenderer != null)
            {
                float halfWidth = tAdjustedWidth / 2f;
                float halfHeight = tAdjustedHeight / 2f;

                // 4개의 모서리 좌표 계산
                Vector3 bottomLeft = new Vector3(tCamPos.x - halfWidth, tCamPos.y - halfHeight, 0f);
                Vector3 topLeft = new Vector3(tCamPos.x - halfWidth, tCamPos.y + halfHeight, 0f);
                Vector3 topRight = new Vector3(tCamPos.x + halfWidth, tCamPos.y + halfHeight, 0f);
                Vector3 bottomRight = new Vector3(tCamPos.x + halfWidth, tCamPos.y - halfHeight, 0f);

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

        // 안드로이드에서 화면 회전 등으로 해상도가 바뀔 때 갱신 (선택 사항)
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
