using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace WallDefense
{
  public class WallManager : MonoBehaviour
  {
    [Header("Debug")]
    public bool triggerAttack;
    public bool debug_CollectClue;
    public bool debug_ApplyDefense;
    [Space(20)]
    [Header("Assignables")]
    public Wall maria;
    public Wall sina;
    public Wall rose;
    public DayNightManager dayNightManager;
    public float attackPercentage;
    public DamageParameters.Type defenseType;
    public Wall targetWallDefense;
    public WallSegmentName targetSegementDefense;
    public Ghoul[] ghouls;
    public Wall[] walls = new Wall[3];
    Ghoul activeGhoul;
    Clue clueMain, clueSecondary;
    bool clueMainFound, clueSecondaryFound;
    Wall ghoulTargetWall;
    int hoursUntilAttack;
    bool willAttackTonight;

    void Start()
    {
      walls[0] = maria;
      walls[1] = sina;
      walls[2] = rose;
      InitializeWalls();

    }


    // Update is called once per frame
    void Update()
    {

      if (debug_CollectClue)
      {
        SearchForClues();
        debug_CollectClue = false;
      }
      if (debug_ApplyDefense)
      {
        ApplyDefense();
        debug_ApplyDefense = false;
      }
      if (triggerAttack)
      {
        GhoulAttack();
        triggerAttack = false;
      }
    }

    public void ApplyDefense()
    {
      Debug.Log(targetWallDefense.name + " applied with " + defenseType.ToString());
      targetWallDefense.ApplyDefense(targetSegementDefense, defenseType);
    }
    void SelectWallToDefend(Wall wall)
    {
      targetWallDefense = wall;
    }
    public void SelectDefendMaria() => SelectWallToDefend(maria);
    public void SelectDefendSina() => SelectWallToDefend(sina);
    public void SelectDefendRose() => SelectWallToDefend(rose);

    void SelectWallSegmentToDefend(WallSegmentName segmentName)
    {
      targetSegementDefense = segmentName;
    }
    public void SelectDefenseTop() => SelectWallSegmentToDefend(WallSegmentName.top);
    public void SelectDefenseMiddle() => SelectWallSegmentToDefend(WallSegmentName.middle);
    public void SelectDefenseBottom() => SelectWallSegmentToDefend(WallSegmentName.bottom);
    void SelectDefenseType(DamageParameters.Type type)
    {
      defenseType = type;
    }
    public void SelectBludgeoningDefense() => SelectDefenseType(DamageParameters.Type.bludgeoning);
    public void SelectCorrosionDefense() => SelectDefenseType(DamageParameters.Type.corrosion);
    public void SelectFinesseDefense() => SelectDefenseType(DamageParameters.Type.finesse);

    /// <summary>
    /// Set all Walls' health and defense parameters to defaults
    /// </summary>
    public void InitializeWalls()
    {
      for (int i = 0; i < walls.Length; i++)
      {
        walls[i].InitializeWalls();
      }
    }

    public void OnNewDay()
    {
      RandomizeGhoul();
      SetupClues();
    }
    void RandomizeGhoul()
    {
      Debug.Log("Randomize Ghouls");
      activeGhoul = ghouls[UnityEngine.Random.Range(0, ghouls.Length)];
      ghoulTargetWall = walls[UnityEngine.Random.Range(0, walls.Length)];
      activeGhoul.targetWall = ghoulTargetWall;
      willAttackTonight = UnityEngine.Random.Range(0, 1) < attackPercentage;
      hoursUntilAttack = UnityEngine.Random.Range(0, 24 - dayNightManager.nightStart + dayNightManager.dayStart);
    }
    void SetupClues()
    {
      clueMain = activeGhoul.clueMain;
      clueSecondary = activeGhoul.clueSecondary;
      clueMainFound = false;
      clueSecondaryFound = false;
    }

    public void SearchForClues()
    {
      if (UnityEngine.Random.Range(0, 2) == 0 && !clueMainFound)
      {
        Debug.Log(clueMain.name + " was found on the shore.");
        clueMainFound = true;
      }
      else if (!clueSecondaryFound)
      {
        Debug.Log(clueSecondary.name + " was found on the shore.");
        clueSecondaryFound = true;
      }
      else if (!clueMainFound)
      {
        Debug.Log(clueMain.name + " was found on the shore.");
        clueMainFound = true;
      }
      else
      {
        Debug.Log("Nothing was found on the shore.");
      }
    }
    public void OnNightHour()
    {
      if (0 != hoursUntilAttack-- || !willAttackTonight) return;
      GhoulAttack();

    }
    public void GhoulAttack()
    {
      Debug.Log("Attacked!");
      //attack 1
      //     if (activeGhoul.MainAttack())
      //     {
      //         Debug.Log("Wall was breached! Game over.");
      //     }

      //     //attack 2
      //     if (activeGhoul.SecondaryAttack())
      //     {
      //         Debug.Log("Wall was breached! Game over.");
      //     }
      // }
    }
  }
}

