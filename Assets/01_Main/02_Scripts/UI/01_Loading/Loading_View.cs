using HM.CodeBase;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 초기 로딩 화면의 진행률 바 및 연출을 담당하는 뷰
/// </summary>
public class Loading_View : AView
{
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private TextMeshProUGUI _progressText;

    public override void Open()
    {
        _loadingSlider.value = 0f;
        _progressText.text = $"0%";
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }

    public void UpdateProgressUI(float progress)
    {
        if ( _loadingSlider == null || _progressText == null )
            return;

        _loadingSlider.value = progress;
        _progressText.text = $"{( progress * 100 ):F0}%";
    }
}
