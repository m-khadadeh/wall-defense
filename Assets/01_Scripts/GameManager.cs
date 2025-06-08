using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

namespace WallDefense
{
    public class GameManager : MonoBehaviour
    {
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

        public List<Button> dialogueBlockButtons;
        public ScreenFader screenFader;

        public Button[] buttonsToLockBeforeFirstDialogue;
        private bool buttonsUnlockedYet;

        public TutorialKun tutorialKun;

        public Task[] tasks;
        public GameObject topWallSegment;
        public GameObject middleWallSegment;
        public GameObject bottomWallSegment;


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
            dayNightManager.Initialize();
            tutorialKun.Initialize();
            OpenStartFirstCall();
            foreach (var buttonToLock in buttonsToLockBeforeFirstDialogue)
            {
                buttonToLock.interactable = false;
            }
            buttonsUnlockedYet = false;
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
            dialogueManager.Initialize(dialogueRunner, aiColonies, playerColony, ContinueSetUp, dayNightManager.currentHour);
        }

        void OnDestroy()
        {
            dialogueManager.Unbind();
            foreach (var task in tasks)
            {
                task.Unbind();
            }
        }

        private void ContinueSetUp()
        {
            foreach (var colony in aiColonies)
            {
                colony.Initialize();
            }
            playerColony.Initialize();
            ghoulSelector.Initialize();
            ghoulSelector.SelectGhoul();
            watch.OnAdvanceHour();
            UpdateWatch();
            theCapitol.Initialize();
            dialogueRunner.onDialogueStart.AddListener(() =>
            {
                foreach (var button in dialogueBlockButtons)
                {
                    button.interactable = false;
                }
            });
            dialogueRunner.onDialogueComplete.AddListener(() =>
            {
                foreach (var button in dialogueBlockButtons)
                {
                    button.interactable = true;
                }
            });
            tutorialKun.ShowStartArrows(true);
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
            tutorialKun.ShowStartArrows(false);
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
            bottomWallSegment.SetActive(!playerColony.Wall.bottom.IsDestroyed());
            middleWallSegment.SetActive(!playerColony.Wall.middle.IsDestroyed());
            topWallSegment.SetActive(!playerColony.Wall.top.IsDestroyed());
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
            if (dialogueManager.NodeQueued && !tutorialKun.MorseArrowsUp)
            {
                tutorialKun.ShowRadioArrows(true);
            }
            else if (!dialogueManager.NodeQueued && tutorialKun.MorseArrowsUp)
            {
                tutorialKun.ShowRadioArrows(false);
            }
            watch.CanClickButtons(!dialogueManager.NodeQueued && DialogBox.Instance == null);
            if (!buttonsUnlockedYet && dialogueManager.NodeQueued)
            {
                buttonsUnlockedYet = true;
                foreach (var button in buttonsToLockBeforeFirstDialogue)
                {
                    button.interactable = true;
                }
            }
        }
        public void OpenStartFirstCall()
        {
            StartCoroutine(OpenStartFirstCallCoroutine());
        }
        IEnumerator OpenStartFirstCallCoroutine()
        {
            currentState = GameState.start;
            currentView = UIView.none;
            startView.SetActive(true);
            mainView.SetActive(false);
            gameOverView.SetActive(false);
            OnStart?.Invoke();
            yield return screenFader.EndFadeCoroutine();
        }

        public void OpenStart()
        {
            StartCoroutine(OpenStartCoroutine());
        }
        IEnumerator OpenStartCoroutine()
        {
            yield return screenFader.StartFadeCoroutine();
            currentState = GameState.start;
            currentView = UIView.none;
            startView.SetActive(true);
            mainView.SetActive(false);
            gameOverView.SetActive(false);
            OnStart?.Invoke();
            yield return screenFader.EndFadeCoroutine();
        }
        public void OpenMain()
        {
            StartCoroutine(OpenMainCoroutine());
        }
        IEnumerator OpenMainCoroutine()
        {
            yield return screenFader.StartFadeCoroutine();
            currentState = GameState.main;
            currentView = UIView.main;
            startView.SetActive(false);
            mainView.SetActive(true);
            gameOverView.SetActive(false);
            OnMain?.Invoke();
            yield return screenFader.EndFadeCoroutine();
        }
        public void OpenGameOver()
        {
            StartCoroutine(OpenGameOverCoroutine());
        }
        IEnumerator OpenGameOverCoroutine()
        {
            yield return screenFader.StartFadeCoroutine();
            currentState = GameState.gameOver;
            currentView = UIView.none;
            previousNotebookView = UIView.none;
            startView.SetActive(false);
            mainView.SetActive(false);
            gameOverView.SetActive(true);
            OnGameOver?.Invoke();
            yield return screenFader.EndFadeCoroutine();
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
            StartCoroutine(OpenSubViewCoroutine(view));
        }

        IEnumerator OpenSubViewCoroutine(UIView view)
        {
            yield return screenFader.StartFadeCoroutine();
            if (currentView == UIView.dialogue || currentView == UIView.info || currentView == UIView.management)
            {
                previousNotebookView = currentView;
            }
            CloseSubViews();
            currentView = view;
            mainSubViews[(int)view].SetActive(true);
            yield return screenFader.EndFadeCoroutine();
            if (currentView == UIView.morse)
            {
                dialogueManager.OnScreenEntry();
            }
        }

        void CloseSubViews()
        {
            foreach (GameObject obj in mainSubViews)
            {
                obj.SetActive(false);
            }
        }

        public void ResetScene()
        {
            SceneManager.LoadScene("00_Main");
        }

        public void RequestQuit()
        {
            DialogBox.QueueDialogueBox(new DialogueBoxParameters(
                GameObject.Find("Canvas").transform,
                "<align=\"center\"><size=40>Do you wish to quit the game?</size>",
                new string[] { "Yes", "No" },
                new DialogBox.ButtonEventHandler[] { () => { QuitGame(); }, () => { } }
              ));
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
