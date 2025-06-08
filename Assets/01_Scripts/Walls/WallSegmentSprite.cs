using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WallDefense
{
  public class WallSegmentSprite : MonoBehaviour
  {
    [SerializeField] Sprite _defaultSprite;
    [SerializeField] List<Sprite> _sprites;
    [SerializeField] List<int> _maxHps;
    [SerializeField] Image _image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Initialize()
    {
      _image.sprite = _defaultSprite;
    }
    
    public void OnHPChange(int hp)
    {
      int i;
      for (i = 0; i < _maxHps.Count; i++)
      {
        if (hp < _maxHps[i]) break;
      }
      _image.sprite = _sprites[i];
    }
  }
}
