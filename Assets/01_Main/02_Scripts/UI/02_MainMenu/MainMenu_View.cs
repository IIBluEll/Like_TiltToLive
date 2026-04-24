using Cysharp.Threading.Tasks;
using DG.Tweening;
using HM.CodeBase;
using UnityEngine;
using UnityEngine.UI;

namespace HM.UI.MainMenu
{
    /// <summary>
    /// 메인 메뉴 화면의 버튼 등 UI 요소를 제어하고 사용자 입력을 프레젠터로 전달하는 뷰
    /// </summary>
    public class MainMenu_View : AView
    {
        [Header("UI")]
        [SerializeField] private Button _startBtn;
        [SerializeField] private Button _settingBtn;
        [SerializeField] private Button _exitBtn;

        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _mainPanel;

        private const float ANIMATION_DURATION = 0.5f;

        public Button StartBtn => _startBtn;
        public Button SettingBtn => _settingBtn;
        public Button ExitBtn => _exitBtn;

        public async UniTask HideMenu_async()
        {
            if(_mainPanel == null || _canvasGroup == null)
            {
                Debug.LogError("MainPanel or CanvasGroup is null");
                gameObject.SetActive(false);
                return;

            }

            _mainPanel.DOAnchorPosY(-500f , ANIMATION_DURATION).SetRelative().SetEase(Ease.InBack);
            _canvasGroup.DOFade(0f , ANIMATION_DURATION);

            await UniTask.Delay((int)( ANIMATION_DURATION * 1000 ));
        }

        public override void Open()
        {
            base.Open();

            if(_mainPanel != null)
            {
                _mainPanel.anchoredPosition = Vector2.zero;
            }

            if(_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }
        }
    }
}

