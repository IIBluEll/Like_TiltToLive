using DG.Tweening;
using TMPro;
using UnityEngine;

namespace HM.UI.InGame
{
    public class CountDown_View : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _countDownText;
        [SerializeField] private CanvasGroup _canvasGroup;

        public void OnEnable()
        {
            _countDownText.text = "";
        }

        public void ShowCountDown(int count)
        {
            gameObject.SetActive(true);
            _countDownText.text = count.ToString();

            _countDownText.transform.localScale = Vector3.one * 0.5f;
            _countDownText.transform.DOScale(1f,0.3f).SetEase(Ease.OutBack);
        }

        public void ShowStartMessage()
        {
            _countDownText.text = "START!";
            _countDownText.transform.DOPunchScale(Vector3.one * 0.2f , 0.5f , 10 , 1);
        }

        public void Hide()
        {
            _canvasGroup.DOFade(0f , 0.5f).OnComplete(() =>
            {
                gameObject.SetActive(false);
                _canvasGroup.alpha = 1f;
            });
        }
    }
}

