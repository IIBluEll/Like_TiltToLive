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

