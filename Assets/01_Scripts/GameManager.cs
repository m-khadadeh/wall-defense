using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

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
        public TaskManager taskManager;
        public InventoryUI inventoryUI;
        public List<ColonyData> aiColonies;
        public ColonyData playerColony;
        public List<AI.SettlementAI> settlementAIs;
        public DialogueManager dialogueManager;
        public DialogueRunner dialogueRunner;
        public Capitol theCapitol;

        public GhoulSelector ghoulSelector;
        public Pocketwatch watch;

        public Button dialogueScreenBackButton;
        

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
            settlementAIs.OrderBy(_ => UnityEngine.Random.Range(0, int.MaxValue));
            int selectedAI = 0;
            foreach (var colony in aiColonies)
            {
                // AI Colony
                colony.AIController = settlementAIs[selectedAI];
                selectedAI++;
                colony.Inventory.Initialize();
            }
            playerColony.Inventory.Initialize();
            List<Action> addCallbacks = new List<Action>() { taskManager.CheckAllTaskAutoButtons };
            List<Action> removeCallbacks = new List<Action>() { taskManager.CheckAllTaskAutoButtons };
            inventoryUI.Initialize(addCallbacks, removeCallbacks);
            taskManager.Initialize(UpdateWatch);
            taskManager.SetInfo(null);
            dialogueManager.Initialize(dialogueRunner, aiColonies, playerColony, ContinueSetUp);
        }

        private void ContinueSetUp()
        {
            foreach (var colony in aiColonies)
            {
                colony.Initialize();
            }
            playerColony.Initialize();
            ghoulSelector.SelectGhoul();
            watch.OnAdvanceHour();
            UpdateWatch();
            theCapitol.Initialize();
            dialogueRunner.onDialogueStart.AddListener(() => dialogueScreenBackButton.enabled = false);
            dialogueRunner.onDialogueComplete.AddListener(() => dialogueScreenBackButton.enabled = true);
        }

        public void OnNewDay()
        {
            ghoulSelector.OnDayStart();
            theCapitol.OnNewDay();
            dialogueRunner.VariableStorage.SetValue("$player_called_capitol_today", false);
            dialogueRunner.VariableStorage.SetValue("$player_called_ws2_today", false);
            dialogueRunner.VariableStorage.SetValue("$player_called_ws3_today", false);
        }
        public void OnBeforeHour(int hour)
        {
            foreach (var colony in aiColonies)
            {
                colony.OnBeforeHour(hour);
            }
            playerColony.OnBeforeHour(hour);
            taskManager.OnIncrementHour();
        }
        public void OnAfterHour(int hour)
        {
            foreach (var colony in aiColonies)
            {
                colony.OnAfterHour(hour);
            }
            playerColony.OnAfterHour(hour);
            ghoulSelector.OnHour(hour);
            watch.OnAdvanceHour();
            UpdateWatch();
            dialogueManager.OnAfterHour(hour, currentView);
        }
        public void UpdateWatch()
        {
            int shortestTime = taskManager.GetShortestTimeToTaskCompletion();
            watch.UpdateXButton(shortestTime);
        }
        public void OnNightHour(int hour)
        {

        }

        public void AdvanceOneHour()
        {
            dayNightManager.AdvanceHour();
        }

        public void AdvanceToTaskComplete()
        {
            int hoursToAdvance = taskManager.GetShortestTimeToTaskCompletion();
            for (int i = 0; i < hoursToAdvance && !dialogueManager.NodeQueued; i++)
            {
                AdvanceOneHour();
            }
        }


        // Update is called once per frame
        void Update()
        {
            watch.CanClickButtons(!dialogueManager.NodeQueued);
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
