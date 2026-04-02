using HM.CodeBase;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HM.UI.GameOver
{
    public class GameOver_Presenter : APresenter
    {
        private GameOver_View _view;
        private PresenterProvider _presenterProvider;

        public GameOver_Presenter(GameOver_View view, PresenterProvider presenterProvider)
        {
            _view = view;
            _presenterProvider = presenterProvider;
        }

        public override void Open()
        {
            _view.Open();
            
        }

        public override void Close()
        {
        }

        public override void Dispose()
        {
        }

        private void OnRetryBtnClicked()
        {
        }
    }
}

