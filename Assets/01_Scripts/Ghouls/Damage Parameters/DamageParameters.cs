using UnityEngine;

namespace WallDefense
{
    [CreateAssetMenu(fileName = "DamageParameters", menuName = "Scriptable Objects/DamageParameters")]
    public class DamageParameters : ScriptableObject
    {
        public enum Type
        {
            bludgeoning,
            corrosion,
            finesse,
            none
        }
        public Type type;
        public int magnitude;
        /// <summary>
        /// Percent to reduce to (.25 means attacks will be 25% effective not reduce by 25%)
        /// </summary>
        [Range(0, 1.0f)]
        public float reducedAmount;
    }
}
