using Cysharp.Threading.Tasks;
using HM.CodeBase;
using HM.Manager;
using UnityEngine;

namespace HM.UI.MainMenu
{
    public class MainMenu_Presenter : APresenter
    {
        private MainMenu_View _view;
        private PresenterProvider _presenterProvider;
        private GameRootManager _rootManager;

        public MainMenu_Presenter(MainMenu_View view , PresenterProvider presenterProvider , GameRootManager rootManager)
        {
            _view = view;
            _presenterProvider = presenterProvider;
            _rootManager = rootManager;
        }

        public override void Open()
        {
            _view.Open();

            _view.StartBtn.onClick.AddListener(OnStartActioned);
            _view.SettingBtn.onClick.AddListener(OnSettingActioned);
            _view.ExitBtn.onClick.AddListener(OnExitActioned);

            _view.StartBtn.interactable = true;
        }

        public override void Close()
        {
            _view.StartBtn.onClick.RemoveListener(OnStartActioned);
            _view.SettingBtn.onClick.RemoveListener(OnSettingActioned);
            _view.ExitBtn.onClick.RemoveListener(OnExitActioned);

            _view.Close();
        }

        public override void Dispose()
        {
        }

        private void OnStartActioned()
        {
            _view.StartBtn.interactable = false;
            TransitionToInGame_async().Forget();

            _rootManager.ShowBoundary();
        }

        private async UniTaskVoid TransitionToInGame_async()
        {
            await _view.HideMenu_async();

            _presenterProvider.ChangeState(UI_STATE.INGAME);
        }

        private void OnSettingActioned()
        {
            Debug.Log("Setting Button Clicked");
        }

        private void OnExitActioned()
        {
            Debug.Log("Exit Button Clicked");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); 
#endif
        }
    }
}


