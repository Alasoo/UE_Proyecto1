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




        public override void Init()
        {
            backButton.onClick.AddListener(MenuStateMachine.Instance.GoBack);

            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            ambientSlider.onValueChanged.AddListener(SetAmbientVolume);
            effectsSlider.onValueChanged.AddListener(SetEffectsVolume);
        }

        private void SetMasterVolume(float volume)
        {
            Audios.Instance.audioMixer.SetFloat(Audios.Instance.masterVolumeParam, volume);
            masterText.text = volume.ToString();
        }

        private void SetAmbientVolume(float volume)
        {
            Audios.Instance.audioMixer.SetFloat(Audios.Instance.ambientVolumeParam, volume);
            ambientTText.text = volume.ToString();
        }

        private void SetEffectsVolume(float volume)
        {
            Audios.Instance.audioMixer.SetFloat(Audios.Instance.effectsVolumeParam, volume);
            effectsText.text = volume.ToString();
        }


    }
}
