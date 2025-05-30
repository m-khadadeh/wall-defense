using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "GhoulSelector", menuName = "Scriptable Objects/GhoulSelector")]
  public class GhoulSelector : ScriptableObject
  {
    [SerializeField] private List<Ghoul> _possibleGhouls;
    [SerializeField] private List<Clue> _allClues;
    private Ghoul _currentGhoul;

    public void SelectGhoul()
    {
      _currentGhoul = _possibleGhouls[Random.Range(0, _possibleGhouls.Count)];
      WipeClues();
    }

    public void WipeClues()
    {
      foreach (var clue in _allClues)
      {
        clue.found = false;
      }
    }

    public Clue FindClue()
    {
      int foundClueIndex = Random.Range(0, 2);
      if (!_currentGhoul.clueMain.found)
      {
        if (foundClueIndex == 0 || _currentGhoul.clueSecondary.found)
        {
          _currentGhoul.clueMain.found = true;
          return _currentGhoul.clueMain;
        }
        else if (!_currentGhoul.clueSecondary.found)
        {
          _currentGhoul.clueSecondary.found = true;
          return _currentGhoul.clueSecondary;
        }
      }
      else if (!_currentGhoul.clueSecondary.found)
      {
        _currentGhoul.clueSecondary.found = true;
        return _currentGhoul.clueSecondary;
      }
      return null;
    }
  }
}
