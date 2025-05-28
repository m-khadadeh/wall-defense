using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

namespace WallDefense
{

    [CreateAssetMenu(fileName = "Wall", menuName = "Scriptable Objects/Wall")]
    public class Wall : ScriptableObject
    {
        public int ownerSettlement;
        public WallSegment top;
        public WallSegment middle;
        public WallSegment bottom;
        public bool isFallen;
        /// <summary>
        /// Set Wall health and defense parameters to defaults
        /// </summary>
        public void InitializeWalls()
        {
            isFallen = false;
            bottom = new()
            {
                wallIndex = 0,
                health = 200,
                currentDefenseType = DamageParameters.Type.none
            };
            middle = new()
            {
                wallIndex = 1,
                health = 150,
                currentDefenseType = DamageParameters.Type.none
            };
            top = new()
            {
                wallIndex = 2,
                health = 100,
                currentDefenseType = DamageParameters.Type.none
            };
        }
        public void ApplyDefense(WallSegmentName segmentName, DamageParameters.Type damageType)
        {
            switch (segmentName)
            {
                case WallSegmentName.bottom:
                    bottom.ApplyDefense(damageType);
                    break;
                case WallSegmentName.middle:
                    middle.ApplyDefense(damageType);
                    break;
                case WallSegmentName.top:
                    top.ApplyDefense(damageType);
                    break;
                default:
                    break;

            }
        }

        public void CheckSegmentFailure()
        {
            if (bottom.IsDestroyed())
            {
                middle.Collapse();
            }
            if (middle.IsDestroyed())
            {
                top.Collapse();
            }
        }
        public void LoadWallData(WallData wallData)
        {
            ownerSettlement = wallData.ownerSettlement;
            top = wallData.top;
            middle = wallData.middle;
            bottom = wallData.bottom;

        }
    }
    [System.Serializable]
    public struct WallSegment
    {
        public int wallIndex; //lowest is 0, highest is 2
        public int health;
        public DamageParameters damageParameters;
        public DamageParameters.Type currentDefenseType;

        public void Repair(int repairAmount)
        {
            health += repairAmount;
        }
        public void ApplyDefense(DamageParameters.Type defenseType)
        {
            currentDefenseType = defenseType;
        }
        public void ApplyDamage(DamageParameters damageParameters)
        {
            health -= (int)math.round(damageParameters.magnitude * (currentDefenseType == damageParameters.type ? damageParameters.reducedAmount : 1));
            currentDefenseType = DamageParameters.Type.none;
        }
        public readonly bool IsDestroyed()
        {
            return health <= 0;
        }
        public void Collapse()
        {
            health = 0;
        }
    }
}
