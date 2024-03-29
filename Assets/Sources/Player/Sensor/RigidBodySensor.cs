using UnityEngine;
using System.Linq;
using System;

public class RigidBodySensor : Sensor
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Vector2 _direction;
    [SerializeField] private float _distance;

    public override RaycastHit2D Hit => TimeUpdate() ? CastUpdate() : _lastHit;

    private RaycastHit2D _lastHit;
    private ContactFilter2D _contactFilter;
    private float _lastUpdated = 0f;

    private void Awake()
    {
        _contactFilter.SetLayerMask(_layerMask);
    }

    private bool TimeUpdate()
    {
        var time = Time.fixedTime;
        if (time <= _lastUpdated) { return false; }
        _lastUpdated = time;
        return true;
    }

    [ContextMenu("Update")]
    private RaycastHit2D CastUpdate()
    {
        var castResults = new RaycastHit2D[3];
        var count = _rigidbody.Cast(_direction, _contactFilter, castResults, _distance);
        if (count <= 1) 
        {
            _lastHit = castResults[0];
            return _lastHit;
        }
        return castResults
            .Where(cast => cast.collider)
            .Aggregate((min, next) => MathF.Abs(min.normal.x) < MathF.Abs(next.normal.x) ? min : next);
    }


    private void OnValidate()
    {
        _direction.Normalize();
        if (!_rigidbody) { _rigidbody = GetComponent<Rigidbody2D>(); }
    }

    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.activeGameObject != gameObject || !enabled) return;
        Gizmos.color = Hit.collider ? Color.yellow : Color.grey;
        Gizmos.DrawRay(_rigidbody.position, _direction * _distance);
    }
}
