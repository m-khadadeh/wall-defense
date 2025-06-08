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
    private Dictionary<DamageParameters.Type, string> _defenseRep;

    public void Initialize()
    {
      _attacked = false;
      _defenseRep = new Dictionary<DamageParameters.Type, string>()
      {
        {DamageParameters.Type.none, "None"},
        {DamageParameters.Type.bludgeoning, "Anti-Bludgeoning"},
        {DamageParameters.Type.corrosion, "Anti-Corrosive"},
        {DamageParameters.Type.finesse, "Anti-Finesse"},
      };
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
        AudioManager.PlaySound("page_ghouls");
        foreach (var colony in _colonies)
        {
          CurrentGhoul.targetWall = colony.Wall;
          (bool, DamageParameters.Type) firstAttackDefended;
          bool firstAttackKills = CurrentGhoul.MainAttack(out firstAttackDefended);
          string attackMessage = $"<align=\"center\"><size=40>ATTACK REPORT</size>\n-------------------------------------\n<align=\"left\"><indent=5%> - {CurrentGhoul.firstAttackRep},";
          if (firstAttackDefended.Item1)
          {
            attackMessage += $" but its impact was lessened by the (now destroyed) <b>{_defenseRep[firstAttackDefended.Item2]}</b> defense.\n";
          }
          else
          {
            if (firstAttackDefended.Item2 != DamageParameters.Type.none)
            {
              attackMessage += $" obliterating the installed <b>{_defenseRep[firstAttackDefended.Item2]}</b> defense.\n";
            }
            else
            {
              attackMessage += " unimpeded by the lack of defense.\n";
            }
          }

          (bool, DamageParameters.Type) secondAttackDefended;
          bool secondAttackKills = false;
          if (!firstAttackKills)
          {
            secondAttackKills = CurrentGhoul.SecondaryAttack(out secondAttackDefended);
            if (CurrentGhoul.secondAttackRep != "")
            {
              attackMessage += $"<indent=5%> - {CurrentGhoul.secondAttackRep},";
              if (secondAttackDefended.Item1)
              {
                attackMessage += $" but its impact was lessened by the (now destroyed) <b>{_defenseRep[secondAttackDefended.Item2]}</b> defense.\n";
              }
              else
              {
                if (secondAttackDefended.Item2 != DamageParameters.Type.none)
                {
                  attackMessage += $" obliterating the installed <b>{_defenseRep[secondAttackDefended.Item2]}</b> defense.\n";
                }
                else
                {
                  attackMessage += " unimpeded by the lack of defense.\n";
                }
              }
            }
          }
          attackMessage += "<indent=0%><align=\"center\">-------------------------------------\n\n";
          attackMessage += "<size=40>DAMAGE REPORT</size>\n";
          attackMessage += "-------------------------------------\n";
          var topSegment = colony.Wall.GetSegmentFromName(WallSegmentName.top);
          attackMessage += $"<align=\"left\"><indent=5%> - Wall TOP: {topSegment.health} / {topSegment.maxhealth}. Currently installed defense: {_defenseRep[topSegment.currentDefenseType]}\n";
          var middleSegment = colony.Wall.GetSegmentFromName(WallSegmentName.middle);
          attackMessage += $"<indent=5%> - Wall MIDDLE: {middleSegment.health} / {middleSegment.maxhealth}. Currently installed defense: {_defenseRep[middleSegment.currentDefenseType]}\n";
          var bottomSegment = colony.Wall.GetSegmentFromName(WallSegmentName.bottom);
          attackMessage += $"<indent=5%> - Wall BOTTOM: {bottomSegment.health} / {bottomSegment.maxhealth}. Currently installed defense: {_defenseRep[bottomSegment.currentDefenseType]}\n";
          attackMessage += "<indent=0%>-------------------------------------";
          if (firstAttackKills || secondAttackKills)
          {
            // Lose condition
            colony.Die(CurrentGhoul.ghoulBreachRep);
          }
          else
          {
            if (colony.AIController == null)
            {
              // Player colony
              DialogBox.QueueDialogueBox(new DialogueBoxParameters(
                GameObject.Find("Canvas").transform,
                attackMessage,
                new string[] { "Understood." },
                new DialogBox.ButtonEventHandler[] { () => { } }
              ));
            }
          }
        }
        SelectGhoul();
        _attacked = true;
      }
    }
  }
}
