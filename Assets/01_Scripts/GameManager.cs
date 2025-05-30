using System;
using Unity.VisualScripting;
using UnityEngine;

namespace WallDefense
{
    public class GameManager : MonoBehaviour
    {
        public WallManager wallManager;
        public DayNightManager dayNightManager;
        public SaveManager saveManager;
        [SerializeField] GameState currentState;
        [SerializeField] UIView currentView, previousNotebookView;
        public GameObject startView, mainView, gameOverView;
        public GameObject[] mainSubViews;
        public delegate void OnStartDelegate();
        public delegate void OnMainDelegate();
        public delegate void OnGameOverDelegate();
        public static event OnStartDelegate OnStart;
        public static event OnMainDelegate OnMain;
        public static event OnGameOverDelegate OnGameOver;

        enum GameState
        {
            start, gameOver, main
        }
        [Serializable]
        public enum UIView
        {
            main, town, management, info, dialogue, morse, none
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            OpenStart();
        }


        // Update is called once per frame
        void Update()
        {

        }

        public void OpenStart()
        {
            currentState = GameState.start;
            currentView = UIView.none;
            startView.SetActive(true);
            mainView.SetActive(false);
            gameOverView.SetActive(false);
            OnStart?.Invoke();
        }
        public void OpenMain()
        {
            currentState = GameState.main;
            currentView = UIView.main;
            startView.SetActive(false);
            mainView.SetActive(true);
            gameOverView.SetActive(false);
            OnMain?.Invoke();
        }
        public void OpenGameOver()
        {
            currentState = GameState.gameOver;
            currentView = UIView.none;
            previousNotebookView = UIView.none;
            startView.SetActive(false);
            mainView.SetActive(false);
            gameOverView.SetActive(true);
            OnGameOver?.Invoke();
        }
        public void OpenNotebookView()
        {
            if (previousNotebookView == UIView.dialogue || previousNotebookView == UIView.info || previousNotebookView == UIView.management)
            {
                OpenSubView(previousNotebookView);
            }
            else
            {
                OpenSubView(UIView.management);
            }
        }
        public void OpenSubView(UIViewComponent viewComponent)
        {
            UIView view = viewComponent.uiView;
            OpenSubView(view);
        }
        public void OpenSubView(UIView view)
        {
            if (currentView == UIView.dialogue || currentView == UIView.info || currentView == UIView.management)
            {
                previousNotebookView = currentView;
            }
            CloseSubViews();
            currentView = view;
            mainSubViews[(int)view].SetActive(true);
        }

        void CloseSubViews()
        {
            foreach (GameObject obj in mainSubViews)
            {
                obj.SetActive(false);
            }
        }
    }
}
