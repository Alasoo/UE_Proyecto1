using System;
using System.Collections;
using System.Collections.Generic;
using Controller.Player;
using MyUI.Panels;
using UnityEngine;

namespace MyUI
{
    public class MenuStateMachine : Singleton<MenuStateMachine>
    {
        [SerializeField] private List<MenuState> allMenu = new();
        [SerializeField] private bool openAtStart = false;

        public MenuState startMenu;

        private MenuState currentState;
        private Stack<MenuState> menuHistory = new();

        public bool opened { get; private set; } = false;



        private void OnDestroy()
        {
            if (PlayerStateMachine.Instance != null)
                PlayerStateMachine.Instance.inputReader.OnCancelEvent -= OnCancel;
            Time.timeScale = 1f;
        }



        public void OpenMenu(MenuState _currentState)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            //_= Time.Freeze();
            Time.timeScale = 0f;

            opened = true;
            currentState = _currentState;
            currentState?.Enter();
        }

        public void CloseAll()
        {
            foreach (var menu in menuHistory)
            {
                menu?.ForceClose();
            }
            menuHistory.Clear();
            currentState?.ForceClose();
            currentState = null;
        }


        void Start()
        {
            foreach (var menu in allMenu)
                menu.Init();

            Init();
        }


        private void Init()
        {
            if (PlayerStateMachine.Instance != null)
            {
                PlayerStateMachine.Instance.inputReader.OnCancelEvent += OnCancel;
            }
            if (!openAtStart) return;

            OpenMenu(startMenu);
        }



        public void ChangeMenu(MenuState newMenu)
        {
            if (currentState != null)
            {
                menuHistory.Push(currentState); // Guardar el menú actual antes de cambiar
                currentState.Exit();
            }

            currentState = newMenu;
            currentState?.Enter();
        }

        public void GoBack()
        {
            if (menuHistory.Count > 0)
            {
                MenuState previousMenu = menuHistory.Pop(); // Recuperar el menú anterior
                currentState.Exit();
                currentState = previousMenu;
                currentState.Enter();
                return;
            }
            else if (currentState != null)
            {
                currentState.Exit();
                currentState = null;
                opened = false;
                Time.timeScale = 1f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }


        private void OnCancel()
        {
            if (opened)
                GoBack();
            else
            {
                OpenMenu(startMenu);
            }
        }

    }
}
