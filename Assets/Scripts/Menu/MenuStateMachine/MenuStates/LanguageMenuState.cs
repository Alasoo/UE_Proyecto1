using System;
using System.Collections;
using System.Collections.Generic;
using AudioController;
using MyUI.Panels;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;


namespace MyUI.OptionButtons
{
    public class LanguageMenuState : MenuState
    {
        [SerializeField] private List<LocalizedString> dropdownOptions = new List<LocalizedString>();
        [SerializeField] private TMP_Dropdown languageDropdown;

        [Header("BUTTONS")]
        [SerializeField] private Button backButton;


        private bool changingLenguage = false;
        private readonly string LANG_KEY = "language";

        private bool isInitialized = false;

        public override void Init()
        {
            backButton.onClick.AddListener(MenuStateMachine.Instance.GoBack);

            if (dropdownOptions.Count == 0) return;
            languageDropdown.ClearOptions();

            // Suscríbete al evento de cambio de idioma.
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            int id = LocalizationSettings.SelectedLocale == null ? 0 : LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);

            if (PlayerPrefs.HasKey(LANG_KEY))
                id = PlayerPrefs.GetInt(LANG_KEY, 0);

            ForceOpen();
            SetLenguage(id);

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
            languageDropdown.onValueChanged.RemoveAllListeners();

            // Crea una lista temporal para las nuevas opciones localizadas.
            List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

            foreach (var localizedString in dropdownOptions)
            {
                // Obtiene la string localizada. NOTA: Esto es síncrono y puede congelar el hilo si no está precargada.
                string localizedText = localizedString.GetLocalizedString();
                newOptions.Add(new TMP_Dropdown.OptionData(localizedText));
            }

            // Aplica las nuevas opciones al Dropdown.
            languageDropdown.options = newOptions;
            languageDropdown.value = LocalizationSettings.SelectedLocale == null ? 0 : LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
            languageDropdown.RefreshShownValue();

            languageDropdown.onValueChanged.AddListener(SetLenguage);
        }


        public void SetLenguage(int index)
        {
            if (changingLenguage) return;
            if (isInitialized)
                Audios.Instance.PlayClickButton();

            StartCoroutine(ChangeLenguage(index));
        }

        private IEnumerator ChangeLenguage(int index)
        {
            changingLenguage = true;

            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
            PlayerPrefs.SetInt(LANG_KEY, index);
            changingLenguage = false;

            if (!isInitialized)
            {
                isInitialized = true;
                ForceClose();
            }
        }



    }
}
