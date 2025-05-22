using UnityEngine;

namespace WallDefense
{
    public enum WallSegmentName
    {
        top, middle, bottom
    }
    [CreateAssetMenu(fileName = "Ghoul", menuName = "Scriptable Objects/Ghoul")]
    public class Ghoul : ScriptableObject
    {
        public Clue clueMain, clueSecondary;
        public DamageType damageTypeMain, damageTypeSecondary;
        public int damageMagnitudeMain, damageMagnitudeSecondary;
        public WallSegmentName damageTargetMain, damageTargetSecondary;
        public Wall targetWall;
        /// <summary>
        /// Applies main attack effects
        /// </summary>
        /// <returns>True, if wall was broken prior to attack</returns>
        public bool MainAttack()
        {
            return Attack(damageTargetMain, damageTypeMain, damageMagnitudeMain);
        }
        /// <summary>
        /// Applies secondary attack effects
        /// </summary>
        /// <returns>True, if wall was broken prior to attack</returns>
        public bool SecondaryAttack()
        {
            return Attack(damageTargetSecondary, damageTypeSecondary, damageMagnitudeSecondary);
        }
        /// <summary>
        /// Applies attack damage to wall segment specified
        /// </summary>
        /// <param name="wallSegmentName">Top, Middle, or Bottom Segment</param>
        /// <param name="damageType">Blugeoning, Corrosion, or Finesse</param>
        /// <param name="damageMagnitude">Unmitigated damage value</param>
        /// <returns>True, if wall was broken prior to attack</returns>
        public bool Attack(WallSegmentName wallSegmentName, DamageType damageType, int damageMagnitude)
        {
            switch (wallSegmentName)
            {
                case WallSegmentName.bottom:
                    if (targetWall.bottom.IsDestroyed())
                    {
                        return true;
                    }
                    targetWall.bottom.ApplyDamage(damageType, damageMagnitude);
                    break;
                case WallSegmentName.middle:
                    if (targetWall.middle.IsDestroyed())
                    {
                        return true;
                    }
                    targetWall.middle.ApplyDamage(damageType, damageMagnitude);
                    break;
                case WallSegmentName.top:
                    if (targetWall.top.IsDestroyed())
                    {
                        return true;
                    }
                    targetWall.top.ApplyDamage(damageType, damageMagnitude);
                    break;
                default:
                    break;
            }
            targetWall.CheckSegmentFailure();
            return false;
        }

    }
}
