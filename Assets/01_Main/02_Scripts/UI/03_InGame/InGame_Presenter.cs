using Cysharp.Threading.Tasks;
using HM.CodeBase;
using HM.Manager;
using System.Threading;
using UnityEngine;

namespace HM.UI.InGame
{
    public class InGame_Presenter : APresenter
    {
        private const int START_COUNTDOWN  = 5;

        private InGame_View _view;
        private InGame_Model _model;
        private PresenterProvider _presenterProvider;
        private GameRootManager _rootManager;

        private CancellationTokenSource _cancellToken;

        public InGame_Presenter(InGame_View view , PresenterProvider _provider , GameRootManager rootManager)
        {
            _view = view;
            _presenterProvider = _provider;
            _rootManager = rootManager;

            _model = new InGame_Model();
        }

        public override void Open()
        {
            _model.ResetData();
            _view.UpdateTimer(_model.SurviveTime);

            _view.Open();

            _cancellToken = new CancellationTokenSource();
            StartCountdown_async(_cancellToken.Token).Forget();
        }
        public override void Close()
        {
            if ( _cancellToken != null )
            {
                _cancellToken.Cancel();
                _cancellToken.Dispose();
                _cancellToken = null;
            }

            _view.Close();
        }
        public override void Dispose()
        {
            if ( _cancellToken != null )
            {
                _cancellToken.Cancel();
                _cancellToken.Dispose();
                _cancellToken = null;
            }
        }

        private async UniTask StartCountdown_async(CancellationToken cancellationToken)
        {
            int tCurrentCount = START_COUNTDOWN;

            _view.CountDownView.gameObject.SetActive(true);
            _view.UpdateScore(0);

            while ( tCurrentCount > 0 )
            {
                _view.CountDownView.ShowCountDown(tCurrentCount);
                await UniTask.Delay(1000 , ignoreTimeScale: true , cancellationToken: cancellationToken);
                tCurrentCount--;
            }

            _view.CountDownView.ShowStartMessage();
            await UniTask.Delay(1000 , ignoreTimeScale: true , cancellationToken: cancellationToken);

            _view.CountDownView.Hide();
            OnGameStartActioned();

            StartGameTimer_async(cancellationToken).Forget();
        }

        private void OnGameStartActioned()
        {
            _rootManager.StartGame();
        }

        private async UniTaskVoid StartGameTimer_async(CancellationToken token)
        {
            while ( !token.IsCancellationRequested )
            {
                await UniTask.Yield(PlayerLoopTiming.Update , token);

                _model.AddTime(Time.deltaTime);
                _view.UpdateTimer(_model.SurviveTime);
            }
        }

    }
}

