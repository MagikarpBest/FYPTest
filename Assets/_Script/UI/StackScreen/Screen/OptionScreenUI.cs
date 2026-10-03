using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionScreenUI : ScreenBase
{
    // [SerializeField] private UI_ButtonBehaviour _closeButton;
    [Header("Sound Settings")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private TMP_Text _masterPercentageText;
    [SerializeField] private TMP_Text _soundPercentageText;
    [SerializeField] private TMP_Text _musicPercentageText;
    private float _lastPreviewTime;
    private const float PREVIEW_COOLDOWN = 0.1f;

    private void Start()
    {
        InitAudioSettings();
        // _closeButton?.AddClickListener(OnClickClose);

        _soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);
        _musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        _masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
    }

    private void OnDestroy()
    {
        _soundSlider.onValueChanged.RemoveListener(OnSoundSliderChanged);
        _musicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
        _masterSlider.onValueChanged.RemoveListener(OnMasterSliderChanged);
    }

    #region Init

    private void InitAudioSettings()
    {
        float masterVolume = AudioService.GetMasterVolume();
        float soundVolume = AudioService.GetSFXVolume();
        float musicVolume = AudioService.GetMusicVolume();

        _masterSlider.SetValueWithoutNotify(masterVolume);
        _soundSlider.SetValueWithoutNotify(soundVolume);
        _musicSlider.SetValueWithoutNotify(musicVolume);

        UpdateMasterText(masterVolume);
        UpdateSoundText(soundVolume);
        UpdateMusicText(musicVolume);
    }

    #endregion

    #region Slider Callbacks

    private void OnMasterSliderChanged(float value)
    {
        AudioService.SetMasterVolume(value);
        UpdateMasterText(value);
    }

    private void OnSoundSliderChanged(float value)
    {
        AudioService.SetSFXVolume(value);
        UpdateSoundText(value);

        if (Time.time - _lastPreviewTime > PREVIEW_COOLDOWN)
        {
            _lastPreviewTime = Time.time;
            // TODO
            // AudioService.PlaySFX("SFX_click_1", 0.6f);
        }
    }

    private void OnMusicSliderChanged(float value)
    {
        AudioService.SetMusicVolume(value);
        UpdateMusicText(value);
    }

    #endregion

    #region UI Helpers
    private void UpdateMasterText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        _masterPercentageText.text = $"{percent}%";
    }

    private void UpdateSoundText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        _soundPercentageText.text = $"{percent}%";
    }

    private void UpdateMusicText(float value)
    {
        int percent = Mathf.RoundToInt(value * 100f);
        _musicPercentageText.text = $"{percent}%";
    }

    #endregion

    #region Close

    private void OnClickClose()
    {
        // TODO
        // AudioService.PlaySFX("SFX_click_1");
        // RequestClose();
    }

    #endregion
}