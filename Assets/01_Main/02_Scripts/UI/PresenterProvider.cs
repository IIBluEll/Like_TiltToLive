using HM.CodeBase;
using HM.Manager;
using HM.UI.Loading;
using System.Collections.Generic;
using UnityEngine;

namespace HM.UI
{
    public enum UI_STATE
    {
        LOADING,
        MAINMENU,
        INGAME,
        GAMEOVER
    }

    public class PresenterProvider : MonoBehaviour
    {
        [Header("View")]
        [SerializeField] private Loading_View _loadingView;

        private Dictionary<UI_STATE,APresenter> _dic_Presenter = new();
        private APresenter _currentPresenter;

        public void Init(GameRootManager rootManager)
        {
            _dic_Presenter[ UI_STATE.LOADING ] = new Loading_Presenter(rootManager , this, _loadingView);

            ChangeState(UI_STATE.LOADING );
        }

        public void ChangeState(UI_STATE nextState)
        {
            if(_currentPresenter != null)
            {
                _currentPresenter.Close();
            }

            if(_dic_Presenter.TryGetValue(nextState, out APresenter tNextPresenter))
            {
                _currentPresenter = tNextPresenter;
                _currentPresenter.Open();
            }
            else
            {
                Debug.LogError($"[UIManager] 등록되지 않은 UI 상태입니다: {nextState}");
            }
        }

        private void OnDestroy()
        {
            foreach ( var tPresenter in _dic_Presenter.Values )
            {
                tPresenter?.Dispose();
            }
            _dic_Presenter.Clear();
        }
    }
}


