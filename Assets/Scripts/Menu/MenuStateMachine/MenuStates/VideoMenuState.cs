using System;
using System.Collections.Generic;
using System.Linq;
using AudioController;
using MyUI.Panels;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;


namespace MyUI.OptionButtons
{
    public class VideoMenuState : MenuState
    {
        [SerializeField] private List<LocalizedString> dropdownOptions = new List<LocalizedString>();
        [SerializeField] private TMP_Dropdown qualityDropdown;
        [SerializeField] private TMP_Dropdown resolutionDropdown;

        [Header("BUTTONS")]
        [SerializeField] private Button backButton;


        private readonly string QUALITY_KEY = "quality";
        private Resolution[] resolutions;
        private bool isInitialized = false;

        public override void Init()
        {
            backButton.onClick.AddListener(MenuStateMachine.Instance.GoBack);

            UpdateResolutionOptions();

            if (dropdownOptions.Count == 0) return;

            qualityDropdown.ClearOptions();
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

            int id = QualitySettings.GetQualityLevel();
            if (PlayerPrefs.HasKey(QUALITY_KEY))
                id = PlayerPrefs.GetInt(QUALITY_KEY, 0);

            SetQuality(id);

            UpdateDropdownOptions();
        }

        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }

        private void OnLocaleChanged(Locale newLocale)
        {
            UpdateDropdownOptions();
        }


        private void UpdateDropdownOptions()
        {
            qualityDropdown.onValueChanged.RemoveAllListeners();

            // Crea una lista temporal para las nuevas opciones localizadas.
            List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

            foreach (var localizedString in dropdownOptions)
            {
                // Obtiene la string localizada. NOTA: Esto es síncrono y puede congelar el hilo si no está precargada.
                string localizedText = localizedString.GetLocalizedString();
                newOptions.Add(new TMP_Dropdown.OptionData(localizedText));
            }

            // Aplica las nuevas opciones al Dropdown.
            qualityDropdown.options = newOptions;
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.RefreshShownValue();
            qualityDropdown.onValueChanged.AddListener(SetQuality);
        }


        public void SetQuality(int index)
        {
            if (isInitialized)
                Audios.Instance.PlayClickButton();

            QualitySettings.SetQualityLevel(index);
            isInitialized = true;
            PlayerPrefs.SetInt(QUALITY_KEY, index);
        }



        private void UpdateResolutionOptions()
        {
            resolutionDropdown.ClearOptions();

            // Obtenemos todas las resoluciones disponibles para el monitor
            resolutions = Screen.resolutions.Select(res => new Resolution { width = res.width, height = res.height }).Distinct().ToArray();
            // Nota: Si quieres incluir refresco (Hz), usa simplemente: resolutions = Screen.resolutions;

            List<string> options = new List<string>();
            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                // Detectamos cuál es la resolución actual para marcarla en el dropdown
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();

            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        public void SetResolution(int index)
        {
            Resolution resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

            if (isInitialized)
                Audios.Instance.PlayClickButton();
        }

    }
}
