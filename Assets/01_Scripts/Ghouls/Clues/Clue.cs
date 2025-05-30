using UnityEngine;

namespace WallDefense
{
    [CreateAssetMenu(fileName = "Clue", menuName = "Scriptable Objects/Clue")]
    public class Clue : ScriptableObject
    {
        public ClueType clueType;
        public string description;
        public enum ClueType
        {
            A, B, C, D
        }

        public bool found;

        public string dialogBoxInfo;
    }
}
