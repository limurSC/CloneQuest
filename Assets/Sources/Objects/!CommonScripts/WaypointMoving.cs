using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LinearMoving))]
public abstract class WaypointMoving : MonoBehaviour
{
    public IEnumerable<Vector2> Waypoints => _waypoints;
    public UnityEvent WaypointReached;
    public UnityEvent TargetReached;
    [SerializeField] private Vector2[] _waypoints;
    [SerializeField][Range(0.1f, 10f)] private float _velocity = 1f;
    [SerializeField] private LinearMoving _linearMoving;
    private int _index = 0;
    private int _lastDirection = 0;

    protected void MoveForward()
    {
        if (_lastDirection != 1) { _index += 1; }
        if (_index >= _waypoints.Length) { _index = _waypoints.Length - 1; return; }
        _lastDirection = 1;
        _linearMoving.MoveTo(_waypoints[_index], _velocity);
    }

    protected void MoveBackward()
    {
        if (_lastDirection != -1) { _index -= 1; }
        if (_index < 0) { _index = 0; return; }
        _lastDirection = -1;
        _linearMoving.MoveTo(_waypoints[_index], _velocity);
    }

    protected void StopMoving()
    {
        _linearMoving.Stop();
    }

    private void WaypointReachedCallback()
    {
        _lastDirection = 0;
        if (_index == 0 || _index == _waypoints.Length - 1) { TargetReached.Invoke(); }
        WaypointReached.Invoke();
    }

    private void Awake()
    {
        _linearMoving.TargetReached += WaypointReachedCallback;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_linearMoving == null) { _linearMoving = GetComponent<LinearMoving>(); }
        if (_waypoints.Length == 0 || _waypoints[0] != Vector2.zero)
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