using HM.CodeBase;
using HM.Manager;

namespace HM.UI.GameOver
{
    public class GameOver_Presenter : APresenter
    {
        private GameOver_View _view;
        private PresenterProvider _presenterProvider;
        private GameRootManager _rootManager;

        public GameOver_Presenter(GameOver_View view , PresenterProvider presenterProvider , GameRootManager rootManager)
        {
            _view = view;
            _presenterProvider = presenterProvider;
            _rootManager = rootManager;
        }

        public override void Open()
        {
            _view.Open();
            _view.RetryBtn.onClick.AddListener(OnRetryBtnClicked);
            _view.MainMenuBtn.onClick.AddListener(OnMainMenuBtnClicked);

        }

        public override void Close()
        {
            _view.RetryBtn.onClick.RemoveListener(OnRetryBtnClicked);
            _view.MainMenuBtn.onClick.RemoveListener(OnMainMenuBtnClicked);
            _view.Close();
        }

        public override void Dispose()
        {
            Close();
        }

        private void OnRetryBtnClicked()
        {
            _rootManager.RetryGame();
        }

        private void OnMainMenuBtnClicked()
        {
            _rootManager.GoToMainMenu();
        }
    }
}

