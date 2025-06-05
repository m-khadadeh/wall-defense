using UnityEngine;
using Yarn.Unity;

namespace WallDefense
{
  public class DialogueCommandRunner : MonoBehaviour
  {
    [SerializeField] private DialogueManager _manager;
    [SerializeField] private Capitol _capitol;

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
    public void StartComms(string target)
    {
      Debug.Log($"Starting comms with {target}");
    }
    [YarnCommand("comms_end")]
    public void EndComms()
    {
      Debug.Log($"Ending Comms");
    }
  }
}
