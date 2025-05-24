using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace WallDefense
{
    public class WallManager : MonoBehaviour
    {
        public Wall maria, sina, rose;

        Wall[] walls = new Wall[3];

        public bool debug_Daybreak;
        public bool debug_Nightfall;
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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            walls[0] = maria;
            walls[1] = sina;
            walls[2] = rose;
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].InitializeWalls();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (debug_Daybreak)
            {
                Debug.Log("A new day breaks.");
                //pick ghoul
                activeGhoul = ghouls[UnityEngine.Random.Range(0, ghouls.Length)];
                //set clues
                clueMain = activeGhoul.clueMain;
                clueSecondary = activeGhoul.clueSecondary;
                debug_Daybreak = false;
                ghoulTargetWall = walls[UnityEngine.Random.Range(0, walls.Length)];
                activeGhoul.targetWall = ghoulTargetWall;
                clueMainFound = false;
                clueSecondaryFound = false;
            }

            if (debug_CollectClue)
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
                debug_CollectClue = false;
            }
            if (debug_ApplyDefense)
            {
                Debug.Log(targetWallDefense.name + " applied with " + defenseType.ToString());
                targetWallDefense.ApplyDefense(targetSegementDefense, defenseType);
                debug_ApplyDefense = false;
            }
            if (debug_Nightfall)
            {
                Debug.Log("Night has fallen");

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


                debug_Nightfall = false;
            }
        }
    }
}
