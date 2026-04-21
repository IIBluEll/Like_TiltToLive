using HM.CodeBase;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
