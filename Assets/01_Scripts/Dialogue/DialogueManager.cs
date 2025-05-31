using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "DialogueManager", menuName = "Scriptable Objects/DialogueManager")]
  public class DialogueManager : ScriptableObject
  {
    private DialogueRunner _runner;
    private Queue<string> _queuedDialogueNodes;
    public VariableStorageBehaviour VariableStorage => _runner.VariableStorage;
    public void Initialize(DialogueRunner runner)
    {
      _runner = runner;
      _queuedDialogueNodes = new Queue<string>();
    }

    public void OnAfterHour()
    {
      if (_queuedDialogueNodes.Count > 0)
      {
        string nextNode = _queuedDialogueNodes.Dequeue();
        _runner.StartDialogue(nextNode);
      }
    }
  }
}
