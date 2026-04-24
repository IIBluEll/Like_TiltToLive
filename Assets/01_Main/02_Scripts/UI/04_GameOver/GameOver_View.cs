using HM.CodeBase;
using UnityEngine;
using UnityEngine.UI;

namespace HM.UI.GameOver
{
    /// <summary>
    /// 게임 오버 화면의 UI 요소(최종 생존 시간, 재시작/메인 메뉴 버튼 등)를 제어하고 이벤트를 전달하는 뷰
    /// </summary>
    public class GameOver_View : AView
    {
        [SerializeField] private Button _retryBtn;
        [SerializeField] private Button _mainMenuBtn;

        public Button RetryBtn => _retryBtn;
        public Button MainMenuBtn => _mainMenuBtn;
    }
}

