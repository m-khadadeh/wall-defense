using System.IO;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

namespace WallDefense
{

    [CreateAssetMenu(fileName = "Wall", menuName = "Scriptable Objects/Wall")]
    public class Wall : ScriptableObject
    {
        public string wallName;
        public int ownerSettlement;
        public WallSegment top;
        public WallSegment middle;
        public WallSegment bottom;
        public bool isFallen;
        public int[] maxHealths;
        public int[] startingHealths;
        /// <summary>
        /// Set Wall health and defense parameters to defaults
        /// </summary>
        public void InitializeWalls()
        {
            isFallen = false;
            bottom = new()
            {
                wallIndex = 0,
                health = startingHealths[0],
                maxhealth = maxHealths[0],
                currentDefenseType = DamageParameters.Type.none
            };
            middle = new()
            {
                wallIndex = 1,
                health = startingHealths[1],
                maxhealth = maxHealths[1],
                currentDefenseType = DamageParameters.Type.none
            };
            top = new()
            {
                wallIndex = 2,
                health = startingHealths[2],
                maxhealth = maxHealths[2],
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
        public void RepairWall(WallSegmentName segmentName, int amount)
        {
            switch (segmentName)
            {
                case WallSegmentName.bottom:
                    bottom.Repair(amount);
                    break;
                case WallSegmentName.middle:
                    middle.Repair(amount);
                    break;
                case WallSegmentName.top:
                    top.Repair(amount);
                    break;
                default:
                    break;
            }
            CheckSegmentFailure();
        }

        public WallSegment GetSegmentFromName(WallSegmentName segmentName)
        {
            switch (segmentName)
            {
                case WallSegmentName.bottom:
                    return bottom;
                case WallSegmentName.middle:
                    return middle;
                case WallSegmentName.top:
                    return top;
                default:
                    throw new InvalidDataException("segment nonexistent");
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
        public int maxhealth;
        public DamageParameters damageParameters;
        public DamageParameters.Type currentDefenseType;

        public void Repair(int repairAmount)
        {
            health += repairAmount;
            health = Mathf.Min(health, maxhealth);
            Debug.Log(health);
        }
        public void ApplyDefense(DamageParameters.Type defenseType)
        {
            currentDefenseType = defenseType;
        }

        public void ApplyDamage(DamageParameters damageParameters, out (bool, DamageParameters.Type) defended)
        {
            defended = (currentDefenseType == damageParameters.type, currentDefenseType);
            ApplyDamage(damageParameters);
        }

        public void ApplyDamage(DamageParameters damageParameters)
        {
            health -= (int)math.round((float)damageParameters.magnitude * (currentDefenseType == damageParameters.type ? damageParameters.reducedAmount : 1.0f));
            currentDefenseType = DamageParameters.Type.none;
            health = Mathf.Max(0, health);
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
