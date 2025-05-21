using NUnit.Framework;
using UnityEngine;

namespace WallDefense
{
    [CreateAssetMenu(fileName = "Wall", menuName = "Scriptable Objects/Wall")]
    public class Wall : ScriptableObject
    {
        public int ownerSettlement;
        public float damageReductionAmount;
        public WallSegment top;
        public WallSegment middle;
        public WallSegment bottom;
        public WallSegment[] walls;
        public void InitializeWalls()
        {
            bottom.health = 200;
            middle.health = 150;
            top.health = 100;
            walls[0] = bottom;
            walls[1] = middle;
            walls[2] = top;
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].damageReductionAmount = damageReductionAmount;
                walls[i].hasBludgeoningDefense = false;
                walls[i].hasCorrosionDefense = false;
                walls[i].hasFinesseDefense = false;
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
    }
    public struct WallSegment
    {
        public int wallIndex; //lowest is 0, highest is 2
        public float health;
        public bool hasBludgeoningDefense;
        public bool hasCorrosionDefense;
        public bool hasFinesseDefense;
        public float damageReductionAmount;
        public enum DamageType
        {
            bludgeoning,
            corrosion,
            finesse
        }
        public void ApplyDefense(DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.bludgeoning:
                    hasBludgeoningDefense = true;
                    break;
                case DamageType.corrosion:
                    hasCorrosionDefense = true;
                    break;
                case DamageType.finesse:
                    hasFinesseDefense = true;
                    break;
                default:
                    break;
            }
        }
        public void ApplyDamage(DamageType damageType, float baseDamage)
        {
            float finalDamage;
            switch (damageType)
            {
                case DamageType.bludgeoning:
                    finalDamage = baseDamage * (hasBludgeoningDefense ? damageReductionAmount : 1);
                    hasBludgeoningDefense = false;
                    break;
                case DamageType.corrosion:
                    finalDamage = baseDamage * (hasCorrosionDefense ? 0 : 1);
                    hasCorrosionDefense = false;
                    break;
                case DamageType.finesse:
                    finalDamage = baseDamage * (hasFinesseDefense ? damageReductionAmount : 1);
                    hasFinesseDefense = false;
                    break;
                default:
                    finalDamage = baseDamage;
                    break;
            }
            health -= finalDamage;
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
