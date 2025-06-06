using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WallDefense
{
  public class RadioController : MonoBehaviour
  {
    [SerializeField] private Animator _radioAnimator;
    [SerializeField] private GameObject _playerCallingLight;
    [SerializeField] private GameObject _playerReceivingLight;
    [SerializeField] List<string> _stationsAtIndices;
    [SerializeField] private DialogueManager _manager;

    private Dictionary<string, int> _stationDictionary;

    public void Awake()
    {
      _stationDictionary = _stationsAtIndices.ToDictionary(e => e, e => _stationsAtIndices.IndexOf(e));
      OnRadioOff();
    }

    public void OnRadioOff()
    {
      _playerCallingLight.SetActive(false);
      _playerReceivingLight.SetActive(false);
      _radioAnimator.SetTrigger("TurnRadioOff");
    }

    public void OnRadioOn(bool playerCalling, string stationName)
    {
      _playerCallingLight.SetActive(playerCalling);
      _playerReceivingLight.SetActive(!playerCalling);
      _radioAnimator.SetInteger("Station", _stationDictionary[stationName]);
      _radioAnimator.SetTrigger("TurnRadioOn");
    }

    public void OnEnable()
    {
      SetReceiving(_manager.NodeQueued);
    }

    public void SetReceiving(bool value)
    {
      _playerReceivingLight.SetActive(value);
    }

    public void OnPlayerStartingDialogue()
    {
      _playerCallingLight.SetActive(true);
    }
  }
}
