using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LinearMoving))]
public class WaypointMoving : MonoBehaviour
{
    public Vector2 Pivot => _linearMoving.Pivot;
    public ReadOnlyCollection<Vector2> Points => Array.AsReadOnly(_waypoints);
    public bool Moving => _movingDirection != 0;
    public bool AtWaypoint => _directionFromLastWaypoint != 0;
    public UnityEvent OnWaypointReached;
    public UnityEvent<int> OnEdgeReached;
    [SerializeField] private Vector2[] _waypoints;
    [SerializeField] private LinearMoving _linearMoving;
    private int _index = 0;
    private int _movingDirection = 0;
    private float _velocity;
    private int _directionFromLastWaypoint = 0;

    public void MoveForward(float velocity) => Move(1, velocity);
    public void MoveBackward(float velocity) => Move(-1, velocity);
    public void MoveForwardByTime(float time) => Move(1, GetDistanceToEdge(1) / time);
    public void MoveBackwardByTime(float time) => Move(-1, GetDistanceToEdge(-1) / time);
    public void StopAtWaypoint() => _movingDirection = 0;
    public void Stop() { _movingDirection = 0; _linearMoving.Stop(); }

    private void Move(int direction, float velocity)
    {
        if (_movingDirection != 0) { Stop(); }
        _velocity = velocity;
        if (direction != _directionFromLastWaypoint) { _index += direction; }
        if (direction > 0 && _index >= _waypoints.Length) { _index = _waypoints.Length - 1; OnEdgeReached.Invoke(1); }
        else if (direction < 0 && _index < 0) { _index = 0; OnEdgeReached.Invoke(-1); }
        else { _movingDirection = direction; MoveWaypoint(); }
    }

    public float GetDistanceToEdge(int direction)
    {
        var time = _directionFromLastWaypoint == 0 ? 0f : (_waypoints[_index] - _linearMoving.RelativePosition).magnitude * (_directionFromLastWaypoint == direction ? 1 : -1);
        if (direction > 0)
        {
            for (var i = _index + 1; i < _waypoints.Length; i++)
            {
                time += (_waypoints[i - 1] - _waypoints[i]).magnitude;
            }
        }
        else
        {
            for (var i = 0; i < _index; i++)
            {
                time += (_waypoints[i + 1] - _waypoints[i]).magnitude;
            }
        }
        return time;
    }

    private void MoveWaypoint()
    {
        if (_movingDirection == 1) { MoveForwardWaypoint(); }
        else if (_movingDirection == -1) { MoveBackwardWaypoint(); }
    }
    private void MoveForwardWaypoint()
    {
        _directionFromLastWaypoint = 1;
        _linearMoving.MoveTo(_waypoints[_index], _velocity);
    }
    private void MoveBackwardWaypoint()
    {
        _directionFromLastWaypoint = -1;
        _linearMoving.MoveTo(_waypoints[_index], _velocity);
    }

    private void WaypointReached()
    {
        OnWaypointReached.Invoke();
        _directionFromLastWaypoint = 0;
        if (_index == 0 | _index == _waypoints.Length - 1)
        {
            _movingDirection = 0;
            OnEdgeReached.Invoke(_index == 0 ? -1 : 1);
        }
        else { _index += _movingDirection; MoveWaypoint(); };
    }

    private void Awake()
    {
        _linearMoving.TargetReached += WaypointReached;
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_linearMoving == null) { _linearMoving = GetComponent<LinearMoving>(); }
        if (_waypoints == null) { _waypoints = new[] { Vector2.zero }; }
        else if (_waypoints.Length == 0 || _waypoints[0] != Vector2.zero)
        {
            _waypoints = _waypoints.Prepend(Vector2.zero).ToArray();
        }
    }
    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeGameObject != gameObject || _waypoints.Length == 0) return;
        var pivot = (Vector2)(!Application.isPlaying ? transform.position : _linearMoving.Pivot);
        var prev = pivot;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(prev, 0.3f);
        for (var i = 1; i < _waypoints.Length; i++)
        {
            var current = pivot + _waypoints[i];
            Gizmos.DrawWireSphere(current, 0.3f);
            Gizmos.DrawLine(prev, current);
            prev = current;
        }
    }
#endif
}