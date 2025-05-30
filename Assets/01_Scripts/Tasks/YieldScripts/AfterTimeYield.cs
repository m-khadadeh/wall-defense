using System.Collections.Generic;
using UnityEngine;

namespace WallDefense
{
  [CreateAssetMenu(fileName = "AfterTimeYield", menuName = "Scriptable Objects/Tasks/Yields/After Time Yield")]
  public class AfterTimeYield : TaskYield
  {
    [SerializeField] private TaskYield _yield;
    [SerializeField] private int _hoursToTickDown;

    private List<QueuedSend> _queue;

    public void Initialize()
    {
      _queue = new List<QueuedSend>();
    }

    public override void GetYield(ColonyData colony, string choice, List<ItemType> consumed)
    {
      _queue.Add(new QueuedSend(colony, choice, consumed, _hoursToTickDown));
    }

    public void TickDown()
    {
      List<QueuedSend> toDelete = new List<QueuedSend>();
      if (_queue == null)
      {
        Initialize();
      }
      foreach (var queued in _queue)
        {
          if (queued.hoursLeft > 0)
          {
            queued.hoursLeft--;
            if (queued.hoursLeft == 0)
            {
              _yield.GetYield(queued.colony, queued.choice, queued.items);
              toDelete.Add(queued);
            }
          }
        }
      foreach (var queued in toDelete)
      {
        _queue.Remove(queued);
      }
    }

    private class QueuedSend
    {
      public ColonyData colony;
      public string choice;
      public List<ItemType> items;
      public int hoursLeft;
      public QueuedSend(ColonyData m_colony, string m_choice, List<ItemType> m_items, int m_hours)
      {
        colony = m_colony;
        choice = m_choice;
        items = m_items;
        hoursLeft = m_hours;
      }
    }
  }
}
