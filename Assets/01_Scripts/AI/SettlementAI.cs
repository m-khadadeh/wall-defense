using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "SettlementAI", menuName = "Scriptable Objects/AI/SettlementAI")]
  public class SettlementAI : ScriptableObject
  {
    private VariableStorageBehaviour _variableStorage => _dialogueManager.VariableStorage;
    private ColonyData _colony;
    [SerializeField] private DialogueManager _dialogueManager;

    public void Initialize(ColonyData data)
    {
      _colony = data;
      _colony.Subscribe(OnGiftReceived);
    }

    public void OnHour(int hour)
    {

    }
    private void OnGiftReceived(List<ItemType> items)
    {

    }
    public void OnClueReceivedViaDialogue(string clueName) => _colony.OnClueReceivedViaDialogue(clueName);
  }
}
