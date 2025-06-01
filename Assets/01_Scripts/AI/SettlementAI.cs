using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "SettlementAI", menuName = "Scriptable Objects/AI/SettlementAI")]
  public class SettlementAI : ScriptableObject
  {
    private VariableStorageBehaviour _variableStorage => _dialogueManager.VariableStorage;
    private ColonyData _colony;
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private List<Ghoul> _ghoulTypes;
    private HashSet<Ghoul> _ghoulPossibilities;
    private HashSet<Ghoul> _researchedGhoulImpossibilites;
    private Dictionary<string, HashSet<Ghoul>> _ghoulsPerClueString;

    public void Initialize(ColonyData data)
    {
      _ghoulPossibilities = _ghoulTypes.ToHashSet();
      _ghoulsPerClueString = new Dictionary<string, HashSet<Ghoul>>();
      foreach (var ghoul in _ghoulTypes)
      {
        Clue[] clues = new Clue[] { ghoul.clueMain, ghoul.clueSecondary };
        foreach (var clue in clues)
        {
          if (!_ghoulsPerClueString.ContainsKey(clue.name))
          {
            _ghoulsPerClueString.Add(clue.name, new HashSet<Ghoul>());
          }
          _ghoulsPerClueString[clue.name].Add(ghoul);
        }
      }

      _colony = data;
      _colony.Subscribe(OnGiftReceived);
    }

    public void OnHour(int hour)
    {

    }
    private void OnGiftReceived(List<ItemType> items)
    {

    }

    public void OnClueReceivedViaDialogue(string clueName)
    {
      _ghoulPossibilities.IntersectWith(_ghoulsPerClueString[clueName]);
    }
  }
}
