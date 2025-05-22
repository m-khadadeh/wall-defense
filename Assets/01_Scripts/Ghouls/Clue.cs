using UnityEngine;

namespace WallDefense
{
    [CreateAssetMenu(fileName = "Clue", menuName = "Scriptable Objects/Clue")]
    public class Clue : ScriptableObject
    {
        public ClueType clueType;
        public enum ClueType
        {
            A, B, C, D
        }
    }
}
