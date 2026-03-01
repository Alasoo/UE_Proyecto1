using System;
using System.Collections;
using System.Collections.Generic;
using MyUI.Panels;
using UnityEngine;

namespace MyUI
{
    public class MenuStateMachine : Singleton<MenuStateMachine>
    {
        [SerializeField] private List<MenuState> allMenu = new();

        public MenuState startMenu;

        private MenuState currentState;
        private Stack<MenuState> menuHistory = new();

        public event Action OnClickStart;


        public void OpenMenu(MenuState _currentState)
        {
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
            OpenMenu(startMenu);


        }


        public void StartGame()
        {
            OnClickStart?.Invoke();
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
            }
        }

    }
}
