using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace WallDefense
{
  public abstract class SettlementAI : ScriptableObject
  {
    protected VariableStorageBehaviour _variableStorage => _dialogueManager.VariableStorage;
    protected ColonyData _colony;
    [SerializeField] protected DialogueManager _dialogueManager;

    public virtual void Initialize(ColonyData data)
    {
      _colony = data;
      _colony.Subscribe(OnGiftReceived);
    }
    public abstract void OnHour(int hour);
    protected abstract void OnGiftReceived(List<ItemType> items);
  }
}
