using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace WallDefense
{
  public class DialogueCommandRunner : MonoBehaviour
  {
    [SerializeField] private DialogueManager _manager;
    [SerializeField] private Capitol _capitol;
    [SerializeField] private RadioController _radio;
    [SerializeField] private ColonyData _ws2;
    [SerializeField] private ColonyData _ws3;

    [YarnCommand("request_material")]
    public void RequestMaterial(string aiObjectName, string materialType, int amount) => _manager.RequestMaterial(aiObjectName, materialType, amount);
    [YarnCommand("get_required_materials")]
    public void GetRequiredMaterials(string aiObjectName) => _manager.GetRequiredMaterials(aiObjectName);
    [YarnCommand("make_promise")]
    public void MakePromise(string aiObjectName, string materialType, int amount) => _manager.MakePromise(aiObjectName, materialType, amount);
    [YarnCommand("give_information")]
    public void GiveInfo(string aiObjectName, string clue) => _manager.GiveInfo(aiObjectName, clue);
    [YarnCommand("capitol_initial_shipment")]
    public void InitialCapitolShipment() => _capitol.InitialCapitolShipment();
    [YarnCommand("capitol_weekly_shipment")]
    public void WeeklyCapitolShipment() => _capitol.WeeklyCapitolShipment();
    [YarnCommand("comms_start")]
    public void StartComms(string target, bool playerCalling)
    {
      _radio.OnRadioOn(playerCalling, target);
    }
    [YarnCommand("comms_end")]
    public void EndComms()
    {
      _radio.OnRadioOff();
    }
    [YarnCommand("initiate")]
    public void InitiateContact(string target)
    {
      switch (target)
      {
        case "capitol":
          _manager.QueueNodeNow(_capitol.PlayerInitiationNode);
          break;
        case "WS2":
          _manager.QueueNodeNow(_ws2.AIController.PlayerInitiationNode);
          break;
        case "WS3":
          _manager.QueueNodeNow(_ws3.AIController.PlayerInitiationNode);
          break;
        default:
          break;
      }
    }
    [YarnCommand("set_receiving")]
    public void SetReceivingLight(bool value)
    {
      _radio.SetReceiving(value);
    }
  }
}
