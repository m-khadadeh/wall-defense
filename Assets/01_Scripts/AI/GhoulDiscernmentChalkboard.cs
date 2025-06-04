using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "GhoulDiscernmentChalkboard", menuName = "Scriptable Objects/AI/Ghoul Discernment Chalkboard")]
  public class GhoulDiscernmentChalkboard : ScriptableObject
  {
    [SerializeField] private List<Ghoul> _ghoulTypes;
    [SerializeField] private WorldState _topWallAttackPossibilities;
    [SerializeField] private WorldState _middleWallAttackPossibilities;
    [SerializeField] private WorldState _bottomWallAttackPossibilities;
    private HashSet<Ghoul> _ghoulPossibilities;
    public HashSet<Ghoul> ResearchedGhoulImpossibilites { get; private set; }
    public HashSet<Ghoul> Possibilities => _ghoulPossibilities;
    private Dictionary<string, HashSet<Ghoul>> _ghoulsPerClueString;
    [SerializeField] private GhoulSelector _ghoulSelector;
    private Dictionary<WallSegmentName, WorldState> _wallAttackPossibilityCounts;
    public Dictionary<WallSegmentName, HashSet<DamageParameters.Type>> WallAttackPossibilities { get; private set; }
    private Dictionary<WallSegmentName, Dictionary<DamageParameters.Type, WorldState>> _wallAttackBooleanPossibilities;
    [SerializeField] private WorldState _topCorrosiveBoolean;
    [SerializeField] private WorldState _topFinesseBoolean;
    [SerializeField] private WorldState _topBludgeoningBoolean;
    [SerializeField] private WorldState _middleCorrosiveBoolean;
    [SerializeField] private WorldState _middleFinesseBoolean;
    [SerializeField] private WorldState _middleBludgeoningBoolean;
    [SerializeField] private WorldState _bottomCorrosiveBoolean;
    [SerializeField] private WorldState _bottomFinesseBoolean;
    [SerializeField] private WorldState _bottomBludgeoningBoolean;
    [SerializeField] private WorldState _ghoulPossibilityAmount;
    private WallSegmentName[] _wallSegments;

    public void Initialize()
    {
      _ghoulsPerClueString = new Dictionary<string, HashSet<Ghoul>>();
      _wallAttackPossibilityCounts = new Dictionary<WallSegmentName, WorldState>();
      _wallAttackPossibilityCounts[WallSegmentName.top] = _topWallAttackPossibilities;
      _wallAttackPossibilityCounts[WallSegmentName.middle] = _middleWallAttackPossibilities;
      _wallAttackPossibilityCounts[WallSegmentName.bottom] = _bottomWallAttackPossibilities;
      _wallAttackBooleanPossibilities = new Dictionary<WallSegmentName, Dictionary<DamageParameters.Type, WorldState>>();
      _wallAttackBooleanPossibilities[WallSegmentName.top] = new Dictionary<DamageParameters.Type, WorldState>
      {
        { DamageParameters.Type.bludgeoning, _topBludgeoningBoolean },
        { DamageParameters.Type.finesse, _topFinesseBoolean },
        { DamageParameters.Type.corrosion, _topCorrosiveBoolean }
      };
      _wallAttackBooleanPossibilities[WallSegmentName.middle] = new Dictionary<DamageParameters.Type, WorldState>
      {
        { DamageParameters.Type.bludgeoning, _middleBludgeoningBoolean },
        { DamageParameters.Type.finesse, _middleFinesseBoolean },
        { DamageParameters.Type.corrosion, _middleCorrosiveBoolean }
      };
      _wallAttackBooleanPossibilities[WallSegmentName.bottom] = new Dictionary<DamageParameters.Type, WorldState>
      {
        { DamageParameters.Type.bludgeoning, _bottomBludgeoningBoolean },
        { DamageParameters.Type.finesse, _bottomFinesseBoolean },
        { DamageParameters.Type.corrosion, _bottomCorrosiveBoolean }
      };

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
      WallAttackPossibilities = new Dictionary<WallSegmentName, HashSet<DamageParameters.Type>>();
      _wallSegments = new WallSegmentName[] { WallSegmentName.top, WallSegmentName.middle, WallSegmentName.bottom };
      OnGhoulReset();
    }

    public void OnGhoulReset()
    {
      _ghoulPossibilities = _ghoulTypes.ToHashSet();
      ResearchedGhoulImpossibilites = new HashSet<Ghoul>();
      UpdateGhoulAttackPossibilities();
    }

    public void OnClueReceivedViaDialogue(string clueName)
    {
      _ghoulPossibilities.IntersectWith(_ghoulsPerClueString[clueName]);
      UpdateGhoulAttackPossibilities();
    }

    public void OnGhoulResearchComplete()
    {
      if (_ghoulPossibilities.Count > 1)
      {
        Ghoul[] removableGhouls = _ghoulPossibilities.Except(new HashSet<Ghoul>() { _ghoulSelector.CurrentGhoul }).ToArray();
        if (removableGhouls.Length == 1)
        {
          _ghoulPossibilities.Remove(removableGhouls[0]);
          ResearchedGhoulImpossibilites.Add(removableGhouls[0]);
        }
        else
        {
          int randomIndex = Random.Range(0, removableGhouls.Length);
          _ghoulPossibilities.Remove(removableGhouls[randomIndex]);
          ResearchedGhoulImpossibilites.Add(removableGhouls[randomIndex]);
        }
      }
      UpdateGhoulAttackPossibilities();
    }

    public void UpdateGhoulAttackPossibilities()
    {
      foreach (var wallSegment in _wallAttackBooleanPossibilities)
      {
        foreach (var attackPossibility in wallSegment.Value)
        {
          attackPossibility.Value.StateValue = 0;
        }
      }
      foreach (var segment in _wallSegments)
      {
        WallAttackPossibilities[segment] = new HashSet<DamageParameters.Type>();
      }
      foreach (var ghoul in _ghoulPossibilities)
      {
        if (ghoul.damageTargetMain != WallSegmentName.none && ghoul.parametersMain.type != DamageParameters.Type.none)
        {
          WallAttackPossibilities[ghoul.damageTargetMain].Add(ghoul.parametersMain.type);
        }
        if (ghoul.damageTargetSecondary != WallSegmentName.none && ghoul.parametersSecondary.type != DamageParameters.Type.none)
        {
          WallAttackPossibilities[ghoul.damageTargetSecondary].Add(ghoul.parametersSecondary.type);
        }
      }
      foreach (var wallAttackPossibilitySet in WallAttackPossibilities)
      {
        _wallAttackPossibilityCounts[wallAttackPossibilitySet.Key].StateValue = wallAttackPossibilitySet.Value.Count;
        foreach (var wallSegmentAttackPossibility in wallAttackPossibilitySet.Value)
        {
          _wallAttackBooleanPossibilities[wallAttackPossibilitySet.Key][wallSegmentAttackPossibility].StateValue = 1;
        }
      }
      _ghoulPossibilityAmount.StateValue = _ghoulPossibilities.Count();
    }
  }
}
