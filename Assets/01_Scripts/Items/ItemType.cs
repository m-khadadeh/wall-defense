using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "Item Type", menuName = "Scriptable Objects/Item Type")]
  public class ItemType : ScriptableObject
  {
    [field: SerializeField] public string ItemName { get; protected set; }
    [field: SerializeField] public Sprite Sprite { get; protected set; }
    [field: SerializeField] public Sprite OutlineSprite { get; protected set; }
    [field: SerializeField] public Metadata Metadata { get; protected set; }
    [field: SerializeField] public MetadataValidator Validator { get; protected set; }
  }
}
