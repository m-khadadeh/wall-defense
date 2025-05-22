using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

namespace WallDefense
{
    public enum DamageType
    {
        bludgeoning,
        corrosion,
        finesse,
        none
    }

    [CreateAssetMenu(fileName = "Wall", menuName = "Scriptable Objects/Wall")]
    public class Wall : ScriptableObject
    {
        public int ownerSettlement;
        public float damageReductionAmount;
        public WallSegment top;
        public WallSegment middle;
        public WallSegment bottom;
        public void InitializeWalls()
        {
            bottom = new()
            {
                wallIndex = 0,
                health = 200,
                _damageReductionAmount = damageReductionAmount,
                currentDefenseType = DamageType.none
            };
            middle = new()
            {
                wallIndex = 1,
                health = 150,
                _damageReductionAmount = damageReductionAmount,
                currentDefenseType = DamageType.none
            };
            top = new()
            {
                wallIndex = 2,
                health = 100,
                _damageReductionAmount = damageReductionAmount,
                currentDefenseType = DamageType.none
            };
        }
        public void ApplyDefense(WallSegmentName segmentName, DamageType damageType)
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

            //TODO fail state
        }
    }
    [System.Serializable]
    public struct WallSegment
    {
        public int wallIndex; //lowest is 0, highest is 2
        public int health;
        public DamageType currentDefenseType;
        public float _damageReductionAmount;

        public void Repair(int repairAmount)
        {
            health += repairAmount;
        }
        public void ApplyDefense(DamageType defenseType)
        {
            currentDefenseType = defenseType;
        }
        public void ApplyDamage(DamageType damageType, int baseDamage)
        {
            float finalDamage;
            switch (damageType)
            {
                case DamageType.bludgeoning:
                    finalDamage = baseDamage * (currentDefenseType == DamageType.bludgeoning ? _damageReductionAmount : 1);
                    currentDefenseType = DamageType.none;
                    break;
                case DamageType.corrosion:
                    finalDamage = baseDamage * (currentDefenseType == DamageType.corrosion ? 0 : 1);
                    currentDefenseType = DamageType.none;
                    break;
                case DamageType.finesse:
                    finalDamage = baseDamage * (currentDefenseType == DamageType.finesse ? _damageReductionAmount : 1);
                    currentDefenseType = DamageType.none;
                    break;
                default:
                    finalDamage = baseDamage;
                    break;
            }
            //TODO if health is already 0 fail state
            health -= (int)math.round(finalDamage);
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
