using System;
using AudioController;
using MyUI.Panels;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


namespace MyUI.OptionButtons
{
    public class AudioMenuState : MenuState
    {
        [Header("SLIDERS")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider ambientSlider;
        [SerializeField] private Slider effectsSlider;

        [Header("TEXTS")]
        [SerializeField] private TMP_Text masterText;
        [SerializeField] private TMP_Text ambientTText;
        [SerializeField] private TMP_Text effectsText;

        [Header("BUTTONS")]
        [SerializeField] private Button backButton;

        private const float minVolume = -80f;
        private const float maxVolume = 0f;


        public override void Init()
        {
            backButton.onClick.AddListener(MenuStateMachine.Instance.GoBack);

            LoadAudio();

            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            ambientSlider.onValueChanged.AddListener(SetAmbientVolume);
            effectsSlider.onValueChanged.AddListener(SetEffectsVolume);
        }

        private void LoadAudio()
        {
            float masterVolume = PlayerPrefs.GetFloat(Audios.Instance.masterVolumeParam, 100f);
            float ambientVolume = PlayerPrefs.GetFloat(Audios.Instance.ambientVolumeParam, 100f);
            float effectsVolume = PlayerPrefs.GetFloat(Audios.Instance.effectsVolumeParam, 100f);

            masterSlider.SetValueWithoutNotify(masterVolume);
            ambientSlider.SetValueWithoutNotify(ambientVolume);
            effectsSlider.SetValueWithoutNotify(effectsVolume);

            SetMasterVolume(masterVolume);
            SetAmbientVolume(ambientVolume);
            SetEffectsVolume(effectsVolume);
        }

        private void SetMasterVolume(float volume)
        {
            Audios.Instance.audioMixer.SetFloat(Audios.Instance.masterVolumeParam, ConvertVolume(volume));
            masterText.text = volume.ToString();
        }

        private void SetAmbientVolume(float volume)
        {
            Audios.Instance.audioMixer.SetFloat(Audios.Instance.ambientVolumeParam, ConvertVolume(volume));
            ambientTText.text = volume.ToString();
        }

        private void SetEffectsVolume(float volume)
        {
            Audios.Instance.audioMixer.SetFloat(Audios.Instance.effectsVolumeParam, ConvertVolume(volume));
            effectsText.text = volume.ToString();
        }

        public override void Exit()
        {
            base.Exit();

            PlayerPrefs.SetFloat(Audios.Instance.masterVolumeParam, masterSlider.value);
            PlayerPrefs.SetFloat(Audios.Instance.ambientVolumeParam, ambientSlider.value);
            PlayerPrefs.SetFloat(Audios.Instance.effectsVolumeParam, effectsSlider.value);
            PlayerPrefs.Save();
        }


        private float ConvertVolume(float volume)
        {
            volume = Mathf.Clamp(volume, 0f, 100f);
            return Mathf.Lerp(minVolume, maxVolume, volume / 100f);
        }


    }
}
