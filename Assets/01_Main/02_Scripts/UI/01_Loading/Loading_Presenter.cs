using HM.CodeBase;
using HM.Manager;
using UnityEngine;

namespace HM.UI.Loading
{
    public class Loading_Presenter : APresenter
    {
        private Loading_View _view;

        private PresenterProvider _presenterProvider;
        private GameRootManager _rootManager;

        public Loading_Presenter(GameRootManager rootManager, PresenterProvider presenterManager ,Loading_View view)
        {
            _presenterProvider = presenterManager;
            _rootManager = rootManager;
            _view = view;
        }

        public override void Open()
        {
            _rootManager.OnLoadingProgressChanged += OnModelProgressAction;
            _view.Open();
        }

        public override void Close()
        {
            _rootManager.OnLoadingProgressChanged -= OnModelProgressAction;
            _view.Close();
        }

        public override void Dispose()
        {
            _rootManager.OnLoadingProgressChanged -= OnModelProgressAction;
        }

        private void OnModelProgressAction(float progress)
        {
            _view?.UpdateProgressUI(progress);

            if(progress >= 1f)
            {
                Debug.Log("로딩 끝");
                _presenterProvider.ChangeState(UI_STATE.MAINMENU);
            }
        }

    }
}


