using HM.CodeBase;
using TMPro;
using UnityEngine;

namespace HM.UI.InGame
{
    public class InGame_View : AView
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _comboText;
        [SerializeField] private TextMeshProUGUI _timerText;

        [Space(5f), Header("Sub View")]
        [SerializeField] private CountDown_View _countDownView;

        public CountDown_View CountDownView => _countDownView;


        public void UpdateScore(int score)
        {
            _scoreText.text = $"SCORE : {score}";
        }
    }
}

