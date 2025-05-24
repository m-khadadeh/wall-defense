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
        public DamageParameters parametersMain, parametersSecondary;
        public WallSegmentName damageTargetMain, damageTargetSecondary;
        public Wall targetWall;
        /// <summary>
        /// Applies main attack effects
        /// </summary>
        /// <returns>True, if wall was broken prior to attack</returns>
        public bool MainAttack()
        {
            return Attack(damageTargetMain, parametersMain);
        }
        /// <summary>
        /// Applies secondary attack effects
        /// </summary>
        /// <returns>True, if wall was broken prior to attack</returns>
        public bool SecondaryAttack()
        {
            return Attack(damageTargetSecondary, parametersSecondary);
        }
        /// <summary>
        /// Applies attack damage to wall segment specified
        /// </summary>
        /// <param name="wallSegmentName">Top, Middle, or Bottom Segment</param>
        /// <param name="damageType">Blugeoning, Corrosion, or Finesse</param>
        /// <param name="damageMagnitude">Unmitigated damage value</param>
        /// <returns>True, if wall was broken prior to attack</returns>
        public bool Attack(WallSegmentName wallSegmentName, DamageParameters damageParameters)
        {
            switch (wallSegmentName)
            {
                case WallSegmentName.bottom:
                    if (targetWall.bottom.IsDestroyed())
                    {
                        return true;
                    }
                    targetWall.bottom.ApplyDamage(damageParameters);
                    break;
                case WallSegmentName.middle:
                    if (targetWall.middle.IsDestroyed())
                    {
                        return true;
                    }
                    targetWall.middle.ApplyDamage(damageParameters);
                    break;
                case WallSegmentName.top:
                    if (targetWall.top.IsDestroyed())
                    {
                        return true;
                    }
                    targetWall.top.ApplyDamage(damageParameters);
                    break;
                default:
                    break;
            }
            targetWall.CheckSegmentFailure();
            return false;
        }

    }
}
