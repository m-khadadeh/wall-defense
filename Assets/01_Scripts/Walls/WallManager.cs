using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace WallDefense
{
    public class WallManager : MonoBehaviour
    {
        public Wall maria, sina, rose;

        Wall[] walls = new Wall[3];
        public bool triggerAttack;
        public bool debug_CollectClue;
        public bool debug_ApplyDefense;
        public DamageParameters.Type defenseType;
        public Wall targetWallDefense;
        public WallSegmentName targetSegementDefense;
        public Ghoul[] ghouls;
        public Ghoul activeGhoul;
        public Clue clueMain, clueSecondary;
        public bool clueMainFound, clueSecondaryFound;
        public Wall ghoulTargetWall;
        public int attackHour;

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
                Debug.Log(targetWallDefense.name + " applied with " + defenseType.ToString());
                targetWallDefense.ApplyDefense(targetSegementDefense, defenseType);
                debug_ApplyDefense = false;
            }
            if (triggerAttack)
            {
                GhoulAttack();
                triggerAttack = false;
            }
        }

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
            activeGhoul = ghouls[UnityEngine.Random.Range(0, ghouls.Length)];
            ghoulTargetWall = walls[UnityEngine.Random.Range(0, walls.Length)];
            activeGhoul.targetWall = ghoulTargetWall;
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

        public void GhoulAttack()
        {
            //attack 1
            if (activeGhoul.MainAttack())
            {
                Debug.Log("Wall was breached! Game over.");
            }

            //attack 2
            if (activeGhoul.SecondaryAttack())
            {
                Debug.Log("Wall was breached! Game over.");
            }
        }
    }
}
