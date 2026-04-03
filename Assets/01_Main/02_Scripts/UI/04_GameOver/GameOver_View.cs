using HM.CodeBase;
using UnityEngine;
using UnityEngine.UI;

namespace HM.UI.GameOver
{
    public class GameOver_View : AView
    {
        [SerializeField] private Button _retryBtn;
        [SerializeField] private Button _mainMenuBtn;

        public Button RetryBtn => _retryBtn;
        public Button MainMenuBtn => _mainMenuBtn;
    }
}

