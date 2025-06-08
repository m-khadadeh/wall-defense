using UnityEngine;

namespace WallDefense
{
    public enum WallSegmentName
    {
        top, middle, bottom, none
    }
    [CreateAssetMenu(fileName = "Ghoul", menuName = "Scriptable Objects/Ghoul")]
    public class Ghoul : ScriptableObject
    {
        public Clue clueMain, clueSecondary;
        public DamageParameters parametersMain, parametersSecondary;
        public WallSegmentName damageTargetMain, damageTargetSecondary;
        public Wall targetWall;
        public Vector2Int hourRange;
        public string morseTextRepresentation;
        public string firstAttackRep;
        public string secondAttackRep;
        public string ghoulBreachRep;
        /// <summary>
        /// Applies main attack effects
        /// </summary>
        /// <returns>True, if wall was broken prior to attack</returns>
        public bool MainAttack(out (bool, DamageParameters.Type) defended)
        {
            return Attack(damageTargetMain, parametersMain, out defended);
        }
        /// <summary>
        /// Applies secondary attack effects
        /// </summary>
        /// <returns>True, if wall was broken prior to attack</returns>
        public bool SecondaryAttack(out (bool, DamageParameters.Type) defended)
        {
            return Attack(damageTargetSecondary, parametersSecondary, out defended);
        }
        /// <summary>
        /// Applies attack damage to wall segment specified
        /// </summary>
        /// <param name="wallSegmentName">Top, Middle, or Bottom Segment</param>
        /// <param name="damageType">Blugeoning, Corrosion, or Finesse</param>
        /// <param name="damageMagnitude">Unmitigated damage value</param>
        /// <returns>True, if wall was broken prior to attack</returns>
        public bool Attack(WallSegmentName wallSegmentName, DamageParameters damageParameters, out (bool, DamageParameters.Type) defended)
        {
            switch (wallSegmentName)
            {
                case WallSegmentName.bottom:
                    if (targetWall.bottom.IsDestroyed())
                    {
                        defended = (false, DamageParameters.Type.none);
                        return true;
                    }
                    targetWall.bottom.ApplyDamage(damageParameters, out defended);
                    break;
                case WallSegmentName.middle:
                    if (targetWall.middle.IsDestroyed())
                    {
                        defended = (false, DamageParameters.Type.none);
                        return true;
                    }
                    targetWall.middle.ApplyDamage(damageParameters, out defended);
                    break;
                case WallSegmentName.top:
                    if (targetWall.top.IsDestroyed())
                    {
                        defended = (false, DamageParameters.Type.none);
                        return true;
                    }
                    targetWall.top.ApplyDamage(damageParameters, out defended);
                    break;
                default:
                    defended = (false, DamageParameters.Type.none);
                    break;
            }
            targetWall.CheckSegmentFailure();
            return false;
        }

    }
}
