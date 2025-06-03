using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WallDefense.AI
{
  [CreateAssetMenu(fileName = "GhoulTimeBasedPriority", menuName = "Scriptable Objects/AI/GOAP/Ghoul Time Based Priority")]
  public class GhoulTimeBasedPriority : Priority
  {
    [SerializeField] private int _dayHour;
    [SerializeField] private int _nightHour;
    [SerializeField] private GhoulSelector _selector;
    [SerializeField] private List<CurvePoint> _curvePoints;
    private int _priority;
    public override int PriorityValue => _priority;
    private int _attackHour;
    private int _hourSinceAttack;
    private bool _attackHourUpdatedPostAttack;
    private List<Vector2Int> _priorityPoints;

    public override void Initialize()
    {
      _attackHour = 0;
      _hourSinceAttack = _dayHour;
      UpdateCurve();
      UpdatePriority();
      _attackHourUpdatedPostAttack = false;
    }

    public override void UpdatePriority()
    {
      for (int i = 0; i < _priorityPoints.Count; i++)
      {
        if (_hourSinceAttack == _priorityPoints[i].x || i == _priorityPoints.Count - 1)
        {
          // Right on the money OR it's the last point so we'll stay constant.
          _priority = _priorityPoints[i].y;
          return;
        }
        else if (_hourSinceAttack > _priorityPoints[i].x && _hourSinceAttack < _priorityPoints[i + 1].x)
        {
          // Lerp between the two points
          _priority = (int)Vector2.Lerp(_priorityPoints[i], _priorityPoints[i + 1],
              (float)(_hourSinceAttack - _priorityPoints[i].x) / (float)(_priorityPoints[i + 1].x - _priorityPoints[i].x)).y;
          return;
        }
      }
      throw new Exception($"Priority wasn't calculated for {_hourSinceAttack} hours since {_attackHour}");
    }

    public void OnHour(int hour)
    {
      if (_selector.HasAttacked && !_attackHourUpdatedPostAttack)
      {
        _attackHour = hour;
        _attackHourUpdatedPostAttack = true;
        UpdateCurve();
        _hourSinceAttack = 0;
      }
      else
      {
        if (!_selector.HasAttacked & _attackHourUpdatedPostAttack)
        {
          _attackHourUpdatedPostAttack = false;
        }
        _hourSinceAttack++;
      }
      UpdatePriority();
    }

    public void UpdateCurve()
    {
      _priorityPoints = new List<Vector2Int>();
      foreach (var point in _curvePoints)
      {
        switch (point.ReferencePoint)
        {
          case CurvePoint.POIPoint.AttackHour:
            _priorityPoints.Add(new Vector2Int(point.PositionFromPOI, point.Value));
            break;
          case CurvePoint.POIPoint.DayHour:
            _priorityPoints.Add(new Vector2Int(ConvertHourToSinceAttackHour(_dayHour) + point.PositionFromPOI, point.Value));
            break;
          case CurvePoint.POIPoint.NightHour:
            _priorityPoints.Add(new Vector2Int(ConvertHourToSinceAttackHour(_nightHour) + point.PositionFromPOI, point.Value));
            break;
          default:
            throw new System.Exception("Impossible Reference Point");
        }
      }
    }

    private int ConvertHourToSinceAttackHour(int realTimeHour)
    {
      if (realTimeHour == _attackHour)
      {
        return 0;
      }
      else if (realTimeHour < _attackHour)
      {
        return realTimeHour + 24 - _attackHour;
      }
      else
      {
        return realTimeHour - _attackHour;
      }
    }

    [Serializable]
    public class CurvePoint
    {
      public enum POIPoint { AttackHour, DayHour, NightHour }
      [field: SerializeField] public POIPoint ReferencePoint { get; private set; }
      [field: SerializeField] public int PositionFromPOI { get; private set; }
      [field: SerializeField] public int Value { get; private set; }
    }
  }
}
