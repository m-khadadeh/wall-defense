using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  public class TutorialKun : MonoBehaviour
  {
    [SerializeField] private List<GameObject> _startArrows;
    [SerializeField] private List<GameObject> _morseArrows;

    public bool MorseArrowsUp { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize()
    {
      foreach (var arrow in _morseArrows)
      {
        arrow.SetActive(false);
      }
      foreach (var arrow in _startArrows)
      {
        arrow.SetActive(false);
      }
      MorseArrowsUp = false;
    }

    public void ShowStartArrows(bool show)
    {
      foreach (var arrow in _startArrows)
      {
        arrow.SetActive(false);
      }
    }

    public void ShowRadioArrows(bool show)
    {
      foreach (var arrow in _morseArrows)
      {
        arrow.SetActive(false);
      }
      MorseArrowsUp = show;
    }
  }
}
