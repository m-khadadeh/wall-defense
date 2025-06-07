using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "GhoulSelector", menuName = "Scriptable Objects/GhoulSelector")]
  public class GhoulSelector : ScriptableObject
  {
    [SerializeField] private List<Ghoul> _possibleGhouls;
    [SerializeField] private List<Clue> _allClues;
    [SerializeField] private List<AI.GhoulDiscernmentChalkboard> _chalkBoards;
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private List<ColonyData> _colonies;
    public Ghoul CurrentGhoul { get; private set; }
    private int _attackHour;
    private bool _attacked;
    public bool HasAttacked => _attacked;
    public delegate void ClueFound();
    public static event ClueFound ClueFoundEvent;
    public delegate void ClearClue();
    public static event ClearClue ClearClueEvent;

    public void Initialize()
    {
      _attacked = false;
    }

    public void SelectGhoul()
    {
      CurrentGhoul = _possibleGhouls[Random.Range(0, _possibleGhouls.Count)];
      _attackHour = Random.Range(CurrentGhoul.hourRange.x, CurrentGhoul.hourRange.y + 1) % 24;
      foreach (var chalkboard in _chalkBoards)
      {
        chalkboard.OnGhoulReset();
      }
      WipeClues();
      _dialogueManager.VariableStorage.SetValue("$ghoul_type_morse_text", CurrentGhoul.morseTextRepresentation);
    }

    public void WipeClues()
    {
      foreach (var clue in _allClues)
      {
        clue.found = false;
      }
      _dialogueManager.VariableStorage.SetValue("$first_clue", "");
      _dialogueManager.VariableStorage.SetValue("$first_clue_morse_text", "");
      _dialogueManager.VariableStorage.SetValue("$second_clue", "");
      _dialogueManager.VariableStorage.SetValue("$second_clue_morse_text", "");
      ClearClueEvent?.Invoke();
    }

    public void OnDayStart()
    {
      _attacked = false;
    }

    public Clue FindClue()
    {
      int foundClueIndex = Random.Range(0, 2);
      if (!CurrentGhoul.clueMain.found)
      {
        if (foundClueIndex == 0 || CurrentGhoul.clueSecondary.found)
        {
          CurrentGhoul.clueMain.found = true;
          _dialogueManager.VariableStorage.SetValue("$first_clue", CurrentGhoul.clueMain.name);
          _dialogueManager.VariableStorage.SetValue("$first_clue_morse_text", CurrentGhoul.clueMain.morseTextRepresentation);
          ClueFoundEvent?.Invoke();
          return CurrentGhoul.clueMain;
        }
        else if (!CurrentGhoul.clueSecondary.found)
        {
          CurrentGhoul.clueSecondary.found = true;
          _dialogueManager.VariableStorage.SetValue("$second_clue", CurrentGhoul.clueSecondary.name);
          _dialogueManager.VariableStorage.SetValue("$second_clue_morse_text", CurrentGhoul.clueSecondary.morseTextRepresentation);
          ClueFoundEvent?.Invoke();
          return CurrentGhoul.clueSecondary;
        }
      }
      else if (!CurrentGhoul.clueSecondary.found)
      {
        CurrentGhoul.clueSecondary.found = true;
        _dialogueManager.VariableStorage.SetValue("$second_clue", CurrentGhoul.clueSecondary.name);
        _dialogueManager.VariableStorage.SetValue("$second_clue_morse_text", CurrentGhoul.clueSecondary.morseTextRepresentation);
        ClueFoundEvent?.Invoke();
        return CurrentGhoul.clueSecondary;
      }
      return null;
    }

    public void OnHour(int hour)
    {
      if (hour == _attackHour && !_attacked)
      {
        foreach (var colony in _colonies)
        {
          CurrentGhoul.targetWall = colony.Wall;
          if (CurrentGhoul.MainAttack() || CurrentGhoul.SecondaryAttack())
          {
            // Lose condition
            colony.Die();
          }
        }
        SelectGhoul();
        _attacked = true;
      }
    }
  }
}
