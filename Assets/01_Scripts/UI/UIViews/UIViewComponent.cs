using UnityEngine;

namespace WallDefense
{
    [CreateAssetMenu(fileName = "UIViewComponent", menuName = "Scriptable Objects/UIViewComponent")]
    public class UIViewComponent : ScriptableObject
    {
        public GameManager.UIView uiView;
    }
}
