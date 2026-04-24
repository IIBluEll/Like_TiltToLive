using HM.CodeBase;
using HM.Manager;
using UnityEngine;

namespace HM.UI.Loading
{
    /// <summary>
    /// 게임 초기 비동기 로딩 진행 상태를 모델과 뷰에 동기화하고 로딩 완료 시점을 관리하는 프레젠터
    /// </summary>
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


