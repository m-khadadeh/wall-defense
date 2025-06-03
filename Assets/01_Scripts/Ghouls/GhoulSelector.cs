using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "GhoulSelector", menuName = "Scriptable Objects/GhoulSelector")]
  public class GhoulSelector : ScriptableObject
  {
    [SerializeField] private List<Ghoul> _possibleGhouls;
    [SerializeField] private List<Clue> _allClues;
    public Ghoul CurrentGhoul { get; private set; }
    private int _attackHour;
    private bool _attacked;
    public bool HasAttacked => _attacked;

    public void Initialize()
    {
      _attacked = false;
    }

    public void SelectGhoul()
    {
      CurrentGhoul = _possibleGhouls[Random.Range(0, _possibleGhouls.Count)];
      _attackHour = Random.Range(CurrentGhoul.hourRange.x, CurrentGhoul.hourRange.y + 1) % 24;
      WipeClues();
    }

    public void WipeClues()
    {
      foreach (var clue in _allClues)
      {
        clue.found = false;
      }
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
          return CurrentGhoul.clueMain;
        }
        else if (!CurrentGhoul.clueSecondary.found)
        {
          CurrentGhoul.clueSecondary.found = true;
          return CurrentGhoul.clueSecondary;
        }
      }
      else if (!CurrentGhoul.clueSecondary.found)
      {
        CurrentGhoul.clueSecondary.found = true;
        return CurrentGhoul.clueSecondary;
      }
      return null;
    }

    public void OnHour(int hour)
    {
      if (hour == _attackHour && !_attacked)
      {
        if (CurrentGhoul.MainAttack() || CurrentGhoul.SecondaryAttack())
        {
          // Lose condition
          Debug.Log("LOSE CONDITION");
        }
        else
        {
          SelectGhoul();
        }
        _attacked = true;
      }
    }
  }
}
