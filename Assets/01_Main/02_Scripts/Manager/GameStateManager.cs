using UnityEngine;

namespace HM.Manager
{
    public enum GAME_STATE
    {
        MAINMENU,
        PLAYING,
        PAUSED,
        GAMEOVER,
    }

    /// <summary>
    /// 게임의 전반적인 상태(PLAYING, GAMEOVER 등)를 관리하고, 상태 변경에 따른 이벤트를 브로드캐스팅하는 매니저
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        public GAME_STATE CurrentState { get; private set; }

        public void Init()
        {
            CurrentState = GAME_STATE.MAINMENU;
            Time.timeScale = 1f;
        }

        public void StartGame()
        {
            CurrentState = GAME_STATE.PLAYING;
            Time.timeScale = 1f;
        }

        public void PauseGame()
        {
            CurrentState = GAME_STATE.PAUSED;
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            CurrentState = GAME_STATE.PLAYING;
            Time.timeScale = 1f;
        }

        public void GameOver()
        {
            CurrentState = GAME_STATE.GAMEOVER;
            Time.timeScale = 0f;
        }
    }
}

